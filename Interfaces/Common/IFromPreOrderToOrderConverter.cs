using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IFromPreOrderToOrderConverter
    {
        Task<IOrder> Convert(IPreOrder preOrder, DateTime creationDate);
        Task CreateOrderItems(IOrder order, IPreOrder preOrder);
        Task RemoveOrder(IOrder order);
    }
}
