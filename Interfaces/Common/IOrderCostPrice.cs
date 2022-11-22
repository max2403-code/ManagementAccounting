using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IOrderCostPrice
    {
        Task<decimal[]> GetOrderCostPrice(IOrder order);
    }
}
