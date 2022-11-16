using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IPreOrderItem : IBlockItem
    {
        public bool IsRemainderNotAvailable { get; }
        public IMaterial Material { get; }
        public decimal MaterialUnitСonsumption { get; }
        public decimal MaxUnitPrice { get; }
        public decimal MinUnitPrice { get; }
        public decimal MaterialСonsumption { get; }
        public decimal MaxPrice { get; }
        public decimal MinPrice { get; }
        public int Quantity { get; }
        public decimal MaxMaterialPrice { get; }
        public decimal MinMaterialPrice { get; }
    }
}
