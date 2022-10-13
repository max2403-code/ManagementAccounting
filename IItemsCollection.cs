using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public interface IItemsCollection
    {
        public string ItemTypeName { get; }
        public int LengthOfItemsList { get; }

        public Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion);
        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item);
    }
}
