using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ManagementAccounting
{
    public interface IEditable
    {
        void AssignParametersToEditCommand(NpgsqlCommand cmd);

        Task EditItemInDataBase(params object[] parameters);
    }
}
