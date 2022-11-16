using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingNotEmptyCollectionCreator : MaterialReceivingCollectionCreator
    {
        //private IMaterial Material { get; }
        public MaterialReceivingNotEmptyCollectionCreator(IMaterial material, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(material, lengthOfItemsList, dataBase, itemsFactory, exChecker)
        {
            //Material = material;
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {Material.Index} AND Remaindermr > 0 AND SearchNamemr LIKE '%{searchCriterion}%' ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}