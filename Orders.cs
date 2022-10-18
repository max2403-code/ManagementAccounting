//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Text;
//using System.Threading.Tasks;

//namespace ManagementAccounting
//{
//    public class Orders : IOrders
//    {
//        public string ItemTypeName { get; }
//        public int LengthOfItemsList { get; }
//        private readonly IDataBase _dataBase;
//        private readonly IItemsFactory _itemFactory;

//        public Orders(IItemsFactory itemFactory, IDataBase dataBase)
//        {
//            ItemTypeName = "order";
//            _dataBase = dataBase;
//            _itemFactory = itemFactory;
//            LengthOfItemsList = 5;
//        }
        

//        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
//        {
//            var commandText = $"SELECT * FROM orders WHERE lower(SearchNameO) LIKE '%{selectionCriterion[0]}%' ORDER BY SearchNameO OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
//            var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

//            return itemsList;
//        }

//        public IBlockItem GetItemFromDataBase(DbDataRecord item)
//        {
//            var orderName = (string)item["OrderNameO"];
//            var creationDate = (DateTime)item["CreationDateO"];
//            var quantity = (decimal)item["QuantityO"];
//            var index = (int) item["IdO"];

//            return _itemFactory.CreateOrder(orderName, creationDate, quantity, index);
//        }
//    }
//}
