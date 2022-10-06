using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IMaterial : IBlockItem, IProgramBlock, IAddable, IEditable, IRemovable
    {
        public MaterialType MaterialType { get; }
    }
}
