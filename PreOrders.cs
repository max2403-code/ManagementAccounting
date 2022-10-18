//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Text;
//using System.Threading.Tasks;

//namespace ManagementAccounting
//{
//    public class PreOrders : IPreOrders
//    {
//        public string ItemTypeName { get; }
//        public int LengthOfItemsList { get; }
//        private readonly IDataBase _dataBase;
//        private readonly IItemsFactory _itemFactory;

//        public PreOrders(IItemsFactory itemFactory, IDataBase dataBase)
//        {
//            ItemTypeName = "preorder";
//            _dataBase = dataBase;
//            _itemFactory = itemFactory;
//            LengthOfItemsList = 5;
//        }
        
//        public IBlockItem GetNewBlockItem(params object[] parameters)
//        {
//            var calculation = (ICalculation)parameters[0];
//            var quantity = (decimal) parameters[1];
//            var creationDate = (DateTime) parameters[2]; 

//            return _itemFactory.CreatePreOrder(calculation, quantity, creationDate);
//        }

//        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
//        {
//            var commandText = $"SELECT * FROM preorders, calculations WHERE lower(preorders.SearchNamePO) LIKE '%{selectionCriterion[0]}%' AND preorders.CalculationIdPO = calculations.IdC ORDER BY preorders.SearchNamePO OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
//            var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

//            return itemsList;
//        }

//        public IBlockItem GetItemFromDataBase(DbDataRecord item)
//        {
//            var calculationName = (string)item["CalculationNamec"];
//            var calculationId = (int)item["Idc"];
//            var calculation = _itemFactory.CreateCalculation(calculationName, calculationId);
//            var quantity = (decimal)item["QuantityPO"];
//            var creationDate = (DateTime)item["CreationDatePO"];
//            var index = (int) item["IdPO"];

//            return _itemFactory.CreatePreOrder(calculation, quantity, creationDate, index);
//        }

//    }
//}
