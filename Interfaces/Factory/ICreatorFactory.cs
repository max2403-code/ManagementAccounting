using ManagementAccounting.Classes.Common;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Classes.SystemCreators;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Interfaces.Factory
{
    public interface ICreatorFactory
    {
        //MaterialCollectionCreator CreateMaterialCreator();
        MaterialReceivingCollectionCreator CreateMaterialReceivingCreator(IMaterial material, int lengthOfItemsList);
        MaterialReceivingNotEmptyCollectionCreator CreateMaterialReceivingNotEmptyCreator(IMaterial material, int lengthOfItemsList);

        PreOrderItemCollectionCreator CreatePreOrderItemCollectionCreator(IPreOrder preOrder, int lengthOfItemsList);
        MaterialReceivingCollectionCreatorForOrders CreateMaterialReceivingCollectionCreatorForOrders(IOrderItem orderItem, int lengthOfItemsList);

        OrderItemCollectionCreatorForOrders CreateOrderItemCollectionCreatorForOrders(IOrderItem orderItem, int lengthOfItemsList);
        OrderMaterialReceivingCollectionCreator CreateOrderMaterialReceivingCollectionCreator(IOrderItem orderItem, int lengthOfItemsList);

        OrderItemCollectionCreatorFromMaterialReceiving CreateOrderItemCollectionCreatorFromMaterialReceiving(IMaterialReceiving materialReceiving, int lengthOfItemsList);

        OrderItemCollectionCreatorWithConsumption CreateOrderItemCollectionCreatorWithConsumption(IMaterial material, int lengthOfItemsList);

        OrderItemCollectionCreatorFromTempOrderItem CreateOrderItemCollectionCreatorFromTempOrderItem(int lengthOfItemsList);

        OrderItemCollectionCreator CreateOrderItemCollectionCreator(IOrder order, int lengthOfItemsList);

        CalculationItemCollectionCreator CreateCalculationItemCollectionCreator(ICalculation calculation,
            int lengthOfItemsList);

        MaterialCollectionCreator CreateMaterialCollectionCreator(int lengthOfItemsList);

        CalculationCollectionCreator CreateCalculationCollectionCreator(int lengthOfItemsList);

        //ICalculationCollectionCreator CreateCalculationCreator();

    }
}
