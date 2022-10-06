using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting
{
    public class Order : IOrder
    {
        public event Action ExceptionEvent;
        private DateTime _creationDate;
        private decimal _quantity;
        private ICalculation _calculation;
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _itemsFactory;

        public string Name => _calculation.Name;
        public int Index { get; }
        public string ItemTypeName { get; }
        public DateTime CreationDate => _creationDate;
        public decimal Quantity => _quantity;
        public int LengthOfItemsList { get; }

        public Order(ICalculation calculation, DateTime creationDate, decimal quantity, int index,  IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            ItemTypeName = "orderitem";
            _calculation = calculation;
            _creationDate = creationDate;
            _quantity = quantity;
            Index = index;
            _dataBase = dataBase;
            _itemsFactory = itemsFactory;
            LengthOfItemsList = 5;
        }

        public IBlockItem GetNewBlockItem(params object[] parameters)
        {
            throw new NotImplementedException(); //
        }

        public Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            throw new NotImplementedException();
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            throw new NotImplementedException();
        }

        
        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO orders (CreationDate, CalculationId, Quantity) VALUES (@CreationDate, @CalculationId, @Quantity)";
            await GetCalculationItems(CheckMaterials);





            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "", AssignParametersToAddCommand);

            ExceptionEvent?.Invoke();
        }

        public Task EditItemInDataBase(params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task RemoveItemFromDataBase()
        {
            throw new NotImplementedException();
        }

        private async Task GetCalculationItems(Func<ICalculationItem, Task> func)
        {
            var offset = 0;
            var itemsLength = 5;

            while (true)
            {
                var isEndOfItems = false;
                var commandText = $"SELECT * FROM calculationitems, remainders WHERE calculationitems.CalculationId = {_calculation.Index} AND calculationitems.MaterialId = remainders.Id LIMIT {itemsLength + 1} OFFSET {offset};";
                var itemsList = await _dataBase.ExecuteReaderAsync(GetCalculationItemFromDataBase, commandText);
                if (itemsList.Count == itemsLength + 1)
                {
                    offset += itemsLength;
                    itemsList.RemoveAt(itemsLength);
                }
                else
                    isEndOfItems = true;

                foreach (var calculationItem in itemsList)
                {
                    await func(calculationItem);
                }
                if(isEndOfItems) break;
            }

        }

        private ICalculationItem GetCalculationItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["remainders.MaterialType"];
            var materialName = (string)item["remainders.MaterialName"];
            var materialIndex = (int)item["remainders.Id"];
            var material = _itemsFactory.CreateMaterial(materialType, materialName, materialIndex);
            var quantity = (decimal)item["calculationitems.Quantity"];
            var calculationId = (int)item["calculationitems.CalculationId"];
            var calculationItemIndex = (int)item["calculationitems.Id"];

            return _itemsFactory.CreateCalculationItem(material, quantity, calculationId, calculationItemIndex);
        }

        
        private async Task CheckMaterials(ICalculationItem calculationItem)
        {
            var commandText =
                $"SELECT SUM(Remainder) FROM materialreceiving WHERE MaterialId = {calculationItem.MaterialId};";
            var maxRemainder = await _dataBase.GetSum(commandText);
            var consumption = Math.Round(calculationItem.Quantity * Quantity, 2);
            if (consumption > maxRemainder) throw new Exception();
        }

        private async Task ReserveOrdersMaterials(ICalculationItem calculationItem)
        {
            var consumption = Math.Round(calculationItem.Quantity * Quantity, 2);

        }

        //private async Task CheckMaterials()
        //{
        //    var offset = 0;
        //    var itemsLength = 5;

        //    while (true)
        //    {
        //        var isEndOfItems = false;
        //        var commandText = $"SELECT * FROM calculationitems, remainders WHERE calculationitems.CalculationId = {_calculation.Index} AND calculationitems.MaterialId = remainders.Id LIMIT {itemsLength + 1} OFFSET {offset};";
        //        var itemsList = await _dataBase.ExecuteReaderAsync(GetCalculationItemFromDataBase, commandText);
        //        if (itemsList.Count == itemsLength + 1)
        //        {
        //            offset += itemsLength;
        //            itemsList.RemoveAt(itemsLength);
        //        }
        //        else
        //            isEndOfItems = true;

        //        foreach (var calculationItem in itemsList)
        //        {
        //            commandText =
        //                $"SELECT SUM(Remainder) FROM materialreceiving WHERE MaterialId = {calculationItem.MaterialId};";
        //            var maxRemainder = (await _dataBase.ExecuteReaderAsync(GetSumOfMaterialRemainderFromDataBase, commandText)).First();
        //            var consumption = Math.Round(calculationItem.Quantity * Quantity, 2);
        //            if (consumption > maxRemainder) throw new Exception();

        //        }
        //        if (isEndOfItems) break;
        //    }

        //}
    }
}
