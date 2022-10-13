using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public  interface IItemCreatable
    {
        public IBlockItem GetNewBlockItem(params object[] parameters);
    }
}
