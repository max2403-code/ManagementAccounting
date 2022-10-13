using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class PreOrder : IPreOrder 
    {
        public event Action ExceptionEvent;
        private decimal _quantity;
        private ICalculation _calculation;
        private DateTime _creationDate;
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _itemsFactory;

        public string Name => string.Join(' ', _calculation.Name, "от", _creationDate.ToString("dd/MM/yyyy"));
        public int Index { get; }
        public int CalculationId => _calculation.Index;
        public decimal Quantity => _quantity;
        public string ItemTypeName { get; }
        public int LengthOfItemsList { get; }
        public bool IsCostPriceFull { get; private set; }
        public decimal MaxCostPrice { get; private set; }
        public decimal MinCostPrice { get; private set; }
        public DateTime? ProductionDate { get; private set; }


        public PreOrder(ICalculation calculation, decimal quantity, DateTime creationDate, int index, IBlockItemsFactory blockItemFactory, IDataBase dataBase)
        {
            ItemTypeName = "preorderitem";
            LengthOfItemsList = 5;
            _creationDate = creationDate;
            _quantity = quantity;
            _calculation = calculation;
            Index = index;
            _dataBase = dataBase;
            _itemsFactory = blockItemFactory;
            IsCostPriceFull = true;
        }

        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CalculationIdPO", NpgsqlDbType.Integer, CalculationId);
            cmd.Parameters.AddWithValue("QuantityPO", NpgsqlDbType.Numeric, Quantity);
            cmd.Parameters.AddWithValue("CreationDatePO", NpgsqlDbType.Date, _creationDate);
            cmd.Parameters.AddWithValue("SearchNamePO", NpgsqlDbType.Varchar, 64, Name);
        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO preorders (CalculationIdPO, QuantityPO, CreationDatePO, SearchNamePO) VALUES (@CalculationIdPO, @QuantityPO, @CreationDatePO, @SearchNamePO)";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблема с БД", AssignParametersToAddCommand);

            ExceptionEvent?.Invoke();
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("QuantityPO", NpgsqlDbType.Numeric, Quantity);
            cmd.Parameters.AddWithValue("CreationDatePO", NpgsqlDbType.Date, _creationDate);
            cmd.Parameters.AddWithValue("SearchNamePO", NpgsqlDbType.Varchar, 64, Name);
        }

        public async Task EditItemInDataBase(params object[] parameters)
        {
            //var copyPreOrder = _itemsFactory.CreatePreOrder(_calculation, Quantity, _creationDate, Index);
            var lastQuantity = Quantity;
            var lastCreationDate = _creationDate;
            var commandText = $"UPDATE preorders SET QuantityPO = @QuantityPO, CreationDatePO = @CreationDatePO, SearchNamePO = @SearchNamePO WHERE idpo = {Index}";

            _quantity = (decimal)parameters[0];
            _creationDate = (DateTime) parameters[1];

            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблема с БД", AssignParametersToEditCommand);

            if (ExceptionEvent != null)
            {
                _quantity = lastQuantity;
                _creationDate = lastCreationDate;
                ExceptionEvent.Invoke();
            }
        }

        public async Task RemoveItemFromDataBase()
        {
            var commandText = $"DELETE FROM preorders WHERE idpo = {Index}";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблема с БД");
            ExceptionEvent?.Invoke();
        }

        
        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT r.MaterialNameM, r.IdM, ci.QuantityCI, (SELECT MIN(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS minprice, (SELECT MAX(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS maxprice FROM calculationitems AS ci, remainders AS r WHERE ci.CalculationIdCI = {CalculationId} AND ci.MaterialIdCI = r.IdM ORDER BY r.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var materialName = (string) item["MaterialNameM"];
            var materialId = (int) item["IdM"];
            var materialConsumption = (decimal) item["QuantityCI"];
            var minPrice = item["minprice"] is DBNull ? -1m : (decimal)item["minprice"];
            var maxPrice = item["maxprice"] is DBNull ? -1m : (decimal)item["maxprice"];

            return _itemsFactory.CreatePreOrderItem(materialName, materialId, materialConsumption, minPrice, maxPrice);
        }

        public async Task AssignCostPrice()
        {
            var offset = 0;
            var itemsLength = 50;

            while (true)
            {
                var isEndOfItems = false;
                var commandText = $"SELECT ci.QuantityCI, (SELECT MIN(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS minprice, (SELECT MAX(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS maxprice FROM calculationitems AS ci WHERE ci.CalculationIdCI = {CalculationId} LIMIT {itemsLength + 1} OFFSET {offset};";
                var itemsList = await _dataBase.ExecuteReaderAsync(GetParametersForCostPrice, commandText);
                if (itemsList.Count == itemsLength + 1)
                {
                    offset += itemsLength;
                    itemsList.RemoveAt(itemsLength);
                }
                else
                    isEndOfItems = true;

                foreach (var parameters in itemsList)
                {
                    var consumptionRate = parameters.Item1;
                    var minPrice = parameters.Item2;
                    var maxPrice = parameters.Item3;
                    MinCostPrice += minPrice * consumptionRate;
                    MaxCostPrice += maxPrice * consumptionRate;
                }
                if (isEndOfItems) break;
            }

            MinCostPrice = Math.Round(MinCostPrice, 2);
            MaxCostPrice = Math.Round(MaxCostPrice, 2);
        }

        private (decimal, decimal, decimal) GetParametersForCostPrice(DbDataRecord item)
        {
            var consumptionRate = (decimal) item["QuantityCI"];
            var minPrice = item["minprice"] is DBNull ? 0m : (decimal)item["minprice"];
            var maxPrice = item["maxprice"] is DBNull ? 0m : (decimal)item["maxprice"];

            if (IsCostPriceFull && (item["minprice"] is DBNull || item["maxprice"] is DBNull))
                IsCostPriceFull = false;

            return (consumptionRate, minPrice, maxPrice);
        }

        public async Task CreateOrder()
        {

        }
    }
}
