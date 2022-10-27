﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.ItemCreators;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }

        public MaterialCollectionCreator(int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.itemsFactory = itemsFactory;
        }
       
        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var materialType = (MaterialType)(int)item["MaterialTypeM"];
            var materialName = (string)item["MaterialNameM"];
            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
            var index = (int)item["IdM"];

            return itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM materials WHERE lower(MaterialNameM) LIKE '%{searchCriterion}%' ORDER BY MaterialTypeM, MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
