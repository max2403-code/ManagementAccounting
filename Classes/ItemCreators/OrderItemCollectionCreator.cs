using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderItemCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory ItemsFactory { get; }
        private IOrder Order { get; }

        public OrderItemCollectionCreator(IOrder order, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(lengthOfItemsList, dataBase, exChecker)
        {
            Order = order;
            ItemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var indexMaterial = (int)item["IdM"];
            var material = ItemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, indexMaterial);
            var consumption = (decimal)item["ConsumptionOI"];
            var totalConsumption = (decimal)item["TotalConsumptionOI"];
            var index = (int)item["IdOI"];

            return ItemsFactory.CreateOrderItem(Order, material, consumption,totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM orderitems AS oi, materials AS m WHERE oi.OrderIdOI = {Order.Index} AND oi.MaterialIdOI = m.IdM ORDER BY m.MaterialTypeM, m.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
