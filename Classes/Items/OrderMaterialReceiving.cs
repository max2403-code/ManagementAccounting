using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class OrderMaterialReceiving : BlockItemDB, IOrderMaterialReceiving
    {
        public string Name { get; }
        public int Index { get; private set; }
        public IOrderItem OrderItem { get; }
        public IMaterialReceiving MaterialReceiving { get; }
        public decimal Consumption { get; }
        //private IItemsFactory itemsFactory { get; }

        public OrderMaterialReceiving(IOrderItem orderItem, IMaterialReceiving materialReceiving, decimal consumption, int index, IDataBase dataBase, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Index = index;
            OrderItem = orderItem;
            MaterialReceiving = materialReceiving;
            Consumption = consumption;
            Name = string.Join(' ', "Списание из поступления от", MaterialReceiving.Date.ToString("dd / MM / yyyy"));
            //this.itemsFactory = itemsFactory;
        }
        //public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        //{
        //    cmd.Parameters.AddWithValue("OrderItemIdOMR", NpgsqlDbType.Integer, OrderItem.Order.Index);
        //    cmd.Parameters.AddWithValue("MaterialReceivingIdOMR", NpgsqlDbType.Integer, MaterialReceiving.Index);
        //    cmd.Parameters.AddWithValue("QuantityOMR", NpgsqlDbType.Numeric, MaterialConsumption);

        //}

        //public async Task AddItemToDataBase()
        //{
        //    var commandText = "INSERT INTO ordermaterialreceiving (OrderItemIdOMR, MaterialReceivingIdOMR, QuantityOMR) VALUES (@OrderItemIdOMR, @MaterialReceivingIdOMR, @QuantityOMR)";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД", AssignParametersToAddCommand);

        //    ExceptionEvent?.Invoke();
        //}

        //public async Task RemoveItemFromDataBase()
        //{
        //    var commandText = $"DELETE FROM ordermaterialreceiving WHERE idomr = {Index}";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД");
        //    ExceptionEvent?.Invoke();
        //}


        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return "INSERT INTO ordermaterialreceiving (OrderItemIdOMR, MaterialReceivingIdOMR, ConsumptionOMR) VALUES (@OrderItemIdOMR, @MaterialReceivingIdOMR, @ConsumptionOMR) RETURNING IdOMR";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("OrderItemIdOMR", NpgsqlDbType.Integer, OrderItem.Index);
            cmd.Parameters.AddWithValue("MaterialReceivingIdOMR", NpgsqlDbType.Integer, MaterialReceiving.Index);
            cmd.Parameters.AddWithValue("ConsumptionOMR", NpgsqlDbType.Numeric, Consumption);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdOMR"];

        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM ordermaterialreceiving WHERE idomr = {Index}";

        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблема с БД";
        }
    }
}
