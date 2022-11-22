using System.Linq;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class OrderCostPrice : IOrderCostPrice
    {
        private ICreatorFactory CreatorFactory { get; }
        private IOrderItemCostPrice OrderItemCostPrice { get; }

        public OrderCostPrice(ICreatorFactory creatorFactory, IOrderItemCostPrice orderItemCostPrice)
        {
            CreatorFactory = creatorFactory;
            OrderItemCostPrice = orderItemCostPrice;
        }

        public async Task<decimal[]> GetOrderCostPrice(IOrder order)
        {
            var orderCostPrice = 0m;
            var orderItemCreator =
                CreatorFactory.CreateOrderItemCollectionCreator(order, 5);
            var offset = 0;
            var prices = new decimal[2];

            while (true)
            {
                var resultOfGettingItemsList = await orderItemCreator.GetItemsList(offset, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;

                offset += orderItemCreator.LengthOfItemsList;

                foreach (var orderItem in itemsList.Cast<IOrderItem>())
                {
                    var orderItemCostPrices = await OrderItemCostPrice.GetOrderItemCostPrice(orderItem);
                    orderCostPrice += orderItemCostPrices[2];
                }
                if (!isThereMoreOfItems) break;
            }

            prices[0] = orderCostPrice / order.Quantity;
            prices[1] = orderCostPrice;

            return prices;
        }
    }
}
