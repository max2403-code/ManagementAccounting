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
    class SystemMaterialReceivingOperations : ISystemMaterialReceivingOperations
    {
        private ICreatorFactory creatorFactory { get; }
        private IOrderItemOperations orderItemOperations { get; }

        public SystemMaterialReceivingOperations(ICreatorFactory creatorFactory, IOrderItemOperations orderItemOperations)
        {
            this.creatorFactory = creatorFactory;
            this.orderItemOperations = orderItemOperations;
        }

        public async Task Insert(IMaterialReceiving materialReceiving)
        {
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(materialReceiving, 5);
            var orderItemCreatorWithConsumption = creatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            //await ((EditingBlockItemDB)materialReceiving).AddItemToDataBase();

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">");
            await ((EditingBlockItemDB) materialReceiving).AddItemToDataBase();
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");


        }

        public async Task Remove(IMaterialReceiving materialReceiving)
        {
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(materialReceiving, 5);
            var orderItemCreatorWithConsumption = creatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">");
            await ((EditingBlockItemDB)materialReceiving).RemoveItemFromDataBase();

            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");
        }

        public async Task Edit(IMaterialReceiving materialReceiving, IMaterialReceiving newMaterialReceiving)
        {
            var controlMaterialReceiving = materialReceiving.Date > newMaterialReceiving.Date
                ? newMaterialReceiving
                : materialReceiving;
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(controlMaterialReceiving, 5);

            var orderItemCreatorWithConsumption = creatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">=");

            await ((EditingBlockItemDB)newMaterialReceiving).EditItemInDataBase<IMaterialReceiving>(newMaterialReceiving.Date, newMaterialReceiving.Quantity, newMaterialReceiving.Cost, newMaterialReceiving.Remainder, newMaterialReceiving.Note);
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");
        }

        public async Task Default(IMaterialReceiving materialReceiving, IMaterialReceiving previousMaterialReceiving)
        {
            var controlMaterialReceiving = materialReceiving.Date > previousMaterialReceiving.Date
                ? previousMaterialReceiving
                : materialReceiving;
            var orderItemCreator = creatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(controlMaterialReceiving, 5);

            var orderItemCreatorWithConsumption = creatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            await DoOperationsWithItemsList(orderItemOperations.RemoveReceiving, orderItemCreator, ">=");

            await((EditingBlockItemDB)previousMaterialReceiving).EditItemInDataBase<IMaterialReceiving>(previousMaterialReceiving.Date, previousMaterialReceiving.Quantity, previousMaterialReceiving.Cost, previousMaterialReceiving.Remainder, previousMaterialReceiving.Note);
            await DoOperationsWithItemsList(orderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");
        }

        private async Task DoOperationsWithItemsList(Func<IOrderItem, Task> operations, BlockItemsCollectionCreator orderItemCreator, string searchCriterion)
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
    }
}
