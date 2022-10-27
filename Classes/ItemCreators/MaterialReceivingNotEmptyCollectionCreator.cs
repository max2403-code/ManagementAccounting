using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingNotEmptyCollectionCreator : MaterialReceivingCollectionCreator
    {
        private IMaterial material { get; }
        public MaterialReceivingNotEmptyCollectionCreator(IMaterial material, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(material, lengthOfItemsList, dataBase, itemsFactory)
        {
            this.material = material;
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {material.Index} AND Remaindermr > 0 AND SearchNamemr LIKE '%{searchCriterion}%' ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}