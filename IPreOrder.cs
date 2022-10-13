using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public interface IPreOrder : IBlockItem, IAddable, IEditable, IRemovable, IItemsCollection
    {
        public bool IsCostPriceFull { get; }
        public decimal MaxCostPrice { get; }
        public decimal MinCostPrice { get; }
        public Task AssignCostPrice();
    }
}
