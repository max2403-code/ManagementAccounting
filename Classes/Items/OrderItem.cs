using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
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

        private IItemsFactory itemsFactory { get; }

        public OrderItem(IOrder order, IMaterial material, decimal consumption, decimal totalConsumption, int index, IDataBase dataBase, IItemsFactory itemsFactory)
            : base(dataBase)
        {
            Index = index;
            Order = order;
            Material = material;
            Name = Material.Name;
            Consumption = consumption;
            TotalConsumption = totalConsumption;
            this.itemsFactory = itemsFactory;
        }


        //public IBlockItem GetNewBlockItem(params object[] parameters)
        //{
        //    var orderItem = this;
        //    var materialReceiving = (IMaterialReceiving)parameters[0];
        //    var materialConsumption = (decimal)parameters[1];
        //    return _itemFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, materialConsumption);
        //}


        //public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        //{
        //    var commandText = $"SELECT * FROM ordermaterialreceiving, materialreceiving WHERE ordermaterialreceiving.OrderItemIdOMR = {Index} AND ordermaterialreceiving.MaterialReceivingIdOMR = materialreceiving.IdMR LIMIT {LengthOfItemsList + 1} OFFSET {offset};";
        //    var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

        //    return itemsList;
        //}

        //public IBlockItem GetItemFromDataBase(DbDataRecord item)
        //{
        //    var orderItem = this;
        //    var date = (DateTime)item["ReceiveDatemr"];
        //    var quantity = (decimal)item["Quantitymr"];
        //    var cost = (decimal)item["TotalCostmr"];
        //    var remainder = (decimal)item["Remaindermr"];
        //    var note = (string)item["Notemr"];
        //    var materialIndex = (int)item["MaterialIdmr"];
        //    var index = (int)item["Idmr"];
        //    var materialReceiving = _itemFactory.CreateMaterialReceiving(date, quantity, cost, remainder, note, materialIndex, index);
        //    var materialConsumption = (decimal)item["QuantityOMR"];

        //    return _itemFactory.CreateOrderMaterialReceiving(orderItem, materialReceiving, materialConsumption);
        //}


        private protected override string GetAddItemCommandText()
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
            return (T) itemsFactory.CreateOrderItem(Order, Material, Consumption, Index);
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
