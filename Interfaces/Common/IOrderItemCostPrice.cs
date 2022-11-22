using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IOrderItemCostPrice
    {
        Task<decimal[]> GetOrderItemCostPrice(IOrderItem orderItem);
    }
}
