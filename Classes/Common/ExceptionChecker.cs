using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.Common
{
    public class ExceptionChecker : IExceptionChecker
    {
        public bool IsExceptionHappened { get; set; }
        public void DoException(string message)
        {
            throw new Exception(message);
        }
    }
}
