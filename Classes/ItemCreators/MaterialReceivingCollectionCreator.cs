using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.ItemCreators;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingCollectionCreator : BlockItemsCollectionCreator
    {
        private BlockItemDB material { get; }
        private IItemsFactory itemsFactory { get; }
        
        public MaterialReceivingCollectionCreator(BlockItemDB material, IDataBase dataBase, IItemsFactory itemsFactory) : base(5, dataBase)
        {
            this.itemsFactory = itemsFactory;
            this.material = material;
        }
        
        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var date = (DateTime)item["ReceiveDatemr"];
            var quantity = (decimal)item["Quantitymr"];
            var cost = (decimal)item["TotalCostmr"];
            var remainder = (decimal)item["Remaindermr"];
            var note = (string)item["Notemr"];
            var index = (int)item["Idmr"];

            return itemsFactory.CreateMaterialReceiving((IMaterial)material, date, quantity, cost, remainder, note, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving AS mr, materials AS m WHERE mr.MaterialIdmr = {material.Index} AND mr.SearchNamemr LIKE '%{searchCriterion}%' AND mr.MaterialIdmr = m.IdM ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
