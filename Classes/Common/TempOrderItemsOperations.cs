using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.Common
{
    public class TempOrderItemsOperations : ITempOrderItemsOperations, IExceptionable
    {
        private IDataBase dataBase { get; }
        public event Action ExceptionEvent;


        public TempOrderItemsOperations(IDataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public async Task Insert(IOrderItem orderItem)
        {
            var cmdText = $"INSERT INTO temporderitems (IdTOI) VALUES ({orderItem.Index})";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await dataBase.ExecuteNonQueryAndReaderAsync(this, cmdText, "Проблема с БД");
            ExceptionEvent?.Invoke();
        }

        public async Task RemoveAll()
        {
            var commandText = "DELETE FROM temporderitems;";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД");
            ExceptionEvent?.Invoke();
        }

    }
}
