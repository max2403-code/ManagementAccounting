using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IPreOrderCostPrice
    {
        Task<(decimal, decimal, bool)> GetPreOrderCostPrice(IPreOrder preOrder);
    }
}
