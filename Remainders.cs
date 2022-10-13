using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting
{
    public class Remainders : IProgramBlock
    {
        public string ItemTypeName { get; }
        public int LengthOfItemsList { get; }
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _blockItemFactory;


        public Remainders(IBlockItemsFactory blockItemFactory, IDataBase dataBase)
        {
            ItemTypeName = "material";
            _dataBase = dataBase;
            _blockItemFactory = blockItemFactory;
            LengthOfItemsList = 5;
        }

        #region Создание новой единицы

        public IBlockItem GetNewBlockItem(params object[] parameters)
        {
            var materialType = (MaterialType)(int)parameters[0];
            var materialName = (string)parameters[1];
            return _blockItemFactory.CreateMaterial(materialType, materialName);
        }

        #endregion


        #region Получение списка единиц текущего блока

        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
        {
            var commandText = $"SELECT * FROM remainders WHERE lower(MaterialNameM) LIKE '%{selectionCriterion[0]}%' ORDER BY MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
            var itemsList = await _dataBase.ExecuteReaderAsync(GetBlockItemFromDataBase, commandText);

            return itemsList;
        }

        public IBlockItem GetBlockItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var index = (int)item["IdM"];
            return _blockItemFactory.CreateMaterial(materialType, materialName, index);
        }

        #endregion
    }
}
