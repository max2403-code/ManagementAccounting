using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting
{
    public class PreOrderItem : IPreOrderItem
    {
        public bool IsRemainderNotAvailable { get; }
        public IMaterial Material { get; }
        public string Name { get; }
        public decimal MaterialUnitСonsumption { get; }
        public decimal MaxUnitPrice { get; }
        public decimal MinUnitPrice { get; }
        public decimal MaterialСonsumption => MaterialUnitСonsumption * Quantity;
        public decimal MinPrice => MinUnitPrice * Quantity;
        public decimal MaxPrice => MaxUnitPrice * Quantity;
        public int Quantity { get; }

        public PreOrderItem(IMaterial material, decimal materialUnitСonsumption, decimal minUnitPrice, decimal maxUnitPrice, int quantity, bool isRemainderNotAvailable)
        {
            IsRemainderNotAvailable = isRemainderNotAvailable;
            Material = material;
            Name = ((BlockItemDB)Material).Name;
            MaterialUnitСonsumption = materialUnitСonsumption;
            MinUnitPrice = minUnitPrice;
            MaxUnitPrice = maxUnitPrice;
            Quantity = quantity;
        }
    }
}
