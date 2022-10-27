using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface ISystemOrderItemOperations
    {
        Task Insert(IOrderItem orderItem);
        Task Remove(IOrderItem orderItem);
        Task Edit(IOrderItem orderItem, IOrderItem newOrderItem);
        Task Default(IOrderItem orderItem, IOrderItem newOrderItem);

    }
}
