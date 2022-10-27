using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IOrderMaterialReceiving : IBlockItem
    {
        public int Index { get; }
        public decimal Consumption { get; }
        public IOrderItem OrderItem { get; }
        public IMaterialReceiving MaterialReceiving { get; }
    }
}
