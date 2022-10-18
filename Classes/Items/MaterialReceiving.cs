using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using ManagementAccounting.Classes.Abstract;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class MaterialReceiving : BlockItemDB, IMaterialReceiving
    {
        public IMaterial Material { get; }
        public DateTime Date { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Cost { get; private set; }
        public decimal Price => Math.Round(Cost / Quantity, 2);
        public decimal Remainder { get; private set; }
        public string Note { get; private set; }
        private IItemsFactory itemsFactory { get; }

        public MaterialReceiving(IMaterial material, DateTime date, decimal quantity, decimal cost, decimal remainder, string note, int index, IDataBase dataBase, IItemsFactory itemsFactory)
            : base(index, $"Поступление от {date.ToString("dd/MM/yyyy")}", dataBase)
        {
            Material = material;
            Date = date;
            Quantity = quantity;
            Cost = cost;
            Remainder = remainder;
            Note = note;
            this.itemsFactory = itemsFactory;
        }

        private protected override string GetAddItemCommandText()
        {
            return "INSERT INTO materialreceiving (MaterialIdmr, Quantitymr, ReceiveDatemr, TotalCostmr, Remaindermr, Notemr, SearchNamemr) VALUES (@MaterialIdmr, @Quantitymr, @ReceiveDatemr, @TotalCostmr, @Remaindermr, @Notemr, @SearchNamemr) RETURNING Idmr";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Проблемы с БД";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("MaterialIdmr", NpgsqlDbType.Integer, ((BlockItemDB)Material).Index);
            AssignParametersToEditCommand(cmd);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdMR"];
        }

        private protected override IBlockItem GetCopyItem()
        {
            return itemsFactory.CreateMaterialReceiving(Material, Date, Quantity, Cost, Remainder, Note, Index);
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
            Note = (string)parameters[3];
            Name = $"Поступление от {Date.ToString("dd/MM/yyyy")}";
            Remainder = Quantity;
        }

        private protected override string GetEditExceptionMessage()
        {
            return $"UPDATE materialreceiving SET Quantitymr = @Quantitymr, ReceiveDatemr = @ReceiveDatemr, TotalCostmr = @TotalCostmr, Remaindermr = @Remaindermr, Notemr = @Notemr, SearchNamemr = @SearchNamemr WHERE idmr = {Index}";
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

        private protected override void UndoValues(IBlockItem copyItem)
        {
            var copyMaterialReceiving = copyItem as IMaterialReceiving;
            Date = copyMaterialReceiving.Date;
            Quantity = copyMaterialReceiving.Quantity;
            Cost = copyMaterialReceiving.Cost;
            Remainder = copyMaterialReceiving.Remainder;
            Note = copyMaterialReceiving.Note;
            Name = ((BlockItemDB)copyMaterialReceiving).Name;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM materialreceiving WHERE idmr = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблемы с БД";
        }
    }
}
