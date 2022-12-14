using System.Linq;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class OrderItemCostPrice : IOrderItemCostPrice
    {
        private ICreatorFactory CreatorFactory { get; }
        public OrderItemCostPrice(ICreatorFactory creatorFactory)
        {
            CreatorFactory = creatorFactory;
        }
        public async Task<decimal[]> GetOrderItemCostPrice(IOrderItem orderItem)
        {
            var orderItemCostPrice = 0m;
            var orderMaterialReceivingCreator =
                CreatorFactory.CreateOrderMaterialReceivingCollectionCreator(orderItem, 5);
            var offset = 0;
            var prices = new decimal[3];

            while (true)
            {
                var resultOfGettingItemsList = await orderMaterialReceivingCreator.GetItemsList(offset, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;
                
                offset += orderMaterialReceivingCreator.LengthOfItemsList;

                foreach (var orderMaterialReceiving in itemsList.Cast<IOrderMaterialReceiving>())
                {
                    var materialReceiving = orderMaterialReceiving.MaterialReceiving;
                    orderItemCostPrice += orderMaterialReceiving.Consumption * materialReceiving.Price;
                }
                if (!isThereMoreOfItems) break;
            }

            prices[0] = orderItemCostPrice / orderItem.TotalConsumption;
            prices[1] = orderItemCostPrice / orderItem.Order.Quantity;
            prices[2] = orderItemCostPrice;


            return prices;
        }
    }
}
