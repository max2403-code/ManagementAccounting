using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class PreOrderCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }

        public PreOrderCollectionCreator(int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.itemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var calculationName = (string)item["CalculationNamec"];
            var calculationId = (int)item["Idc"];
            var calculation = itemsFactory.CreateCalculation(calculationName, calculationId);
            var quantity = (int)item["QuantityPO"];
            var creationDate = (DateTime)item["CreationDatePO"];
            var index = (int)item["IdPO"];

            return itemsFactory.CreatePreOrder(calculation, quantity, creationDate, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM preorders, calculations WHERE lower(preorders.SearchNamePO) LIKE '%{searchCriterion}%' AND preorders.CalculationIdPO = calculations.IdC ORDER BY preorders.SearchNamePO OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
