using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IMaterialReceiving : IBlockItem, IAddable, IEditable, IRemovable
    {
        public int MaterialIndex { get; }
        public DateTime Date { get; }
        public decimal Quantity { get; }
        public decimal Cost { get; }
        public decimal Price { get; }
        public decimal Remainder { get; }
        public string Note { get; }
    }
}
