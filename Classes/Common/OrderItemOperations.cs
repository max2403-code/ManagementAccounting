using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class OrderItemOperations : IOrderItemOperations
    {
        private IItemsFactory ItemsFactory{ get; }
        private ICreatorFactory CreatorFactory{ get; }



        public OrderItemOperations(IItemsFactory itemsFactory, ICreatorFactory creatorFactory)
        {
            CreatorFactory = creatorFactory;
            ItemsFactory = itemsFactory;
        }

        public async Task AddReceiving(IOrderItem orderItem)
        {
            var materialReceivingCollectionCreator = CreatorFactory.CreateMaterialReceivingCollectionCreatorForOrders(orderItem, 1);
            var consumption = orderItem.Consumption;
            var isEndOfOperation = consumption == 0;

            while (!isEndOfOperation)
            {
                var resultOfGettingItemsList = await materialReceivingCollectionCreator.GetItemsList(0, "");
                var itemsList = resultOfGettingItemsList.Item1;

                if (itemsList.Count == 0)
                    throw new OrderItemOperationException($"Недостаточно на складе: {orderItem.Material.Name}");
                
                foreach (var blockItem  in itemsList)
                {
                    var materialReceiving = (IMaterialReceiving) blockItem;
                    var remainder = materialReceiving.Remainder;
                    var temporaryConsumption = consumption;

                    if (consumption <= remainder)
                    {
                        remainder -= consumption;
                        consumption = 0;
                        isEndOfOperation = true;
                    }
                    else
                    {
                        consumption -= remainder;
                        temporaryConsumption = remainder;
                        remainder = 0;
                    }

                    await ((EditingBlockItemDB) orderItem).EditItemInDataBase<IOrderItem>(consumption, orderItem.TotalConsumption);
                    await ((EditingBlockItemDB) materialReceiving).EditItemInDataBase<IMaterialReceiving>(materialReceiving.Date, materialReceiving.Quantity, materialReceiving.Cost, remainder, materialReceiving.Note);
                    
                    var orderMaterialReceiving = ItemsFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, temporaryConsumption);

                    await ((BlockItemDB)orderMaterialReceiving).AddItemToDataBase();
                }
            }
        }

        public async Task RemoveReceiving(IOrderItem orderItem)
        {
            var orderItemConsumption = orderItem.Consumption;
            var orderMaterialReceivingCollectionCreator = CreatorFactory.CreateOrderMaterialReceivingCollectionCreator(orderItem, 1);

            while (true)
            {
                var resultOfGettingItemsList = await orderMaterialReceivingCollectionCreator.GetItemsList(0, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;

                foreach (var blockItem in itemsList)
                {
                    var orderMaterialReceiving = (IOrderMaterialReceiving)blockItem;
                    var materialReceiving = orderMaterialReceiving.MaterialReceiving;
                    var orderMaterialReceivingConsumption = orderMaterialReceiving.Consumption;
                    var remainder = orderMaterialReceivingConsumption + materialReceiving.Remainder;
                    orderItemConsumption += orderMaterialReceivingConsumption;

                    await ((EditingBlockItemDB) orderItem).EditItemInDataBase<IOrderItem>(orderItemConsumption, orderItem.TotalConsumption);
                    await ((EditingBlockItemDB)materialReceiving).EditItemInDataBase<IMaterialReceiving>(materialReceiving.Date, materialReceiving.Quantity, materialReceiving.Cost, remainder, materialReceiving.Note);
                    await ((BlockItemDB)orderMaterialReceiving).RemoveItemFromDataBase();
                }
                if (!isThereMoreOfItems) break;
            }

            //await ((EditingBlockItemDB) orderItem).RemoveItemFromDataBase();
        }
    }
}
