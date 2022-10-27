using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderMaterialReceivingCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }
        private IOrderItem orderItem { get; }
        //private IMaterial material { get; }


        public OrderMaterialReceivingCollectionCreator(IOrderItem orderItem, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.orderItem = orderItem;
            //this.material = material;

            this.itemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var date = (DateTime)item["ReceiveDatemr"];
            var quantity = (decimal)item["Quantitymr"];
            var cost = (decimal)item["TotalCostmr"];
            var remainder = (decimal)item["Remaindermr"];
            var note = (string)item["Notemr"];
            var indexMaterialReceiving = (int)item["Idmr"];

            var materialReceiving = itemsFactory.CreateMaterialReceiving(orderItem.Material, date, quantity, cost, remainder, note, indexMaterialReceiving);
            var consumption = (decimal)item["ConsumptionOMR"];
            var index = (int)item["IdOMR"];

            return itemsFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, consumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM ordermaterialreceiving AS omr, materialreceiving AS mr WHERE omr.OrderItemIdOMR = {orderItem.Index} AND omr.MaterialReceivingIdOMR = mr.IdMR LIMIT {LengthOfItemsList + 1} OFFSET {offset}";
        }
    }
}
