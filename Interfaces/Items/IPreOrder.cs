using System;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting
{
    public interface IPreOrder : IBlockItem
    {
        public int Index { get; }
        public int Quantity { get; }
        public DateTime CreationDate { get; }
        public ICalculation Calculation { get; }
    }
}
