using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IBlockItemsFactory
    {
        IMaterial CreateMaterial(MaterialType materialType, string name, int index = -1);
        IMaterialReceiving CreateMaterialReceiving(DateTime date, decimal quantity, decimal cost, decimal remainder, string note, int materialIndex, int index = -1);
        ICalculation CreateCalculation(string calculationName, int index = -1);
        ICalculationItem CreateCalculationItem(IMaterial material,  decimal quantity, int calculationId, int index = -1);
        IPreOrder CreatePreOrder(ICalculation calculation, decimal quantity, DateTime creationDate, int index = -1);
        IPreOrderItem CreatePreOrderItem(string materialName, int materialId, decimal materialСonsumption, decimal minPrice, decimal maxPrice);
        IOrder CreateOrder(string name, DateTime creationDate, decimal quantity, int index = -1);
        IOrderItem CreateOrderItem(IOrder order, IMaterial material, decimal materialСonsumption, int index = -1);
        IOrderMaterialReceiving CreateOrderMaterialReceiving(IOrderItem orderItem, IMaterialReceiving materialReceiving, decimal materialConsumption, int index = -1);
    }
}