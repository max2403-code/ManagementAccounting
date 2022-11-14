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
        private IItemsFactory ItemsFactory { get; }
        private ICreatorFactory CreatorFactory { get; }

        public FromPreOrderToOrderConverter(IItemsFactory itemsFactory, ICreatorFactory creatorFactory)
        {
            CreatorFactory = creatorFactory;
            ItemsFactory = itemsFactory;
        }

        public async Task<IOrder> Convert(IPreOrder preOrder, DateTime creationDate)
        {
            var order = ItemsFactory.CreateOrder(preOrder.Calculation.Name, creationDate, preOrder.Quantity);
            await ((EditingBlockItemDB) order).AddItemToDataBase();

            //await CreateOrderItems(order, preOrderItemCreator);

            return order;

        }

        public async Task CreateOrderItems(IOrder order, IPreOrder preOrder)
        {
            var preOrderItemCreator = CreatorFactory.CreateCalculationItemCollectionCreator(preOrder.Calculation, 5);
            var offset = 0;

            while (true)
            {
                var resultOfGettingItemsList = await preOrderItemCreator.GetItemsList(offset, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;
                offset += preOrderItemCreator.LengthOfItemsList;

                foreach (var calculationItem in itemsList.Cast<ICalculationItem>())
                {
                    var orderItem = ItemsFactory.CreateOrderItem(order, calculationItem.Material, calculationItem.Consumption * preOrder.Quantity, calculationItem.Consumption * preOrder.Quantity);
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
