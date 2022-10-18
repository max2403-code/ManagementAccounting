using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting
{
    public interface IPreOrder 
    {
        //public bool IsCostPriceFull { get; }
        //public decimal MaxCostPrice { get; }
        //public decimal MinCostPrice { get; }
        public int Quantity { get; }
        public DateTime CreationDate { get; }
        public BlockItemDB Calculation { get; }

        //public Task AssignCostPrice();
    }
}
