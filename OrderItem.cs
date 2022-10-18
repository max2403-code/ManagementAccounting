//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Npgsql;

//namespace ManagementAccounting
//{
//    public class OrderItem : IOrderItem
//    {
//        public event Action ExceptionEvent;
//        public string Name { get; }
//        public int Index { get; }
//        public IMaterial Material { get; }
//        public IOrder Order { get; }
//        public decimal MaterialСonsumption { get; private set; }
//        public string ItemTypeName { get; }
//        public int LengthOfItemsList { get; }

//        private readonly IDataBase _dataBase;
//        private readonly IItemsFactory _itemFactory;

       

//        public OrderItem(IOrder order, IMaterial material, decimal materialСonsumption, int index, IDataBase dataBase, IItemsFactory itemsFactory)
//        {
//            ItemTypeName = "ordermaterialreceiving";
//            Order = order;
//            Index = index;
//            MaterialСonsumption = materialСonsumption;
//            Material = material;
//            _dataBase = dataBase;
//            _itemFactory = itemsFactory;
//            LengthOfItemsList = 5;
//        }

        

//        public IBlockItem GetNewBlockItem(params object[] parameters)
//        {
//            var orderItem = this;
//            var materialReceiving = (IMaterialReceiving)parameters[0];
//            var materialConsumption = (decimal) parameters[1];
//            return _itemFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, materialConsumption);
//        }

        
//        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
//        {
//            var commandText = $"SELECT * FROM ordermaterialreceiving, materialreceiving WHERE ordermaterialreceiving.OrderItemIdOMR = {Index} AND ordermaterialreceiving.MaterialReceivingIdOMR = materialreceiving.IdMR LIMIT {LengthOfItemsList + 1} OFFSET {offset};";
//            var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

//            return itemsList;
//        }

//        public IBlockItem GetItemFromDataBase(DbDataRecord item)
//        {
//            var orderItem = this;
//            var date = (DateTime)item["ReceiveDatemr"];
//            var quantity = (decimal)item["Quantitymr"];
//            var cost = (decimal)item["TotalCostmr"];
//            var remainder = (decimal)item["Remaindermr"];
//            var note = (string)item["Notemr"];
//            var materialIndex = (int)item["MaterialIdmr"];
//            var index = (int)item["Idmr"];
//            var materialReceiving = _itemFactory.CreateMaterialReceiving(date, quantity, cost, remainder, note, materialIndex, index);
//            var materialConsumption = (decimal)item["QuantityOMR"];

//            return _itemFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, materialConsumption);
//        }

      
//    }
//}
