using System;
using System.Linq;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class PreOrderCostPrice : IPreOrderCostPrice
    {
        private ICreatorFactory CreatorFactory { get; }

        public PreOrderCostPrice(ICreatorFactory creatorFactory)
        {
            CreatorFactory = creatorFactory;
        }

        public async Task<(decimal[], bool)> GetPreOrderCostPrice(IPreOrder preOrder, DateTime orderDate)
        {
            var preOrderItemCreator = CreatorFactory.CreatePreOrderItemCollectionCreator(preOrder, 5);
            var offset = 0;
            var minUnitCostPrice = 0m;
            var maxUnitCostPrice = 0m;
            var isPriceCostAvailable = true;
            var quantity = preOrder.Quantity;
            var prices = new decimal[4];

            while (true)
            {
                var resultOfGettingItemsList = await preOrderItemCreator.GetItemsList(offset, orderDate.ToString("dd/MM/yyyy"));
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;
                offset += preOrderItemCreator.LengthOfItemsList;

                foreach (var preOrderItem in itemsList.Cast<IPreOrderItem>())
                {
                    minUnitCostPrice += preOrderItem.MinUnitPrice;
                    maxUnitCostPrice += preOrderItem.MaxUnitPrice;
                    if (isPriceCostAvailable && preOrderItem.IsRemainderNotAvailable)
                        isPriceCostAvailable = false;
                }
                if (!isThereMoreOfItems) break;
            }

            prices[0] = minUnitCostPrice;
            prices[1] = maxUnitCostPrice;
            prices[2] = minUnitCostPrice * quantity;
            prices[3] = maxUnitCostPrice * quantity;

            return (prices, isPriceCostAvailable);
        }
    }
}
