//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Text;
//using System.Threading.Tasks;

//namespace ManagementAccounting
//{
//    public class Calculations : IProgramBlock
//    {
//        public string ItemTypeName { get; }
//        public int LengthOfItemsList { get; }
//        private readonly IDataBase _dataBase;
//        private readonly IItemsFactory _itemFactory;

//        public Calculations(IItemsFactory itemFactory, IDataBase dataBase)
//        {
//            ItemTypeName = "calculation";
//            _dataBase = dataBase;
//            _itemFactory = itemFactory;
//            LengthOfItemsList = 5;
//        }

//        public IBlockItem GetNewBlockItem(params object[] parameters)
//        {
//            var calculationName = (string)parameters[0];
//            return _itemFactory.CreateCalculation(calculationName);
//        }

//        //public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
//        //{
//        //    var commandText = $"SELECT * FROM calculations WHERE lower(CalculationNamec) LIKE '%{selectionCriterion[0]}%' ORDER BY CalculationNamec OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
//        //    var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

//        //    return itemsList;
//        //}

//        public IBlockItem GetItemFromDataBase(DbDataRecord item)
//        {
//            var calculationName = (string)item["CalculationNamec"];
//            var index = (int)item["Idc"];
//            return _itemFactory.CreateCalculation(calculationName, index);
//        }
//    }
//}
