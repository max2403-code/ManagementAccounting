using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IMaterial : IBlockItem
    { 
        public int Index { get; }
        public MaterialType MaterialType { get; } 
        public UnitOfMaterial Unit { get; }
    }
}
