namespace ManagementAccounting
{
    public interface ICalculationItem : IBlockItem
    {
        public int Index { get; }
        public IMaterial Material { get; }
        public decimal Consumption { get; }
    }
}
