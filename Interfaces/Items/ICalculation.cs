using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting.Interfaces.Items
{
    public interface ICalculation : IBlockItem
    {
        public int Index { get; }
    }
}
