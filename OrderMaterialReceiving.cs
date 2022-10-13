using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class OrderMaterialReceiving : IOrderMaterialReceiving
    {
        public event Action ExceptionEvent;
        public string Name { get; }
        public int Index { get; }
        public IOrderItem OrderItem { get; }
        public IMaterialReceiving MaterialReceiving { get; }
        public decimal MaterialConsumption { get; }
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _itemsFactory;

        public OrderMaterialReceiving(IOrderItem orderItem, IMaterialReceiving materialReceiving,
            decimal materialConsumption, int index, IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            Name = orderItem.Material.Name;
            Index = index;
            OrderItem = orderItem;
            MaterialReceiving = materialReceiving;
            MaterialConsumption = materialConsumption;
            _dataBase = dataBase;
            _itemsFactory = itemsFactory;
        }
        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("OrderItemIdOMR", NpgsqlDbType.Integer, OrderItem.Order.Index);
            cmd.Parameters.AddWithValue("MaterialReceivingIdOMR", NpgsqlDbType.Integer, MaterialReceiving.Index);
            cmd.Parameters.AddWithValue("QuantityOMR", NpgsqlDbType.Numeric, MaterialConsumption);

        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO ordermaterialreceiving (OrderItemIdOMR, MaterialReceivingIdOMR, QuantityOMR) VALUES (@OrderItemIdOMR, @MaterialReceivingIdOMR, @QuantityOMR)";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблема с БД", AssignParametersToAddCommand);

            ExceptionEvent?.Invoke();
        }

        public async Task RemoveItemFromDataBase()
        {
            var commandText = $"DELETE FROM ordermaterialreceiving WHERE idomr = {Index}";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблема с БД");
            ExceptionEvent?.Invoke();
        }

       
    }
}
