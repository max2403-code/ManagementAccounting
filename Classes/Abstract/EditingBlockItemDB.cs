using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting.Classes.Abstract
{
    public abstract class EditingBlockItemDB : BlockItemDB
    {
        protected EditingBlockItemDB(IDataBase dataBase, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
        }
        public async Task EditItemInDataBase<T>(params object[] parameters) 
        {
            var copyItem = GetCopyItem<T>();
            var commandText = GetEditItemCommandText();
            UpdateItem(parameters);
            ExceptionChecker.IsExceptionHappened = false;

            await DataBase.ExecuteNonQueryAndReaderAsync(ExceptionChecker, commandText, AssignParametersToEditCommand);

            if (ExceptionChecker.IsExceptionHappened)
            {
                UndoValues(copyItem);
                ExceptionChecker.DoException(GetEditExceptionMessage());
            }
        }

        #region EditItem

        private protected abstract T GetCopyItem<T>();
        private protected abstract string GetEditItemCommandText();
        private protected abstract void UpdateItem(object[] parameters);
        private protected abstract string GetEditExceptionMessage();
        private protected abstract void AssignParametersToEditCommand(NpgsqlCommand cmd);
        private protected abstract void UndoValues<T>(T copyItem);

        #endregion
    }
}
