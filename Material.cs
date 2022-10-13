using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class Material : IMaterial
    {
        private MaterialType _materialType;
        private string _name;
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _itemsFactory;

        public event Action ExceptionEvent;
        public int Index { get; }
        public MaterialType MaterialType => _materialType;
        public string Name => _name;
        public string ItemTypeName { get; }
        public int LengthOfItemsList { get; }

        public Material(MaterialType materialType, string name, int index, IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            Index = index;
            _materialType = materialType;
            _name = name;
            _dataBase = dataBase;
            ItemTypeName = "materialflow";
            _itemsFactory = itemsFactory;
            LengthOfItemsList = 5;
        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO remainders (materialtypem, materialnamem) VALUES (@materialtypem, @materialnamem)";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Материал с таким именем уже существует", AssignParametersToAddCommand);

            ExceptionEvent?.Invoke();
        }

        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("materialtypem", NpgsqlDbType.Integer, (int)MaterialType);
            cmd.Parameters.AddWithValue("materialnamem", NpgsqlDbType.Varchar, 50, Name);
        }

        public async Task EditItemInDataBase(params object[] parameters)
        {
            var copyMaterial = _itemsFactory.CreateMaterial(MaterialType, Name, Index);
            var commandText = $"UPDATE remainders SET materialtypem = @materialtypem, materialnamem = @materialnamem WHERE idm = {Index}";

            _materialType = (MaterialType) parameters[0];
            _name = (string) parameters[1];
            
            if (ExceptionEvent != null) ExceptionEvent = null;
            //await _dataBase.ExecuteNonQueryAsync(this, commandText, "Материал с таким именем уже существует", true, "edit");
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Материал с таким именем уже существует", AssignParametersToEditCommand);

            if (ExceptionEvent != null)
            {
                _materialType = copyMaterial.MaterialType;
                _name = copyMaterial.Name;
                ExceptionEvent.Invoke();
            }
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("materialtypem", NpgsqlDbType.Integer, (int)MaterialType);
            cmd.Parameters.AddWithValue("materialnamem", NpgsqlDbType.Varchar, 50, Name);
        }

        public async Task RemoveItemFromDataBase()
        {
            var commandText = $"DELETE FROM remainders WHERE idm = {Index}";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Материал используется в калькуляции");
            ExceptionEvent?.Invoke();
        }

        public IBlockItem GetNewBlockItem(params object[] parameters)
        {
            var date = (DateTime)parameters[0];
            var quantity = (decimal) parameters[1];
            var cost = (decimal) parameters[2];
            var remainder = (decimal)parameters[3];
            var note = (string) parameters[4];
            var materialIndex = (int) parameters[5];

            return _itemsFactory.CreateMaterialReceiving(date, quantity, cost, remainder, note, materialIndex);
        }

        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = selectionCriterion.Length == 2 
                ? $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {Index} AND SearchNamemr LIKE '%{selectionCriterion[1]}%' ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;"
                    : $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {Index} AND Remaindermr > 0 AND SearchNamemr LIKE '%{selectionCriterion[0]}%' ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            //var itemsList = await _dataBase.ExecuteReaderAsync(this, commandText);
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var date = (DateTime)item["ReceiveDatemr"];
            var quantity = (decimal)item["Quantitymr"];
            var cost = (decimal)item["TotalCostmr"];
            var remainder = (decimal)item["Remaindermr"];
            var note = (string)item["Notemr"];
            var materialIndex = (int)item["MaterialIdmr"];
            var index = (int)item["Idmr"];

            return _itemsFactory.CreateMaterialReceiving(date, quantity, cost, remainder, note, materialIndex, index);
        }
    }
}
