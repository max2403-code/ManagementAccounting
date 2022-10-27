using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IOrder : IBlockItem
    {
        public int Index { get; }
        public DateTime CreationDate { get; }
        public int Quantity { get; }
        public string ShortName { get; }

    }
}
