using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IOrderItemOperations
    {
        Task AddReceiving(IOrderItem orderItem);
        Task RemoveReceiving(IOrderItem orderItem);
    }
}
