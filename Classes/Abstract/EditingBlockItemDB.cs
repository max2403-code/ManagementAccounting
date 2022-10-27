using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting.Classes.Abstract
{
    public abstract class EditingBlockItemDB : BlockItemDB, IEditable
    {
        public new event Action ExceptionEvent;
        private IDataBase dataBase { get; }


        protected EditingBlockItemDB(IDataBase dataBase) : base(dataBase)
        {
            this.dataBase = dataBase;
        }
        public async Task EditItemInDataBase<T>(params object[] parameters) 
        {
            var copyItem = GetCopyItem<T>();
            var commandText = GetEditItemCommandText();
            UpdateItem(parameters);

            if (ExceptionEvent != null) ExceptionEvent = null;
            await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, GetEditExceptionMessage(), AssignParametersToEditCommand);

            if (ExceptionEvent != null)
            {
                UndoValues(copyItem);
                ExceptionEvent.Invoke();
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
