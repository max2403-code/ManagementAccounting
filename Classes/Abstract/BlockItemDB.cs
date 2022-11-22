using System.Data.Common;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting.Classes.Abstract
{
    public abstract class BlockItemDB
    {
        protected IExceptionChecker ExceptionChecker { get; }
        protected IDataBase DataBase { get; }

        protected BlockItemDB(IDataBase dataBase, IExceptionChecker exceptionChecker)
        {
            DataBase = dataBase;
            ExceptionChecker = exceptionChecker;
        }

        public async Task AddItemToDataBase(bool isPreviouslyExistingItem = false)
        {
            var commandText = GetAddItemCommandText(isPreviouslyExistingItem);
            ExceptionChecker.IsExceptionHappened = false;
            if(isPreviouslyExistingItem)
                await DataBase.ExecuteNonQueryAndReaderAsync(ExceptionChecker, commandText,  AssignParametersToAddCommand);
            else
                await DataBase.ExecuteNonQueryAndReaderAsync(ExceptionChecker, commandText, AssignParametersToAddCommand, AssignIndex);
            if(ExceptionChecker.IsExceptionHappened) ExceptionChecker.DoException(GetAddExceptionMessage());
        }

        #region AddItem

        private protected abstract string GetAddItemCommandText(bool isPreviouslyExistingItem = false);
        private protected abstract string GetAddExceptionMessage();
        private protected abstract void AssignParametersToAddCommand(NpgsqlCommand cmd);
        private protected abstract void AssignIndex(DbDataRecord index);

        #endregion

        public async Task RemoveItemFromDataBase()
        {
            var commandText = GetRemoveItemCommandText();
            ExceptionChecker.IsExceptionHappened = false;
            await DataBase.ExecuteNonQueryAndReaderAsync(ExceptionChecker, commandText);
            if(ExceptionChecker.IsExceptionHappened) ExceptionChecker.DoException(GetRemoveExceptionMessage());
        }

        #region Remove

        private protected abstract string GetRemoveItemCommandText();
        private protected abstract string GetRemoveExceptionMessage();

        #endregion
    }
}
