using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.ItemCreators;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class CalculationCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }

        public CalculationCollectionCreator(IDataBase dataBase, IItemsFactory itemsFactory) : base(5, dataBase)
        {
            this.itemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var calculationName = (string)item["CalculationNamec"];
            var index = (int)item["Idc"];
            return itemsFactory.CreateCalculation(calculationName, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM calculations WHERE lower(CalculationNamec) LIKE '%{searchCriterion}%' ORDER BY CalculationNamec OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
