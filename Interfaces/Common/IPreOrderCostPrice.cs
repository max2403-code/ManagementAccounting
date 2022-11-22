using System;
using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IPreOrderCostPrice
    {
        Task<(decimal[], bool)> GetPreOrderCostPrice(IPreOrder preOrder, DateTime orderDate);
    }
}
