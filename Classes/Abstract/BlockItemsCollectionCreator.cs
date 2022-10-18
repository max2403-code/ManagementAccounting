using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting.Classes.Abstract
{
    public abstract class BlockItemsCollectionCreator
    {
        public int LengthOfItemsList { get; }
        private IDataBase dataBase { get; }

        protected BlockItemsCollectionCreator(int lengthOfItemsList, IDataBase dataBase)
        {
            this.dataBase = dataBase;
            LengthOfItemsList = lengthOfItemsList;
        }

        public async Task<(List<IBlockItem>, bool)> GetItemsList(int offset, string searchCriterion)
        {
            var isThereMoreOfItems = false;
            var commandText = GetCommandText(offset, searchCriterion);
            var itemsList = await dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

            if (itemsList.Count != LengthOfItemsList + 1) return (itemsList, isThereMoreOfItems);
            itemsList.RemoveAt(LengthOfItemsList);
            isThereMoreOfItems = true;

            return (itemsList, isThereMoreOfItems);
        }

        private protected abstract IBlockItem GetItemFromDataBase(DbDataRecord item);
        private protected abstract string GetCommandText(int offset, string searchCriterion);
    }
}
