using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class Order : EditingBlockItemDB, IOrder 
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int Quantity { get; }
        public string ShortName { get; }
        private IItemsFactory itemsFactory { get; }

        public Order(string shortName, DateTime creationDate, int quantity, int index, IDataBase dataBase, IItemsFactory itemsFactory) 
            : base(dataBase)
        {
            Index = index;
            Name = string.Join(' ', shortName, "от", creationDate.ToString("dd/MM/yyyy"));
            ShortName = shortName;
            CreationDate = creationDate;
            Quantity = quantity;
            this.itemsFactory = itemsFactory;
        }

        //public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        //{
        //    var commandText = $"SELECT * FROM orderitems, remainders WHERE orderitems.OrderIdOI = {Index} AND orderitems.MaterialIdOI = remainders.IdM ORDER BY remainders.MaterialTypeM, remainders.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        //    var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

        //    return itemsList;
        //}

        //public IBlockItem GetItemFromDataBase(DbDataRecord item)
        //{

        //    var order = this;
        //    var materialType = (MaterialType)(int)item["MaterialTypeM"];
        //    var materialName = (string)item["MaterialNameM"];
        //    var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
        //    var materialIndex = (int)item["IdM"];

        //    var material = _itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, materialIndex);
        //    var materialConsumption = (decimal)item["QuantityOI"];
        //    var index = (int)item["IdOI"];

        //    return _itemsFactory.CreateOrderItem(order, material, materialConsumption, index);
        //}

        //public IBlockItem GetNewBlockItem(params object[] parameters)
        //{
        //    //var order = (IOrder)parameters[0];
        //    var order = this;
        //    var material = (IMaterial)parameters[0];
        //    var materialConsumption = (decimal)parameters[1];

        //    return _itemsFactory.CreateOrderItem(order, material, materialConsumption);
        //}

        //public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        //{
        //    cmd.Parameters.AddWithValue("CreationDateO", NpgsqlDbType.Timestamp, CreationDate);
        //    cmd.Parameters.AddWithValue("OrderNameO", NpgsqlDbType.Varchar, 50, shortName);
        //    cmd.Parameters.AddWithValue("QuantityO", NpgsqlDbType.Numeric, Quantity);
        //    cmd.Parameters.AddWithValue("SearchNameO", NpgsqlDbType.Varchar, 64, Name);

        //}

        //public async Task AddItemToDataBase()
        //{
        //    var commandText = "INSERT INTO orders (CreationDateO, OrderNameO, QuantityO) VALUES (@CreationDateO, @OrderNameO, @QuantityO) RETURNING IdO;";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteIdReaderAsync(this, AssignIndex, commandText, "Время изготовления данного заказа совпадает с временем другого заказа");

        //    ExceptionEvent?.Invoke();
        //}

        //private void AssignIndex(DbDataRecord item)
        //{
        //    Index = (int)item["IdO"];
        //}

        //public async Task RemoveItemFromDataBase()
        //{
        //    var commandText = $"DELETE FROM orders WHERE ido = {Index}";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД");
        //    ExceptionEvent?.Invoke();
        //}
        private protected override string GetAddItemCommandText()
        {
            return "INSERT INTO orders (CreationDateO, OrderNameO, QuantityO, SearchNameO) VALUES (@CreationDateO, @OrderNameO, @QuantityO, @SearchNameO) RETURNING IdO;";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            AssignParametersToEditCommand(cmd);
            cmd.Parameters.AddWithValue("OrderNameO", NpgsqlDbType.Varchar, 50, ShortName);
            cmd.Parameters.AddWithValue("QuantityO", NpgsqlDbType.Numeric, Quantity);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdO"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T) itemsFactory.CreateOrder(ShortName, CreationDate, Quantity, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE orders SET CreationDateO = @CreationDateO, SearchNameO = @SearchNameO WHERE ido = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            CreationDate = (DateTime)parameters[0];
            Name = string.Join(' ', ShortName, "от", CreationDate.ToString("dd/MM/yyyy"));
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CreationDateO", NpgsqlDbType.Date, CreationDate);
            cmd.Parameters.AddWithValue("SearchNameO", NpgsqlDbType.Varchar, 64, Name);

        }

        private protected override void UndoValues<T>(T copyItem)
        {
            var copyOrder = copyItem as IOrder;
            CreationDate = copyOrder.CreationDate;
            Name = string.Join(' ', ShortName, "от", CreationDate.ToString("dd/MM/yyyy"));
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM orders WHERE ido = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблема с БД";
        }
    }
}
