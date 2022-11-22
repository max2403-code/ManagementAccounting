using System;
using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IFromPreOrderToOrderConverter
    {
        Task<IOrder> Convert(IPreOrder preOrder, DateTime creationDate);
        Task CreateOrderItems(IOrder order, IPreOrder preOrder);
        Task RemoveOrder(IOrder order);
    }
}
