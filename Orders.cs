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
        

        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM orders WHERE lower(OrderNameO) LIKE '%{selectionCriterion[0]}%' ORDER BY OrderNameO OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var orderName = (string)item["CalculationName"];
            var creationDate = (DateTime)item["CreationDate"];
            var quantity = (decimal)item["Quantity"];
            var index = (int) item["Id"];

            return _blockItemFactory.CreateOrder(orderName, creationDate, quantity, index);
        }
    }
}
