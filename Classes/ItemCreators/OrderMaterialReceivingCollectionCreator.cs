using System;
using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderMaterialReceivingCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory ItemsFactory { get; }
        private IOrderItem OrderItem { get; }
        //private IMaterial material { get; }


        public OrderMaterialReceivingCollectionCreator(IOrderItem orderItem, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(lengthOfItemsList, dataBase, exChecker)
        {
            OrderItem = orderItem;
            //this.material = material;

            ItemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var date = (DateTime)item["ReceiveDatemr"];
            var quantity = (decimal)item["Quantitymr"];
            var cost = (decimal)item["TotalCostmr"];
            var remainder = (decimal)item["Remaindermr"];
            var note = (string)item["Notemr"];
            var indexMaterialReceiving = (int)item["Idmr"];

            var materialReceiving = ItemsFactory.CreateMaterialReceiving(OrderItem.Material, date, quantity, cost, remainder, note, indexMaterialReceiving);
            var consumption = (decimal)item["ConsumptionOMR"];
            var index = (int)item["IdOMR"];

            return ItemsFactory.CreateOrderMaterialReceiving(OrderItem, materialReceiving, consumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM ordermaterialreceiving AS omr, materialreceiving AS mr WHERE omr.OrderItemIdOMR = {OrderItem.Index} AND omr.MaterialReceivingIdOMR = mr.IdMR LIMIT {LengthOfItemsList + 1} OFFSET {offset}";
        }
    }
}
