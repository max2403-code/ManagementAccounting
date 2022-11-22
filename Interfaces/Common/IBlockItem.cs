namespace ManagementAccounting
{
    public interface IBlockItem
    {
        string Name { get; }
        //void AssignParametersToAddCommand(NpgsqlCommand cmd);
        //void AssignParametersToEditCommand(NpgsqlCommand cmd);

        //Task AddItemToDataBase();
        //Task EditItemInDataBase(params object[] parameters);
        //Task RemoveItemFromDataBase();
    }
}
