using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderItemCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }
        private IOrder order { get; }

        public OrderItemCollectionCreator(IOrder order, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.order = order;
            this.itemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var indexMaterial = (int)item["IdM"];

            var material = itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, indexMaterial);
            var consumption = (decimal)item["ConsumptionOI"];
            var totalConsumption = (decimal)item["TotalConsumptionOI"];
            var index = (int)item["IdOI"];

            return itemsFactory.CreateOrderItem(order, material, consumption,totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM orderitems AS oi, materials AS m WHERE oi.OrderIdOI = {order.Index} AND oi.MaterialIdOI = m.IdM ORDER BY m.MaterialTypeM, m.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
