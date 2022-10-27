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
    public class SystemOrderOperations : ISystemOrderOperations
    {

        private ICreatorFactory creatorFactory { get; }
        private ISystemOrderItemOperations orderItemOperations { get; }
        private IItemsFactory itemsFactory { get; }

        public SystemOrderOperations(ICreatorFactory creatorFactory, ISystemOrderItemOperations orderItemOperations, IItemsFactory itemsFactory)
        {
            this.creatorFactory = creatorFactory;
            this.orderItemOperations = orderItemOperations;
            this.itemsFactory = itemsFactory;
        }

        public async Task Insert(IOrder order)
        {
            var orderItemsCreator = creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await DoInsertOperationsWithItemsList(orderItemOperations.Insert, orderItemsCreator, "");
        }

        public async Task Remove(IOrder order) // можно ускорить алгоритм
        {
            var orderItemsCreator = creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await DoInsertOperationsWithItemsList(orderItemOperations.Remove, orderItemsCreator, "");
        }

        public async Task Edit(IOrder order, IOrder newOrder)
        {
            var orderItemsCreator = creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await ((EditingBlockItemDB) newOrder).EditItemInDataBase<IOrder>(newOrder.CreationDate);
            await DoEditOperationsWithItemsList(orderItemOperations.Edit, newOrder, orderItemsCreator, "");
        }

        public async Task Default(IOrder order, IOrder previousOrder)
        {
            var orderItemsCreator = creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            await ((EditingBlockItemDB)previousOrder).EditItemInDataBase<IOrder>(previousOrder.CreationDate);

            await DoEditOperationsWithItemsList(orderItemOperations.Default, previousOrder, orderItemsCreator, "");
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
                    var editedOrderItem = itemsFactory.CreateOrderItem(editedOrder, orderItem.Material,
                        orderItem.TotalConsumption, orderItem.TotalConsumption, orderItem.Index);
                    await operations(orderItem, editedOrderItem);
                }
            }
        }
    }
}
