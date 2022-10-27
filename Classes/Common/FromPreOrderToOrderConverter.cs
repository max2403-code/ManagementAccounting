using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class FromPreOrderToOrderConverter : IFromPreOrderToOrderConverter
    {
        private IItemsFactory itemsFactory { get; }
        private ICreatorFactory creatorFactory { get; }
        public FromPreOrderToOrderConverter(IItemsFactory itemsFactory, ICreatorFactory creatorFactory)
        {
            this.creatorFactory = creatorFactory;
            this.itemsFactory = itemsFactory;
        }

        public async Task<IOrder> Convert(IPreOrder preOrder, DateTime creationDate)
        {
            var order = itemsFactory.CreateOrder(preOrder.Calculation.Name, creationDate, preOrder.Quantity);
            var preOrderItemCreator = creatorFactory.CreatePreOrderItemCollectionCreator(preOrder, 5);
            await ((EditingBlockItemDB) order).AddItemToDataBase();

            await CreateOrderItems(order, preOrderItemCreator);

            return order;

        }

        private async Task CreateOrderItems(IOrder order, BlockItemsCollectionCreator creator)
        {
            var offset = 0;

            while (true)
            {
                var resultOfGettingItemsList = await creator.GetItemsList(offset, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;
                offset += creator.LengthOfItemsList;

                foreach (var preOrderItem in itemsList.Cast<IPreOrderItem>())
                {
                    var orderItem = itemsFactory.CreateOrderItem(order, preOrderItem.Material, preOrderItem.MaterialСonsumption, preOrderItem.MaterialСonsumption);
                    await ((EditingBlockItemDB)orderItem).AddItemToDataBase();
                }

                if (!isThereMoreOfItems) break;
            }
        }

        public async Task RemoveOrder(IOrder order)
        {
            await ((EditingBlockItemDB)order).RemoveItemFromDataBase();
        }
    }
}
