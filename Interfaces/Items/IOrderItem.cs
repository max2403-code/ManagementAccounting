namespace ManagementAccounting
{
    public interface IOrderItem : IBlockItem
    {
        public int Index { get; }
        public IMaterial Material { get; }
        public IOrder Order { get; }
        public decimal Consumption { get; }
        public decimal TotalConsumption { get; }
        public decimal UnitConsumption { get; }
    }
}
