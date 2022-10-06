using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class MaterialReceiving : IMaterialReceiving
    {
        private string _name;
        private int _index;
        private int _materialIndex;
        private DateTime _date;
        private decimal _quantity;
        private decimal _cost;
        private decimal _remainder;
        private string _note;
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _itemsFactory;

        public event Action ExceptionEvent;
        public string Name => _name;
        public int Index => _index;
        public int MaterialIndex => _materialIndex;
        public DateTime Date => _date;
        public decimal Quantity => _quantity;
        public decimal Cost => _cost;
        public decimal Price => Math.Round(Cost / Quantity, 2);
        public decimal Remainder => _remainder;
        public string Note => _note;


        public MaterialReceiving(DateTime date, decimal quantity, decimal cost, decimal remainder, string note, int materialIndex, int index, IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            _index = index;
            _materialIndex = materialIndex;
            _date = date;
            _quantity = quantity;
            _cost = cost;
            _remainder = remainder;
            _note = note;
            _dataBase = dataBase;
            _itemsFactory = itemsFactory;
            _name = $"Поступление от {Date.ToString("dd/MM/yyyy")}";
        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO materialreceiving (MaterialIdmr, Quantitymr, ReceiveDatemr, TotalCostmr, Remaindermr, Notemr, SearchNamemr) VALUES (@MaterialIdmr, @Quantitymr, @ReceiveDatemr, @TotalCostmr, @Remaindermr, @Notemr, @SearchNamemr)";
            if (ExceptionEvent != null) ExceptionEvent = null;
            //await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблемы с БД", true, "add");
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблемы с БД", AssignParametersToAddCommand);

            ExceptionEvent?.Invoke();
        }

        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("MaterialIdmr", NpgsqlDbType.Integer, MaterialIndex);
            cmd.Parameters.AddWithValue("Quantitymr", NpgsqlDbType.Numeric, Quantity);
            cmd.Parameters.AddWithValue("ReceiveDatemr", NpgsqlDbType.Date, Date);
            cmd.Parameters.AddWithValue("TotalCostmr", NpgsqlDbType.Numeric, Cost);
            cmd.Parameters.AddWithValue("Remaindermr", NpgsqlDbType.Numeric, Remainder);
            cmd.Parameters.AddWithValue("Notemr", NpgsqlDbType.Varchar, 50, Note);
            cmd.Parameters.AddWithValue("SearchNamemr", NpgsqlDbType.Varchar, 10, Date.ToString("dd/MM/yyyy"));
        }

        public async Task EditItemInDataBase(params object[] parameters)
        {
            var copyMaterialReceiving = _itemsFactory.CreateMaterialReceiving(Date, Quantity, Cost, Remainder, Note, MaterialIndex, Index);
            var commandText = $"UPDATE materialreceiving SET Quantitymr = @Quantitymr, ReceiveDatemr = @ReceiveDatemr, TotalCostmr = @TotalCostmr, Remaindermr = @Remaindermr, Notemr = @Notemr, SearchNamemr = @SearchNamemr WHERE idmr = {Index}";

            _date = (DateTime)parameters[0];
            _quantity = (decimal)parameters[1];
            _cost = (decimal)parameters[2];
            _remainder = (decimal)parameters[3];
            _note = (string)parameters[4];
            _name = $"Поступление от {Date.ToString("dd/MM/yyyy")}";


            if (ExceptionEvent != null) ExceptionEvent = null;
            //await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблемы с БД", true, "edit");
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблемы с БД", AssignParametersToEditCommand);

            if (ExceptionEvent != null)
            {
                _date = copyMaterialReceiving.Date;
                _quantity = copyMaterialReceiving.Quantity;
                _cost = copyMaterialReceiving.Cost;
                _remainder = copyMaterialReceiving.Remainder;
                _note = copyMaterialReceiving.Note;
                _name = copyMaterialReceiving.Name;
                ExceptionEvent.Invoke();
            }
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("Quantitymr", NpgsqlDbType.Numeric, Quantity);
            cmd.Parameters.AddWithValue("ReceiveDatemr", NpgsqlDbType.Date, Date);
            cmd.Parameters.AddWithValue("TotalCostmr", NpgsqlDbType.Numeric, Cost);
            cmd.Parameters.AddWithValue("Remaindermr", NpgsqlDbType.Numeric, Remainder);
            cmd.Parameters.AddWithValue("Notemr", NpgsqlDbType.Varchar, 50, Note);
            cmd.Parameters.AddWithValue("SearchNamemr", NpgsqlDbType.Varchar, 10, Date.ToString("dd/MM/yyyy"));
        }

        public async Task RemoveItemFromDataBase()
        {
            var commandText = $"DELETE FROM materialreceiving WHERE idmr = {Index}";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблемы с БД");
            ExceptionEvent?.Invoke();
        }
    }
}
