using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class OrderCostPrice : IOrderCostPrice
    {
        private ICreatorFactory creatorFactory { get; }
        private IOrderItemCostPrice orderItemCostPrice { get; }

        public OrderCostPrice(ICreatorFactory creatorFactory, IOrderItemCostPrice orderItemCostPrice)
        {
            this.creatorFactory = creatorFactory;
            this.orderItemCostPrice = orderItemCostPrice;
        }

        public async Task<decimal> GetOrderCostPrice(IOrder order)
        {
            var orderCostPrice = 0m;
            var orderItemCreator =
                creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            var offset = 0;

            while (true)
            {
                var resultOfGettingItemsList = await orderItemCreator.GetItemsList(offset, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;

                offset += orderItemCreator.LengthOfItemsList;

                foreach (var orderItem in itemsList.Cast<IOrderItem>())
                {
                    orderCostPrice += await orderItemCostPrice.GetOrderItemCostPrice(orderItem);
                }
                if (!isThereMoreOfItems) break;
            }

            return orderCostPrice;
        }
    }
}
