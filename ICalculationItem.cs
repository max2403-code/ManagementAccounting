using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface ICalculationItem : IBlockItem, IAddable, IEditable, IRemovable
    {
        public int CalculationId { get; }
        public int MaterialId { get; }
        public decimal Quantity { get; }
    }
}
