using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public interface ICalculation : IBlockItem, IProgramBlock, IAddable, IEditable, IRemovable
    {
        Task<List<IMaterial>> GetMaterialsList(int offset, params string[] selectionCriterion);
    }
}
