using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting.Classes.Common
{
    public class OrderItemOperationException : Exception
    {
        public OrderItemOperationException(string message) : base(message)
        {

        }
    }
}
