using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class PreOrderItemCollectionCreator : BlockItemsCollectionCreator
    {
        private IPreOrder preOrder { get; }
        private IItemsFactory itemsFactory { get; }
        
        public PreOrderItemCollectionCreator(IPreOrder preOrder, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.itemsFactory = itemsFactory;
            this.preOrder = preOrder;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var index = (int)item["IdM"];
            var material = itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, index);
            var materialUnitConsumption = (decimal)item["ConsumptionCI"];
            var isRemainderNotAvailable = item["minprice"] is DBNull || item["maxprice"] is DBNull;

            var minPrice = isRemainderNotAvailable ? 0m : (decimal)item["minprice"];
            var maxPrice = isRemainderNotAvailable ? 0m : (decimal)item["maxprice"];

            return itemsFactory.CreatePreOrderItem(material, materialUnitConsumption, minPrice, maxPrice, preOrder.Quantity, isRemainderNotAvailable);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT m.*, ci.ConsumptionCI, (SELECT MIN(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS minprice, (SELECT MAX(TotalCostMR / QuantityMR) FROM materialreceiving AS mr WHERE mr.MaterialIdMR = ci.MaterialIdCI AND mr.RemainderMR > 0) AS maxprice FROM calculationitems AS ci, materials AS m WHERE ci.CalculationIdCI = {preOrder.Calculation.Index} AND ci.MaterialIdCI = m.IdM ORDER BY m.MaterialTypeM, m.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
