﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting
{
    public interface IDataBase
    {
        Task<List<T>> ExecuteReaderAsync<T>(Func<DbDataRecord, T> getItem, string commandText);
        Task ExecuteNonQueryAndReaderAsync(IExceptionable item, string commandText, string exceptionMessage, Action<NpgsqlCommand> assignItemParameters = null, Action<DbDataRecord> assignId = null);

        //public Task ExecuteIdReaderAsync(IExceptionable item, Action<DbDataRecord> assignId, string commandText, string textMessage);
        Task<decimal> GetSum(string commandText);
        Task SignIn(string login, string password);

        ////Task<List<IBlockItem>> ExecuteReaderAsync(IProgramBlock block, string commandText);
        //Task<List<IBlockItem>> ExecuteReaderAsync(Func<DbDataRecord, IBlockItem> getItem, string commandText);
        ////Task ExecuteNonQueryAndReaderAsync(IBlockItem item, string commandText, string exceptionMessage, bool hasParameters, string typeOfCommand = null);
        //Task ExecuteNonQueryAndReaderAsync(IBlockItem item, string commandText, string exceptionMessage, Action<NpgsqlCommand> assignItemParameters = null);
        //Task SignIn(string login, string password);
    }
}