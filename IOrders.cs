using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public interface IOrders : IProgramBlock
    {
        Task<List<ICalculation>> GetCalculationsList(int offset, params string[] selectionCriterion);
    }
}
