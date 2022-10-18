using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingNotEmptyCollectionCreator : MaterialReceivingCollectionCreator
    {
        private BlockItemDB material { get; }
        public MaterialReceivingNotEmptyCollectionCreator(BlockItemDB material, IDataBase dataBase, IItemsFactory itemsFactory) : base(material, dataBase, itemsFactory)
        {
            this.material = material;
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving AS mr, materials AS m WHERE mr.MaterialIdmr = {material.Index} AND mr.Remaindermr > 0 AND mr.SearchNamemr LIKE '%{searchCriterion}%' AND mr.MaterialIdmr = m.IdM ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}