﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class SystemOrderItemOperations : ISystemOrderItemOperations
    {
        private ICreatorFactory creatorFactory { get; }
        private IOrderItemOperations orderItemOperations { get; }

        public SystemOrderItemOperations(ICreatorFactory creatorFactory, IOrderItemOperations orderItemOperations)
        {
            this.creatorFactory = creatorFactory;
            this.orderItemOperations = orderItemOperations;
        }

        public async Task Insert(IOrderItem orderItem)
        {
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorForOrders(orderItem, 5);

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">");
            await orderItemOperations.AddReceiving(orderItem);
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreator, ">");
        }
        
        public async Task Remove(IOrderItem orderItem)
        {
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorForOrders(orderItem, 5);

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">");
            await orderItemOperations.RemoveReceiving(orderItem);
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreator, ">");

        }

        public async Task Edit(IOrderItem orderItem, IOrderItem newOrderItem)
        {
            var controlOrderItem = orderItem.Order.CreationDate > newOrderItem.Order.CreationDate
                ? newOrderItem
                : orderItem;
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorForOrders(controlOrderItem, 5);
            
            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">=");

            await ((EditingBlockItemDB)newOrderItem).EditItemInDataBase<IOrderItem>(newOrderItem.TotalConsumption, newOrderItem.TotalConsumption);
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreator, ">=");

        }

        public async Task Default(IOrderItem orderItem, IOrderItem previousOrderItem)
        {
            var controlOrderItem = orderItem.Order.CreationDate > previousOrderItem.Order.CreationDate
                ? previousOrderItem
                : orderItem;
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorForOrders(controlOrderItem, 5);

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">=");

            await ((EditingBlockItemDB)previousOrderItem).EditItemInDataBase<IOrderItem>(previousOrderItem.TotalConsumption, previousOrderItem.TotalConsumption);
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreator, ">=");
        }

        private async Task DoOperationsWithItemsList(Func<IOrderItem, Task> operations, BlockItemsCollectionCreator orderItemCreator, string searchCriterion, int skipId = -1)
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
                    if(nextOrderItem.Index == skipId) continue;
                    await operations(nextOrderItem);
                }
            }
        }
    }
}