using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting
{
    public interface IAddable :IIndexable
    {
        //void AssignParametersToAddCommand(NpgsqlCommand cmd);

        Task AddItemToDataBase();
    }
}
