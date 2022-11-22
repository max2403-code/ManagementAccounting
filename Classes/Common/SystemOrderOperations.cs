using System;
using System.Linq;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class SystemOrderOperations : ISystemOrderOperations
    {

        private ICreatorFactory CreatorFactory { get; }
        private ISystemOrderItemOperations OrderItemOperations { get; }
        private IItemsFactory ItemsFactory { get; }

        public SystemOrderOperations(ICreatorFactory creatorFactory, ISystemOrderItemOperations orderItemOperations, IItemsFactory itemsFactory)
        {
            CreatorFactory = creatorFactory;
            OrderItemOperations = orderItemOperations;
            ItemsFactory = itemsFactory;
        }

        public async Task Insert(IOrder order)
        {
            var orderItemsCreator = CreatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await DoInsertOperationsWithItemsList(OrderItemOperations.Insert, orderItemsCreator, "");
        }

        public async Task Remove(IOrder order) // можно ускорить алгоритм
        {
            var orderItemsCreator = CreatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await DoInsertOperationsWithItemsList(OrderItemOperations.Remove, orderItemsCreator, "");
        }

        public async Task Edit(IOrder order, IOrder newOrder)
        {
            var orderItemsCreator = CreatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await ((EditingBlockItemDB) newOrder).EditItemInDataBase<IOrder>(newOrder.CreationDate);
            await DoEditOperationsWithItemsList(OrderItemOperations.Edit, newOrder, orderItemsCreator, "");
        }

        public async Task Default(IOrder order, IOrder previousOrder)
        {
            var orderItemsCreator = CreatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await ((EditingBlockItemDB)previousOrder).EditItemInDataBase<IOrder>(previousOrder.CreationDate);

            await DoEditOperationsWithItemsList(OrderItemOperations.Default, previousOrder, orderItemsCreator, "");
        }

        private async Task DoInsertOperationsWithItemsList(Func<IOrderItem, Task> operations, BlockItemsCollectionCreator orderItemCreator, string searchCriterion)
        {
            var offset = 0;
            while (true)
            {
                var resultOfGettingItemsList = await orderItemCreator.GetItemsList(offset, searchCriterion);
                var itemsList = resultOfGettingItemsList.Item1;

                if (itemsList.Count == 0) break;

                offset += orderItemCreator.LengthOfItemsList;

                foreach (var nextOrderItem in itemsList.Cast<IOrderItem>())
                {
                    await operations(nextOrderItem);
                }
            }
        }

        private async Task DoEditOperationsWithItemsList(Func<IOrderItem, IOrderItem, Task> operations, IOrder editedOrder, BlockItemsCollectionCreator orderItemCreator, string searchCriterion)
        {
            var offset = 0;
            while (true)
            {
                var resultOfGettingItemsList = await orderItemCreator.GetItemsList(offset, searchCriterion);
                var itemsList = resultOfGettingItemsList.Item1;

                if (itemsList.Count == 0) break;

                offset += orderItemCreator.LengthOfItemsList;

                foreach (var orderItem in itemsList.Cast<IOrderItem>())
                {
                    var oldOrderItem = ItemsFactory.CreateOrderItem(orderItem.Order, orderItem.Material, orderItem.TotalConsumption, orderItem.TotalConsumption, orderItem.Index);
                    var editedOrderItem = ItemsFactory.CreateOrderItem(editedOrder, oldOrderItem.Material, oldOrderItem.Consumption, oldOrderItem.TotalConsumption, oldOrderItem.Index);
                    await operations(oldOrderItem, editedOrderItem);
                }
            }
        }
    }
}
