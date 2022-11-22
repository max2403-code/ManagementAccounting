using System;
using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Items;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class PreOrder : EditingBlockItemDB, IPreOrder 
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public DateTime CreationDate { get; private set; }
        public ICalculation Calculation { get; }
        public int Quantity { get; private set; }
        private IItemsFactory ItemsFactory { get; }

        public PreOrder(ICalculation calculation, int quantity, DateTime creationDate, int index, IItemsFactory itemsFactory, IDataBase dataBase, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Index = index;
            Name = string.Join(' ', calculation.Name, "от", creationDate.ToString("dd/MM/yyyy"));
            Quantity = quantity;
            Calculation = calculation;
            CreationDate = creationDate;
            Index = index;
            ItemsFactory = itemsFactory;
        }

        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return "INSERT INTO preorders (CalculationIdPO, QuantityPO, CreationDatePO, SearchNamePO) VALUES (@CalculationIdPO, @QuantityPO, @CreationDatePO, @SearchNamePO) RETURNING IdPO";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CalculationIdPO", NpgsqlDbType.Integer, Calculation.Index);
            AssignParametersToEditCommand(cmd);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdPO"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T) ItemsFactory.CreatePreOrder(Calculation, Quantity, CreationDate, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE preorders SET QuantityPO = @QuantityPO, CreationDatePO = @CreationDatePO, SearchNamePO = @SearchNamePO WHERE idpo = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            Quantity = (int)parameters[0];
            CreationDate = (DateTime)parameters[1];
            Name = string.Join(' ', Calculation.Name, "от", CreationDate.ToString("dd/MM/yyyy"));
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Проблема с БД";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("QuantityPO", NpgsqlDbType.Integer, Quantity);
            cmd.Parameters.AddWithValue("CreationDatePO", NpgsqlDbType.Date, CreationDate);
            cmd.Parameters.AddWithValue("SearchNamePO", NpgsqlDbType.Varchar, 64, Name);
        }

        private protected override void UndoValues<T>(T copyItem)
        {
            var copyPreOrder = copyItem as IPreOrder;
            Quantity = copyPreOrder.Quantity;
            CreationDate = copyPreOrder.CreationDate;
            Name = copyPreOrder.Name;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM preorders WHERE idpo = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблема с БД";
        }
    }
}
