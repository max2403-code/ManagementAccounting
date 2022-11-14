using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting
{
    public interface IAddable 
    {
        //void AssignParametersToAddCommand(NpgsqlCommand cmd);

        Task AddItemToDataBase(bool isPreviouslyExistingItem = false);
    }
}
