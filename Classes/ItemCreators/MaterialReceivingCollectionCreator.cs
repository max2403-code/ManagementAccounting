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
        private IMaterial material { get; }
        private IItemsFactory itemsFactory { get; }
        
        public MaterialReceivingCollectionCreator(IMaterial material, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
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

            return itemsFactory.CreateMaterialReceiving(material, date, quantity, cost, remainder, note, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {material.Index} AND SearchNamemr LIKE '%{searchCriterion}%' ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";

            //return $"SELECT * FROM materialreceiving AS mr, materials AS m WHERE mr.MaterialIdmr = {material.Index} AND mr.SearchNamemr LIKE '%{searchCriterion}%' AND mr.MaterialIdmr = m.IdM ORDER BY ReceiveDatemr OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
