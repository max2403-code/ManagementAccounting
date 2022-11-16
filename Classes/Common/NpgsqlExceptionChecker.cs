using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting.Classes.Common
{
    public class NpgsqlExceptionChecker : IExceptionChecker
    {
        public bool IsExceptionHappened { get; set; }
        public void DoException(string message)
        {
            throw new NpgsqlException(message);
        }
    }
}
