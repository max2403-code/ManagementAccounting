using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public interface IRemovable
    {
        Task RemoveItemFromDataBase();
    }
}
