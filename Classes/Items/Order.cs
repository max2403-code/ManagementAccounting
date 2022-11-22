using System;
using System.Data.Common;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
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
        private IItemsFactory ItemsFactory { get; }

        public Order(string shortName, DateTime creationDate, int quantity, int index, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Index = index;
            Name = string.Join(' ', shortName, "от", creationDate.ToString("dd/MM/yyyy"));
            ShortName = shortName;
            CreationDate = creationDate;
            Quantity = quantity;
            ItemsFactory = itemsFactory;
        }

        
        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
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
            return (T) ItemsFactory.CreateOrder(ShortName, CreationDate, Quantity, Index);
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
        public async Task Update()
        {
            var cmdText = $"SELECT * FROM orders WHERE ido = {Index}";
            ExceptionChecker.IsExceptionHappened = false;
            await DataBase.ExecuteUpdaterAsync(ExceptionChecker, UpdateOrderFromDataBase, cmdText);
            if (ExceptionChecker.IsExceptionHappened) ExceptionChecker.DoException("Проблема с БД");
        }

        private void UpdateOrderFromDataBase(DbDataRecord item)
        {
            CreationDate = (DateTime)item["CreationDateO"];
            Name = string.Join(' ', ShortName, "от", CreationDate.ToString("dd/MM/yyyy"));
        }
    }
}
