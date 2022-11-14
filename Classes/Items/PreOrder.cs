using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Items;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class PreOrder : EditingBlockItemDB, IPreOrder 
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public DateTime CreationDate { get; private set; }
        public ICalculation Calculation { get; }
        public int Quantity { get; private set; }
        private IItemsFactory ItemsFactory { get; }

        public PreOrder(ICalculation calculation, int quantity, DateTime creationDate, int index, IItemsFactory itemsFactory, IDataBase dataBase, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Index = index;
            Name = string.Join(' ', calculation.Name, "от", creationDate.ToString("dd/MM/yyyy"));
            Quantity = quantity;
            Calculation = calculation;
            CreationDate = creationDate;
            Index = index;
            ItemsFactory = itemsFactory;
        }

        //public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        //{
        //    cmd.Parameters.AddWithValue("CalculationIdPO", NpgsqlDbType.Integer, Calculation.Index);
        //    cmd.Parameters.AddWithValue("QuantityPO", NpgsqlDbType.Numeric, Quantity);
        //    cmd.Parameters.AddWithValue("CreationDatePO", NpgsqlDbType.Date, CreationDate);
        //    cmd.Parameters.AddWithValue("SearchNamePO", NpgsqlDbType.Varchar, 64, Name);
        //}

        //public async Task AddItemToDataBase()
        //{
        //    var commandText = "INSERT INTO preorders (CalculationIdPO, QuantityPO, CreationDatePO, SearchNamePO) VALUES (@CalculationIdPO, @QuantityPO, @CreationDatePO, @SearchNamePO)";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД", AssignParametersToAddCommand);

        //    ExceptionEvent?.Invoke();
        //}

        //public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        //{
        //    cmd.Parameters.AddWithValue("QuantityPO", NpgsqlDbType.Numeric, Quantity);
        //    cmd.Parameters.AddWithValue("CreationDatePO", NpgsqlDbType.Date, CreationDate);
        //    cmd.Parameters.AddWithValue("SearchNamePO", NpgsqlDbType.Varchar, 64, Name);
        //}

        //public async Task EditItemInDataBase(params object[] parameters)
        //{
        //    var copyPreOrder = _itemsFactory.CreatePreOrder(Calculation, Quantity, CreationDate, Index);
            
        //    var commandText = $"UPDATE preorders SET QuantityPO = @QuantityPO, CreationDatePO = @CreationDatePO, SearchNamePO = @SearchNamePO WHERE idpo = {Index}";

        //    Quantity = (decimal)parameters[0];
        //    CreationDate = (DateTime) parameters[1];
        //    Name = string.Join(' ', Calculation.Name, "от", CreationDate.ToString("dd/MM/yyyy"));

        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД", AssignParametersToEditCommand);

        //    if (ExceptionEvent != null)
        //    {
        //        Quantity = copyPreOrder.Quantity;
        //        CreationDate = copyPreOrder.CreationDate;
        //        Name = copyPreOrder.Name;
        //        ExceptionEvent.Invoke();
        //    }
        //}

        //public async Task RemoveItemFromDataBase()
        //{
        //    var commandText = $"DELETE FROM preorders WHERE idpo = {Index}";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД");
        //    ExceptionEvent?.Invoke();
        //}

        
        //public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        //{
        //    var commandText = $"SELECT r.*, ci.QuantityCI, (SELECT MIN(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS minprice, (SELECT MAX(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS maxprice FROM calculationitems AS ci, remainders AS r WHERE ci.CalculationIdCI = {Calculation.Index} AND ci.MaterialIdCI = r.IdM ORDER BY r.MaterialTypeM, r.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        //    var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

        //    return itemsList;
        //}

        //public IBlockItem GetItemFromDataBase(DbDataRecord item)
        //{
        //    var materialType = (MaterialType)(int)item["MaterialTypeM"];
        //    var materialName = (string)item["MaterialNameM"];
        //    var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
        //    var index = (int)item["IdM"];

        //    var material = _itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, index);
        //    var materialConsumption = (decimal) item["QuantityCI"];
        //    var minPrice = item["minprice"] is DBNull ? -1m : (decimal)item["minprice"];
        //    var maxPrice = item["maxprice"] is DBNull ? -1m : (decimal)item["maxprice"];

        //    return _itemsFactory.CreatePreOrderItem(material, materialConsumption, minPrice, maxPrice);
        //}

        //public async Task AssignCostPrice()
        //{
        //    var offset = 0;
        //    var itemsLength = 50;

        //    while (true)
        //    {
        //        var isEndOfItems = false;
        //        var commandText = $"SELECT ci.QuantityCI, (SELECT MIN(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS minprice, (SELECT MAX(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS maxprice FROM calculationitems AS ci WHERE ci.CalculationIdCI = {Calculation.Index} LIMIT {itemsLength + 1} OFFSET {offset};";
        //        var itemsList = await _dataBase.ExecuteReaderAsync(GetParametersForCostPrice, commandText);
        //        if (itemsList.Count == itemsLength + 1)
        //        {
        //            offset += itemsLength;
        //            itemsList.RemoveAt(itemsLength);
        //        }
        //        else
        //            isEndOfItems = true;

        //        foreach (var parameters in itemsList)
        //        {
        //            var consumptionRate = parameters.Item1;
        //            var minPrice = parameters.Item2;
        //            var maxPrice = parameters.Item3;
        //            MinCostPrice += minPrice * consumptionRate;
        //            MaxCostPrice += maxPrice * consumptionRate;
        //        }
        //        if (isEndOfItems) break;
        //    }

        //    MinCostPrice = Math.Round(MinCostPrice, 2);
        //    MaxCostPrice = Math.Round(MaxCostPrice, 2);
        //}

        //private (decimal, decimal, decimal) GetParametersForCostPrice(DbDataRecord item)
        //{
        //    var consumptionRate = (decimal) item["QuantityCI"];
        //    var minPrice = item["minprice"] is DBNull ? 0m : (decimal)item["minprice"];
        //    var maxPrice = item["maxprice"] is DBNull ? 0m : (decimal)item["maxprice"];

        //    if (IsCostPriceFull && (item["minprice"] is DBNull || item["maxprice"] is DBNull))
        //        IsCostPriceFull = false;

        //    return (consumptionRate, minPrice, maxPrice);
        //}

        //public async Task CreateOrder()
        //{

        //}
        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return "INSERT INTO preorders (CalculationIdPO, QuantityPO, CreationDatePO, SearchNamePO) VALUES (@CalculationIdPO, @QuantityPO, @CreationDatePO, @SearchNamePO) RETURNING IdPO";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CalculationIdPO", NpgsqlDbType.Integer, Calculation.Index);
            AssignParametersToEditCommand(cmd);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdPO"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T) ItemsFactory.CreatePreOrder(Calculation, Quantity, CreationDate, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE preorders SET QuantityPO = @QuantityPO, CreationDatePO = @CreationDatePO, SearchNamePO = @SearchNamePO WHERE idpo = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            Quantity = (int)parameters[0];
            CreationDate = (DateTime)parameters[1];
            Name = string.Join(' ', Calculation.Name, "от", CreationDate.ToString("dd/MM/yyyy"));
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("QuantityPO", NpgsqlDbType.Integer, Quantity);
            cmd.Parameters.AddWithValue("CreationDatePO", NpgsqlDbType.Date, CreationDate);
            cmd.Parameters.AddWithValue("SearchNamePO", NpgsqlDbType.Varchar, 64, Name);
        }

        private protected override void UndoValues<T>(T copyItem)
        {
            var copyPreOrder = copyItem as IPreOrder;
            Quantity = copyPreOrder.Quantity;
            CreationDate = copyPreOrder.CreationDate;
            Name = copyPreOrder.Name;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM preorders WHERE idpo = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблема с БД";
        }
    }
}
