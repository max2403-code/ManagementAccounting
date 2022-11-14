using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting
{
    public interface IDataBase 
    {
        Task<List<T>> ExecuteReaderAsync<T>(Func<DbDataRecord, T> getItem, string commandText);
        Task ExecuteNonQueryAndReaderAsync(IExceptionChecker exceptionChecker, string commandText, Action<NpgsqlCommand> assignItemParameters = null, Action<DbDataRecord> assignId = null);
        Task SignInAsync(IExceptionChecker exceptionChecker, string login, string password);


        //public Task ExecuteIdReaderAsync(IExceptionable item, Action<DbDataRecord> assignId, string commandText, string textMessage);
        //Task<decimal> GetSum(string commandText);

        ////Task<List<IBlockItem>> ExecuteReaderAsync(IProgramBlock block, string commandText);
        //Task<List<IBlockItem>> ExecuteReaderAsync(Func<DbDataRecord, IBlockItem> getItem, string commandText);
        ////Task ExecuteNonQueryAndReaderAsync(IBlockItem item, string commandText, string exceptionMessage, bool hasParameters, string typeOfCommand = null);
        //Task ExecuteNonQueryAndReaderAsync(IBlockItem item, string commandText, string exceptionMessage, Action<NpgsqlCommand> assignItemParameters = null);
        //Task SignIn(string login, string password);
    }
}
