using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class Calculation : ICalculation
    {
        public event Action ExceptionEvent;

        private string _name;
        private int _index;
        private string _itemTypeName;
        private IDataBase _dataBase;
        private IBlockItemsFactory _itemsFactory;



        public string Name => _name;
        public int Index => _index;
        public string ItemTypeName => _itemTypeName;
        public int LengthOfItemsList { get; }
        public int LengthOfMaterialsList { get; }


        public Calculation(string calculationName, int index, IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            _name = calculationName;
            _index = index;
            _itemTypeName = "calculationitem";
            _dataBase = dataBase;
            _itemsFactory = itemsFactory;
            LengthOfItemsList = 5;
            LengthOfMaterialsList = 5;
        }

        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CalculationNamec", NpgsqlDbType.Varchar, 50, Name);
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CalculationNamec", NpgsqlDbType.Varchar, 50, Name);
        }

        public async Task AddItemToDataBase()
        {
            var commandText = "INSERT INTO calculations (CalculationNamec) VALUES (@CalculationNamec)";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Калькуляция с таким названием уже существует", AssignParametersToAddCommand);
            ExceptionEvent?.Invoke();
        }

        public async Task EditItemInDataBase(params object[] parameters)
        {
            var copyCalculation = _itemsFactory.CreateCalculation(Name, Index);
            var commandText = $"UPDATE calculations SET CalculationNamec = @CalculationNamec WHERE idc = {Index}";
            
            _name = (string)parameters[0];

            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Калькуляция с таким названием уже существует", AssignParametersToEditCommand);

            if (ExceptionEvent != null)
            {
                _name = copyCalculation.Name;
                ExceptionEvent.Invoke();
            }
        }

        public async Task RemoveItemFromDataBase()
        {
            var commandText = $"DELETE FROM calculations WHERE idc = {Index}";
            if (ExceptionEvent != null) ExceptionEvent = null;
            await _dataBase.ExecuteNonQueryAsync(this, commandText, "Калькуляция используется в наряд-заказе");
            ExceptionEvent?.Invoke();
        }

        
        public IBlockItem GetNewBlockItem(params object[] parameters)
        {
            var material = (IMaterial) parameters[0];
            var quantity = (decimal) parameters[1];
            var calculationId = (int) parameters[2];

            return _itemsFactory.CreateCalculationItem(material, quantity, calculationId);
        }

        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM calculationitems, remainders WHERE calculationitems.CalculationIdci = {Index} AND calculationitems.MaterialIdci = remainders.Idm ORDER BY remainders.MaterialTypem, remainders.MaterialNamem OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypem"];
            var materialName = (string)item["MaterialNamem"];
            var materialIndex = (int)item["Idm"];
            var material = _itemsFactory.CreateMaterial(materialType, materialName, materialIndex);
            var quantity = (decimal)item["Quantityci"];
            var calculationId = (int)item["CalculationIdci"];
            var calculationItemIndex = (int)item["Idci"];

            return _itemsFactory.CreateCalculationItem(material, quantity, calculationId, calculationItemIndex);
        }

        public async Task<List<IMaterial>> GetMaterialsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM remainders WHERE lower(MaterialNamem) LIKE '%{selectionCriterion[0]}%' ORDER BY MaterialNamem OFFSET {offset} ROWS FETCH NEXT {LengthOfMaterialsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(AssignMaterials, commandText);

            return itemsList;
        }

        private IMaterial AssignMaterials(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypem"];
            var materialName = (string)item["MaterialNamem"];
            var index = (int)item["Idm"];

            return _itemsFactory.CreateMaterial(materialType, materialName, index);
        }
    }
}
