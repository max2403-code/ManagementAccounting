using System;
using System.Data.Common;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class MaterialReceiving : EditingBlockItemDB, IMaterialReceiving
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public IMaterial Material { get; }
        public DateTime Date { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Cost { get; private set; }
        public decimal Price => Math.Round(Cost / Quantity, 2);
        public decimal Remainder { get; private set; }
        public string Note { get; private set; }
        private IItemsFactory ItemsFactory { get; }

        public MaterialReceiving(IMaterial material, DateTime date, decimal quantity, decimal cost, decimal remainder, string note, int index, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Name = $"Поступление от {date.ToString("dd/MM/yyyy")}";
            Index = index;
            Material = material;
            Date = date;
            Quantity = quantity;
            Cost = cost;
            Remainder = remainder;
            Note = note;
            ItemsFactory = itemsFactory;
        }

        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return isPreviouslyExistingItem 
                ? "INSERT INTO materialreceiving VALUES (@IdMR, @MaterialIdmr, @Quantitymr, @ReceiveDatemr, @TotalCostmr, @Remaindermr, @Notemr, @SearchNamemr)"
                : "INSERT INTO materialreceiving (MaterialIdmr, Quantitymr, ReceiveDatemr, TotalCostmr, Remaindermr, Notemr, SearchNamemr) VALUES (@MaterialIdmr, @Quantitymr, @ReceiveDatemr, @TotalCostmr, @Remaindermr, @Notemr, @SearchNamemr) RETURNING Idmr";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблемы с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            if(Index != -1) cmd.Parameters.AddWithValue("IdMR", NpgsqlDbType.Integer, Index);
            cmd.Parameters.AddWithValue("MaterialIdmr", NpgsqlDbType.Integer, Material.Index);
            AssignParametersToEditCommand(cmd);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdMR"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T) ItemsFactory.CreateMaterialReceiving(Material, Date, Quantity, Cost, Remainder, Note, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE materialreceiving SET Quantitymr = @Quantitymr, ReceiveDatemr = @ReceiveDatemr, TotalCostmr = @TotalCostmr, Remaindermr = @Remaindermr, Notemr = @Notemr, SearchNamemr = @SearchNamemr WHERE idmr = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            Date = (DateTime)parameters[0];
            Quantity = (decimal)parameters[1];
            Cost = (decimal)parameters[2];
            Remainder = (decimal)parameters[3];
            Note = (string)parameters[4];
            Name = $"Поступление от {Date.ToString("dd/MM/yyyy")}";
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Проблемы с БД";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("Quantitymr", NpgsqlDbType.Numeric, Quantity);
            cmd.Parameters.AddWithValue("ReceiveDatemr", NpgsqlDbType.Date, Date);
            cmd.Parameters.AddWithValue("TotalCostmr", NpgsqlDbType.Numeric, Cost);
            cmd.Parameters.AddWithValue("Remaindermr", NpgsqlDbType.Numeric, Remainder);
            cmd.Parameters.AddWithValue("Notemr", NpgsqlDbType.Varchar, 50, Note);
            cmd.Parameters.AddWithValue("SearchNamemr", NpgsqlDbType.Varchar, 10, Date.ToString("dd/MM/yyyy"));
        }

        private protected override void UndoValues<T>(T copyItem)
        {
            var copyMaterialReceiving = copyItem as IMaterialReceiving;
            Date = copyMaterialReceiving.Date;
            Quantity = copyMaterialReceiving.Quantity;
            Cost = copyMaterialReceiving.Cost;
            Remainder = copyMaterialReceiving.Remainder;
            Note = copyMaterialReceiving.Note;
            Name = copyMaterialReceiving.Name;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM materialreceiving WHERE idmr = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблемы с БД";
        }

        public async Task Update()
        {
            var cmdText = $"SELECT * FROM materialreceiving WHERE idmr = {Index}";
            ExceptionChecker.IsExceptionHappened = false;
            await DataBase.ExecuteUpdaterAsync(ExceptionChecker, UpdateMaterialReceivingFromDataBase, cmdText);
            if (ExceptionChecker.IsExceptionHappened) ExceptionChecker.DoException("Проблема с БД");
        }

        private void UpdateMaterialReceivingFromDataBase(DbDataRecord item)
        {
            Date = (DateTime)item["ReceiveDatemr"];
            Quantity = (decimal)item["Quantitymr"];
            Cost = (decimal)item["TotalCostmr"];
            Remainder = (decimal)item["Remaindermr"];
            Note = (string)item["Notemr"];
            Name = $"Поступление от {Date.ToString("dd/MM/yyyy")}";
        }
    }
}
