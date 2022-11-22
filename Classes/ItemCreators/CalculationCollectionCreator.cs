using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class CalculationCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory ItemsFactory { get; }

        public CalculationCollectionCreator(int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(lengthOfItemsList, dataBase, exChecker)
        {
            ItemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var calculationName = (string)item["CalculationNamec"];
            var index = (int)item["Idc"];
            return ItemsFactory.CreateCalculation(calculationName, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM calculations WHERE lower(CalculationNamec) LIKE '%{searchCriterion}%' ORDER BY CalculationNamec OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
