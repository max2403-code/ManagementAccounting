using System;
using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class PreOrderCollectionCreator : BlockItemsCollectionCreator
    {
        private IItemsFactory ItemsFactory { get; }

        public PreOrderCollectionCreator(int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exChecker) : base(lengthOfItemsList, dataBase, exChecker)
        {
            ItemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var calculationName = (string)item["CalculationNamec"];
            var calculationId = (int)item["Idc"];
            var calculation = ItemsFactory.CreateCalculation(calculationName, calculationId);
            var quantity = (int)item["QuantityPO"];
            var creationDate = (DateTime)item["CreationDatePO"];
            var index = (int)item["IdPO"];

            return ItemsFactory.CreatePreOrder(calculation, quantity, creationDate, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return $"SELECT * FROM preorders, calculations WHERE lower(preorders.SearchNamePO) LIKE '%{searchCriterion}%' AND preorders.CalculationIdPO = calculations.IdC ORDER BY preorders.SearchNamePO OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
