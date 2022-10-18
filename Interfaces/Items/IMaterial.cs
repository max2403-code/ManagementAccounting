using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IMaterial 
    {
        public MaterialType MaterialType { get; } 
        public UnitOfMaterial Unit { get; }
    }
}
