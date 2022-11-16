using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.ItemCreators;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class CalculationItemCollectionCreator : BlockItemsCollectionCreator
    {
        private ICalculation Calculation { get; }
        private IItemsFactory ItemsFactory { get; }

        public CalculationItemCollectionCreator(ICalculation calculation, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(lengthOfItemsList, dataBase, exChecker)
        {
            ItemsFactory = itemsFactory;
            Calculation = calculation;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var materialIndex = (int)item["IdM"];
            var material = ItemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, materialIndex);
            var consumption = (decimal)item["ConsumptionCI"];
            var calculationId = (int)item["CalculationIdci"];
            var index = (int)item["Idci"];

            return ItemsFactory.CreateCalculationItem(material, consumption, calculationId, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM calculationitems, materials WHERE calculationitems.CalculationIdci = {Calculation.Index} AND calculationitems.MaterialIdci = materials.Idm ORDER BY materials.MaterialTypem, materials.MaterialNamem OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
