using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting
{
    public interface IItemsFactory
    {
        Material CreateMaterial(MaterialType materialType, string name, UnitOfMaterial unit, int index = -1);
        MaterialReceiving CreateMaterialReceiving(IMaterial material, DateTime date, decimal quantity, decimal cost, decimal remainder, string note, int index = -1);
        Calculation CreateCalculation(string calculationName, int index = -1);
        CalculationItem CreateCalculationItem(IMaterial material, decimal consumption, int calculationId, int index = -1);
        PreOrder CreatePreOrder(BlockItemDB calculation, int quantity, DateTime creationDate, int index = -1);
        PreOrderItem CreatePreOrderItem(IMaterial material, decimal materialUnitСonsumption, decimal minUnitPrice, decimal maxUnitPrice, int quantity, bool isRemainderNotAvailable);
        
        
        
        
        IOrder CreateOrder(string name, DateTime creationDate, decimal quantity, int index = -1);
        IOrderItem CreateOrderItem(IOrder order, IMaterial material, decimal materialСonsumption, int index = -1);
        IOrderMaterialReceiving CreateOrderMaterialReceiving(IOrderItem orderItem, IMaterialReceiving materialReceiving, decimal materialConsumption, int index = -1);
    }
}