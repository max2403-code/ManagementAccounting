using System;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting
{
    public interface IPreOrder : IBlockItem
    {
        public int Index { get; }
        //public bool IsCostPriceFull { get; }
        //public decimal MaxCostPrice { get; }
        //public decimal MinCostPrice { get; }
        public int Quantity { get; }
        public DateTime CreationDate { get; }
        public ICalculation Calculation { get; }

        //public Task AssignCostPrice();
    }
}
