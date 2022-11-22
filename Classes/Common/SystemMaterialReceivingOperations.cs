using System;
using System.Linq;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    class SystemMaterialReceivingOperations : ISystemMaterialReceivingOperations
    {
        private ICreatorFactory CreatorFactory { get; }
        private IOrderItemOperations OrderItemOperations { get; }

        public SystemMaterialReceivingOperations(ICreatorFactory creatorFactory, IOrderItemOperations orderItemOperations)
        {
            CreatorFactory = creatorFactory;
            OrderItemOperations = orderItemOperations;
        }

        public async Task Insert(IMaterialReceiving materialReceiving, bool isPreviouslyExistingItem = false)
        {
            var orderItemCreator = CreatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(materialReceiving, 5);
            var orderItemCreatorWithConsumption = CreatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            //await ((EditingBlockItemDB)materialReceiving).AddItemToDataBase();

            await DoOperationsWithItemsList(OrderItemOperations.RemoveReceiving, orderItemCreator, ">=");
            await ((EditingBlockItemDB) materialReceiving).AddItemToDataBase(isPreviouslyExistingItem);
            await DoOperationsWithItemsList(OrderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");


        }

        public async Task Remove(IMaterialReceiving materialReceiving)
        {
            var orderItemCreator = CreatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(materialReceiving, 5);
            var orderItemCreatorWithConsumption = CreatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            await DoOperationsWithItemsList(OrderItemOperations.RemoveReceiving, orderItemCreator, ">=");
            await ((EditingBlockItemDB)materialReceiving).RemoveItemFromDataBase();

            await DoOperationsWithItemsList(OrderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");
        }

        public async Task Edit(IMaterialReceiving materialReceiving, IMaterialReceiving newMaterialReceiving)
        {
            var controlMaterialReceiving = materialReceiving.Date > newMaterialReceiving.Date
                ? newMaterialReceiving
                : materialReceiving;
            var orderItemCreator = CreatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(controlMaterialReceiving, 5);

            var orderItemCreatorWithConsumption = CreatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            await DoOperationsWithItemsList(OrderItemOperations.RemoveReceiving, orderItemCreator, ">=");

            await ((EditingBlockItemDB)newMaterialReceiving).EditItemInDataBase<IMaterialReceiving>(newMaterialReceiving.Date, newMaterialReceiving.Quantity, newMaterialReceiving.Cost, newMaterialReceiving.Remainder, newMaterialReceiving.Note);
            await DoOperationsWithItemsList(OrderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");
        }

        public async Task Default(IMaterialReceiving materialReceiving, IMaterialReceiving previousMaterialReceiving)
        {
            var controlMaterialReceiving = materialReceiving.Date > previousMaterialReceiving.Date
                ? previousMaterialReceiving
                : materialReceiving;
            var orderItemCreator = CreatorFactory.CreateOrderItemCollectionCreatorFromMaterialReceiving(controlMaterialReceiving, 5);

            var orderItemCreatorWithConsumption = CreatorFactory.CreateOrderItemCollectionCreatorWithConsumption(materialReceiving.Material, 5);

            await DoOperationsWithItemsList(OrderItemOperations.RemoveReceiving, orderItemCreator, ">=");

            await((EditingBlockItemDB)previousMaterialReceiving).EditItemInDataBase<IMaterialReceiving>(previousMaterialReceiving.Date, previousMaterialReceiving.Quantity, previousMaterialReceiving.Cost, previousMaterialReceiving.Remainder, previousMaterialReceiving.Note);
            await DoOperationsWithItemsList(OrderItemOperations.AddReceiving, orderItemCreatorWithConsumption, "");
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
