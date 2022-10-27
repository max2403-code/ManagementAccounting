using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;
using DateTime = System.DateTime;

namespace ManagementAccounting.Classes.SystemCreators
{
    public class OrderItemCollectionCreatorWithConsumption : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }
        private IMaterial material { get; }
        public OrderItemCollectionCreatorWithConsumption(IMaterial material, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.material = material;
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
            return itemsFactory.CreateOrderItem(order, material, consumption, totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT oi.*, o.* FROM orderitems AS oi, orders AS o WHERE oi.ConsumptionOI > 0 AND oi.MaterialIdOI = {material.Index} AND oi.OrderIdOI = o.IdO ORDER BY o.CreationDateO, oi.IdOI OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
