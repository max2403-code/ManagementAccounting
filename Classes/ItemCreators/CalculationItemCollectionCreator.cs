using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.ItemCreators;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class CalculationItemCollectionCreator : BlockItemsCollectionCreator
    {
        private BlockItemDB calculation { get; }
        private IItemsFactory itemsFactory { get; }

        public CalculationItemCollectionCreator(BlockItemDB calculation, IDataBase dataBase, IItemsFactory itemsFactory) : base(5, dataBase)
        {
            this.itemsFactory = itemsFactory;
            this.calculation = calculation;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var materialIndex = (int)item["IdM"];
            var material = itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, materialIndex);
            var consumption = (decimal)item["ConsumptionCI"];
            var calculationId = (int)item["CalculationIdci"];
            var index = (int)item["Idci"];

            return itemsFactory.CreateCalculationItem(material, consumption, calculationId, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM calculationitems, materials WHERE calculationitems.CalculationIdci = {calculation.Index} AND calculationitems.MaterialIdci = materials.Idm ORDER BY materials.MaterialTypem, materials.MaterialNamem OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
