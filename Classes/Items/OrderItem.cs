using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class OrderItem : EditingBlockItemDB, IOrderItem
    {
        public string Name { get; }
        public int Index { get; private set; }
        public IMaterial Material { get; }
        public IOrder Order { get; }
        public decimal Consumption { get; private set; }
        public decimal TotalConsumption { get; private set; }
        public decimal UnitConsumption => TotalConsumption / Order.Quantity;
        private IItemsFactory ItemsFactory { get; }

        public OrderItem(IOrder order, IMaterial material, decimal consumption, decimal totalConsumption, int index, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Index = index;
            Order = order;
            Material = material;
            Name = Material.Name;
            Consumption = consumption;
            TotalConsumption = totalConsumption;
            ItemsFactory = itemsFactory;
        }

        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return "INSERT INTO orderitems (OrderIdOI, MaterialIdOI, ConsumptionOI, TotalConsumptionOI) VALUES (@OrderIdOI, @MaterialIdOI, @ConsumptionOI, @TotalConsumptionOI) RETURNING IdOI;";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("OrderIdOI", NpgsqlDbType.Integer, Order.Index);
            cmd.Parameters.AddWithValue("MaterialIdOI", NpgsqlDbType.Integer, Material.Index);
            cmd.Parameters.AddWithValue("ConsumptionOI", NpgsqlDbType.Numeric, Consumption);
            cmd.Parameters.AddWithValue("TotalConsumptionOI", NpgsqlDbType.Numeric, TotalConsumption);

        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdOI"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T) ItemsFactory.CreateOrderItem(Order, Material, Consumption, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE orderitems SET ConsumptionOI = @ConsumptionOI, TotalConsumptionOI = @TotalConsumptionOI WHERE idoi = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            Consumption = (decimal)parameters[0];
            TotalConsumption = (decimal) parameters[1];
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("ConsumptionOI", NpgsqlDbType.Numeric, Consumption);
            cmd.Parameters.AddWithValue("TotalConsumptionOI", NpgsqlDbType.Numeric, TotalConsumption);
        }

        private protected override void UndoValues<T>(T copyItem)
        {
            Consumption = ((IOrderItem)copyItem).Consumption;
            TotalConsumption = ((IOrderItem)copyItem).TotalConsumption;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM orderitems WHERE idoi = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблема с БД";
        }
    }
}
