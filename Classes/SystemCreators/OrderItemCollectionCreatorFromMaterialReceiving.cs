using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderItemCollectionCreatorFromMaterialReceiving : BlockItemsCollectionCreator
    {
        private IItemsFactory ItemsFactory { get; }
        private IMaterialReceiving MaterialReceiving { get; }
        public OrderItemCollectionCreatorFromMaterialReceiving(IMaterialReceiving materialReceiving, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            MaterialReceiving = materialReceiving;
            ItemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var shortName = (string)item["OrderNameO"];
            var quantity = (int)item["QuantityO"];
            var creationDate = (DateTime)item["CreationDateO"];
            var orderIndex = (int)item["IdO"];

            var order = ItemsFactory.CreateOrder(shortName, creationDate, quantity, orderIndex);
            var consumption = (decimal)item["ConsumptionOI"];
            var totalConsumption = (decimal)item["TotalConsumptionOI"];
            var index = (int)item["IdOI"];
            return ItemsFactory.CreateOrderItem(order, MaterialReceiving.Material, consumption, totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion) // > >=
        {
            return $"SELECT DISTINCT oi.*, o.* FROM orderitems AS oi, orders AS o, materialreceiving AS mr, ordermaterialreceiving AS omr WHERE mr.ReceiveDateMR {searchCriterion} '{MaterialReceiving.Date.ToString("dd/MM/yyyy")}' AND mr.MaterialIdMR = {MaterialReceiving.Material.Index} AND omr.MaterialReceivingIdOMR = mr.IdMR AND omr.OrderItemIdOMR = oi.IdOI AND oi.OrderIdOI = o.IdO ORDER BY o.CreationDateO, oi.IdOI OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
