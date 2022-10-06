using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class CalculationItem : ICalculationItem
    {
        public event Action ExceptionEvent;
        private int _index;
        private IMaterial _material;
        private int _calculationId;
        private decimal _quantity;
        private IDataBase _dataBase;
        private IBlockItemsFactory _itemsFactory;

        public int CalculationId => _calculationId;
        public int MaterialId => _material.Index;
        public decimal Quantity => _quantity;
        public string Name => _material.Name;
        public int Index => _index;

        public CalculationItem(IMaterial material,  decimal quantity, int calculationId, int index,  IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            _material = material;
            _calculationId = calculationId;
            _quantity = quantity;
            _index = index;
            _dataBase = dataBase;
            _itemsFactory = itemsFactory;
        }

        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("MaterialIdci", NpgsqlDbType.Integer, MaterialId);
            cmd.Parameters.AddWithValue("CalculationIdci", NpgsqlDbType.Integer, CalculationId);
            cmd.Parameters.AddWithValue("Quantityci", NpgsqlDbType.Numeric, Quantity);
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("Quantityci", NpgsqlDbType.Numeric, Quantity);
        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO calculationitems (CalculationIdci, MaterialIdci, Quantityci) VALUES (@CalculationIdci, @MaterialIdci, @Quantityci)";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Введены некорректные данные", AssignParametersToAddCommand);
            ExceptionEvent?.Invoke();
        }

        public async Task EditItemInDataBase(params object[] parameters)
        {
            var copyCalculationItem = _itemsFactory.CreateCalculationItem(_material, Quantity, CalculationId, Index);
            var commandText = $"UPDATE calculationitems SET Quantityci = @Quantityci WHERE idci = {Index}";

            _quantity = (decimal)parameters[0];

            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Введены некорректные данные", AssignParametersToEditCommand);

            if (ExceptionEvent != null)
            {
                _quantity = copyCalculationItem.Quantity;
                ExceptionEvent.Invoke();
            }
        }

        public async Task RemoveItemFromDataBase()
        {
            var commandText = $"DELETE FROM calculationitems WHERE idci = {Index}";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Проблемы с БД");
            ExceptionEvent?.Invoke();
        }
    }
}
