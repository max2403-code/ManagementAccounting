using System;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting
{
    public interface IOrder : IBlockItem, IUpdatable
    {
        public int Index { get; }
        public DateTime CreationDate { get; }
        public int Quantity { get; }
        public string ShortName { get; }

    }
}
