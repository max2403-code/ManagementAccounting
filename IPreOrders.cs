using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public interface IPreOrders : IProgramBlock
    {
        public Task<List<ICalculation>> GetCalculations(int offset, params string[] selectionCriterion);
    }
}
