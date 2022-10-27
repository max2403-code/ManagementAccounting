using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderItemCollectionCreatorFromMaterialReceiving : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }
        private IMaterialReceiving materialReceiving { get; }
        public OrderItemCollectionCreatorFromMaterialReceiving(IMaterialReceiving materialReceiving, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.materialReceiving = materialReceiving;
            this.itemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var shortName = (string)item["OrderNameO"];
            var quantity = (int)item["QuantityO"];
            var creationDate = (DateTime)item["CreationDateO"];
            var orderIndex = (int)item["IdO"];

            var order = itemsFactory.CreateOrder(shortName, creationDate, quantity, orderIndex);
            var consumption = (decimal)item["ConsumptionOI"];
            var totalConsumption = (decimal)item["TotalConsumptionOI"];
            var index = (int)item["IdOI"];
            return itemsFactory.CreateOrderItem(order, materialReceiving.Material, consumption, totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion) // > >=
        {
            return $"SELECT DISTINCT oi.*, o.* FROM orderitems AS oi, orders AS o, materialreceiving AS mr, ordermaterialreceiving AS omr WHERE mr.ReceiveDateMR {searchCriterion} '{materialReceiving.Date.ToString("dd/MM/yyyy")}' AND mr.MaterialIdMR = {materialReceiving.Material.Index} AND omr.MaterialReceivingIdOMR = mr.IdMR AND omr.OrderItemIdOMR = oi.IdOI AND oi.OrderIdOI = o.IdO ORDER BY o.CreationDateO, oi.IdOI OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
