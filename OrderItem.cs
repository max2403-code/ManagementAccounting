using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace ManagementAccounting
{
    public class OrderItem : IOrderItem
    {
        public event Action ExceptionEvent;
        public string Name { get; }
        public int Index { get; }
        public IMaterial Material { get; }
        public IOrder Order { get; }
        public decimal MaterialСonsumption { get; private set; }
        public string ItemTypeName { get; }
        public int LengthOfItemsList { get; }

        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _blockItemFactory;

       

        public OrderItem(IOrder order, IMaterial material, decimal materialСonsumption, int index, IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            ItemTypeName = "ordermaterialreceiving";
            Order = order;
            Index = index;
            MaterialСonsumption = materialСonsumption;
            Material = material;
            _dataBase = dataBase;
            _blockItemFactory = itemsFactory;
            LengthOfItemsList = 5;
        }

        

        public IBlockItem GetNewBlockItem(params object[] parameters)
        {
            var orderItem = this;
            var materialReceiving = (IMaterialReceiving)parameters[0];
            var materialConsumption = (decimal) parameters[1];
            return _blockItemFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, materialConsumption);
        }

        
        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM ordermaterialreceiving, materialreceiving WHERE ordermaterialreceiving.OrderItemIdOMR = {Index} AND ordermaterialreceiving.MaterialReceivingIdOMR = materialreceiving.IdMR LIMIT {LengthOfItemsList + 1} OFFSET {offset};";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var orderItem = this;
            var date = (DateTime)item["ReceiveDatemr"];
            var quantity = (decimal)item["Quantitymr"];
            var cost = (decimal)item["TotalCostmr"];
            var remainder = (decimal)item["Remaindermr"];
            var note = (string)item["Notemr"];
            var materialIndex = (int)item["MaterialIdmr"];
            var index = (int)item["Idmr"];
            var materialReceiving = _blockItemFactory.CreateMaterialReceiving(date, quantity, cost, remainder, note, materialIndex, index);
            var materialConsumption = (decimal)item["QuantityOMR"];

            return _blockItemFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, materialConsumption);
        }

      
    }
}
