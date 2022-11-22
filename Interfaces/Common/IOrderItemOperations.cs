using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IOrderItemOperations
    {
        Task AddReceiving(IOrderItem orderItem);
        Task RemoveReceiving(IOrderItem orderItem);
    }
}
