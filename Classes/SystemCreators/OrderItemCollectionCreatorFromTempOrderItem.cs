using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.Common
{
    public class OrderItemCollectionCreatorFromTempOrderItem : BlockItemsCollectionCreator
    {
        private IItemsFactory ItemsFactory { get; }

        public OrderItemCollectionCreatorFromTempOrderItem(int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            ItemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var shortName = (string)item["OrderNameO"];
            var quantity = (int)item["QuantityO"];
            var creationDate = (DateTime)item["CreationDateO"];
            var orderIndex = (int)item["IdO"];

            var order = ItemsFactory.CreateOrder(shortName, creationDate, quantity, orderIndex);

            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var materialIndex = (int)item["IdM"];

            var material = ItemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, materialIndex);
            var consumption = (decimal)item["ConsumptionOI"];
            var totalConsumption = (decimal)item["TotalConsumptionOI"];
            var index = (int)item["IdOI"];
            return ItemsFactory.CreateOrderItem(order, material, consumption, totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM toi AS temporderitems, oi AS orderitems, o AS orders, m AS materials WHERE toi.IdTOI = oi.IdOI AND oi.MaterialIdOI = m.IdM AND oi.OrderIdOI = o.IdO LIMIT {LengthOfItemsList + 1} OFFSET {offset};";
        }
    }
}
