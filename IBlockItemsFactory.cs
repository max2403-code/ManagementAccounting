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
        IOrder CreateOrder(ICalculation calculation, DateTime creationDate, decimal quantity, int index = -1);
        IOrderItem CreateOrderItem(ICalculationItem calculationItem, int orderId, int index = -1);
    }
}
