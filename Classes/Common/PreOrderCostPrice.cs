using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class PreOrderCostPrice : IPreOrderCostPrice
    {
        private ICreatorFactory creatorFactory { get; }

        public PreOrderCostPrice(ICreatorFactory creatorFactory)
        {
            this.creatorFactory = creatorFactory;
        }

        public async Task<(decimal, decimal, bool)> GetPreOrderCostPrice(IPreOrder preOrder)
        {
            var preOrderItemCreator = creatorFactory.CreatePreOrderItemCollectionCreator(preOrder, 5);
            var offset = 0;
            var minUnitCostPrice = 0m;
            var maxUnitCostPrice = 0m;
            var isPriceCostAvailable = true;

            while (true)
            {
                var resultOfGettingItemsList = await preOrderItemCreator.GetItemsList(offset, "");
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

            return (minUnitCostPrice, maxUnitCostPrice, isPriceCostAvailable);
        }
    }
}
