using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting.Classes.Abstract
{
    public abstract class BlockItemDB : IAddable, IRemovable, IExceptionable
    {
        public event Action ExceptionEvent;
        //public string Name { get; private protected set; }
        //public int Index { get; private protected set; }
        private IDataBase dataBase { get; }

        protected BlockItemDB(IDataBase dataBase)
        {
            //Index = index;
            //Name = name;
            this.dataBase = dataBase;
        }

        public async Task AddItemToDataBase()
        {
            var commandText = GetAddItemCommandText();
            if (ExceptionEvent != null) ExceptionEvent = null;
            await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, GetAddExceptionMessage(), AssignParametersToAddCommand, AssignIndex);
            ExceptionEvent?.Invoke();
        }

        #region AddItem

        private protected abstract string GetAddItemCommandText();
        private protected abstract string GetAddExceptionMessage();
        private protected abstract void AssignParametersToAddCommand(NpgsqlCommand cmd);
        private protected abstract void AssignIndex(DbDataRecord index);

        #endregion

        //public async Task EditItemInDataBase(params object[] parameters)
        //{
        //    var copyItem = GetCopyItem();
        //    var commandText = GetEditItemCommandText();
        //    UpdateItem(parameters);

        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, GetEditExceptionMessage(), AssignParametersToEditCommand);

        //    if (ExceptionEvent != null)
        //    {
        //        UndoValues(copyItem);
        //        ExceptionEvent.Invoke();
        //    }
        //}

        //#region EditItem

        //private protected abstract IBlockItem GetCopyItem();
        //private protected abstract string GetEditItemCommandText();
        //private protected abstract void UpdateItem(object[] parameters);
        //private protected abstract string GetEditExceptionMessage();
        //private protected abstract void AssignParametersToEditCommand(NpgsqlCommand cmd);
        //private protected abstract void UndoValues(IBlockItem copyItem);

        //#endregion

        public async Task RemoveItemFromDataBase()
        {
            var commandText = GetRemoveItemCommandText();
            if (ExceptionEvent != null) ExceptionEvent = null;
            await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, GetRemoveExceptionMessage());
            ExceptionEvent?.Invoke();
        }

        #region Remove

        private protected abstract string GetRemoveItemCommandText();
        private protected abstract string GetRemoveExceptionMessage();

        #endregion
    }
}
