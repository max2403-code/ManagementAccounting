using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting
{
    public interface IDataBase 
    {
        Task<List<T>> ExecuteReaderAsync<T>(IExceptionChecker exceptionChecker, Func<DbDataRecord, T> getItem, string commandText);
        Task ExecuteUpdaterAsync(IExceptionChecker exceptionChecker, Action<DbDataRecord> updateItem, string commandText);
        Task ExecuteNonQueryAndReaderAsync(IExceptionChecker exceptionChecker, string commandText, Action<NpgsqlCommand> assignItemParameters = null, Action<DbDataRecord> assignId = null);
        Task SignInAsync(IExceptionChecker exceptionChecker, string login, string password);
    }
}
