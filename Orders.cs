using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public class Orders : IOrders
    {
        public string ItemTypeName { get; }
        public int LengthOfItemsList { get; }
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _blockItemFactory;

        public Orders(IBlockItemsFactory blockItemFactory, IDataBase dataBase)
        {
            ItemTypeName = "order";
            _dataBase = dataBase;
            _blockItemFactory = blockItemFactory;
            LengthOfItemsList = 5;
        }
        public IBlockItem GetNewBlockItem(params object[] parameters)
        {
            var calculation = (ICalculation) parameters[0];
            var creationDate = (DateTime) parameters[1];
            var quantity = (decimal) parameters[2];

            return _blockItemFactory.CreateOrder(calculation, creationDate, quantity);
        }

        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM orders, calculations WHERE orders.CalculationId = calculations.Id AND lower(calculations.CalculationName) LIKE '%{selectionCriterion[0]}%' ORDER BY calculations.CalculationName OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var calculationName = (string)item["CalculationName"];
            var calculationId = (int)item["Id"];
            var calculation = _blockItemFactory.CreateCalculation(calculationName, calculationId);

            var creationDate = (DateTime)item["CreationDate"];
            var quantity = (decimal)item["Quantity"];
            var index = (int) item["Id"];

            return _blockItemFactory.CreateOrder(calculation, creationDate, quantity, index);
        }

        public async Task<List<ICalculation>> GetCalculationsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM calculations WHERE lower(CalculationName) LIKE '%{selectionCriterion[0]}%' ORDER BY CalculationName OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetCalculationsFromDataBase, commandText);

            return itemsList;
        }

        private ICalculation GetCalculationsFromDataBase(DbDataRecord item)
        {
            var calculationName = (string)item["CalculationName"];
            var calculationId = (int)item["Id"];

            return _blockItemFactory.CreateCalculation(calculationName, calculationId);
        }
    }
}
