using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingNotEmptyCollectionCreator : MaterialReceivingCollectionCreator
    {
        public MaterialReceivingNotEmptyCollectionCreator(IMaterial material, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(material, lengthOfItemsList, dataBase, itemsFactory, exChecker)
        {
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {Material.Index} AND Remaindermr > 0 AND SearchNamemr LIKE '%{searchCriterion}%' ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}