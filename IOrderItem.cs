using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IOrderItem : IProgramBlock, IBlockItem
    {
        public IMaterial Material { get; }
        public IOrder Order { get; }

    }
}
