using System;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting
{
    public interface IItemsFactory
    {
        IMaterial CreateMaterial(MaterialType materialType, string name, UnitOfMaterial unit, int index = -1);
        IMaterialReceiving CreateMaterialReceiving(IMaterial material, DateTime date, decimal quantity, decimal cost, decimal remainder, string note, int index = -1);
        ICalculation CreateCalculation(string calculationName, int index = -1);
        ICalculationItem CreateCalculationItem(IMaterial material, decimal consumption, int calculationId, int index = -1);
        IPreOrder CreatePreOrder(ICalculation calculation, int quantity, DateTime creationDate, int index = -1);
        IPreOrderItem CreatePreOrderItem(IMaterial material, decimal materialUnitСonsumption, decimal minMaterialPrice, decimal maxMaterialPrice, int quantity, bool isRemainderNotAvailable);
        IOrder CreateOrder(string shortName, DateTime creationDate, int quantity, int index = -1);
        IOrderItem CreateOrderItem(IOrder order, IMaterial material, decimal consumption, decimal totalConsumption, int index = -1);
        IOrderMaterialReceiving CreateOrderMaterialReceiving(IOrderItem orderItem, IMaterialReceiving materialReceiving, decimal consumption, int index = -1);
    }
}