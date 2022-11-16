using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.Abstract
{
    public abstract class BlockItemsCollectionCreator
    {
        public int LengthOfItemsList { get; }
        private IDataBase DataBase { get; }
        private IExceptionChecker ExChecker { get; }

        protected BlockItemsCollectionCreator(int lengthOfItemsList, IDataBase dataBase, IExceptionChecker exChecker)
        {
            DataBase = dataBase;
            LengthOfItemsList = lengthOfItemsList;
            ExChecker = exChecker;
        }

        public async Task<(List<IBlockItem>, bool)> GetItemsList(int offset, string searchCriterion)
        {
            var isThereMoreOfItems = false;
            var commandText = GetCommandText(offset, searchCriterion);
            ExChecker.IsExceptionHappened = false;
            var itemsList = await DataBase.ExecuteReaderAsync(ExChecker, GetItemFromDataBase, commandText);
            if (ExChecker.IsExceptionHappened)
            {
                ExChecker.DoException("Проблема с БД");
            }

            if (itemsList.Count != LengthOfItemsList + 1) return (itemsList, isThereMoreOfItems);
            itemsList.RemoveAt(LengthOfItemsList);
            isThereMoreOfItems = true;

            return (itemsList, isThereMoreOfItems);
        }

        private protected abstract IBlockItem GetItemFromDataBase(DbDataRecord item);
        private protected abstract string GetCommandText(int offset, string searchCriterion);
    }
}
