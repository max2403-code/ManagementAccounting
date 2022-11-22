using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface ISystemOrderOperations
    {
        Task Insert(IOrder order);
        Task Remove(IOrder order);
        Task Edit(IOrder order, IOrder newOrder);
        Task Default(IOrder order, IOrder previousOrder);
    }
}
