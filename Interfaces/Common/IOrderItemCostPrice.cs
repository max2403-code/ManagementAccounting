using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IOrderItemCostPrice
    {
        Task<decimal> GetOrderItemCostPrice(IOrderItem orderItem);
    }
}
