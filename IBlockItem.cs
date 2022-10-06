using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting
{
    public interface IBlockItem
    {
        public event Action ExceptionEvent;
        string Name { get; }
        int Index { get; }
        //void AssignParametersToAddCommand(NpgsqlCommand cmd);
        //void AssignParametersToEditCommand(NpgsqlCommand cmd);

        //Task AddItemToDataBase();
        //Task EditItemInDataBase(params object[] parameters);
        //Task RemoveItemFromDataBase();
    }
}
