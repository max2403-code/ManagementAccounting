using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public class PreOrderItem : IPreOrderItem
    {
        public event Action ExceptionEvent;
        public string Name { get; }
        public int Index { get; }
        public decimal MaterialСonsumption { get; }
        public decimal MinPrice { get; }
        public decimal MaxPrice { get; }
        public int MaterialId { get; }


        public PreOrderItem(string materialName, int materialId, decimal materialСonsumption, decimal minPrice, decimal maxPrice)
        {
            Name = materialName;
            MaterialСonsumption = materialСonsumption;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            MaterialId = materialId;
        }
    }
}
