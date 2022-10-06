//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace ManagementAccounting
//{
//    public class WorkSpace
//    {
//        public event Action ExEvent;
//        public int ShowCountOffset { get; set; }
//        public List<IObjectable> ShowListItems { get; }
//        private OperationsDB dataBase { get; set; }

//        private static readonly WorkSpace workSpace = new WorkSpace();

//        private WorkSpace()
//        {
//            ShowListItems = new List<IObjectable>();
//        }

//        public bool CheckNextOffset()
//        {
//            var indexOfLastItem = FactoryClass.GetLengthOfItemList();
//            var lengthOfItemList = indexOfLastItem + 1;

//            if (ShowListItems.Count != lengthOfItemList) return false;
//            ShowListItems.RemoveAt(indexOfLastItem);
//            return true;
//        }

//        public bool CheckPreviousOffset()
//        {
//            return ShowCountOffset > 0;
//        }

//        public async Task FillItemsList(string searchName, TypeOfItem blockType)
//        {
//            var cmdText = FactoryClass.GetCommandTextToGetItems(ShowCountOffset, searchName, blockType);
//            await dataBase.GetShowItemsAsync(cmdText, blockType);
//        }

//        public void CreateDB(string login, string password)
//        {
//            dataBase = new OperationsDB(login, password);
//        }

//        public async Task AddMaterial(MaterialType materialType, string materialName)
//        {
//            var material = new Material(materialType, materialName, 0, -1);
//            var cmdText = FactoryClass.GetCommandTextToAddItems(TypeOfItem.Material);
//            if(ExEvent != null) ExEvent = null;
//            await dataBase.AddNewItemAsync(cmdText, material, "Материал с таким же наименованием уже существует");
//            ExEvent?.Invoke();
//        }

//        public async Task RemoveMaterial(Material material)
//        {
//            var cmdText = FactoryClass.GetCommandTextToRemoveItems(material);
//            if (ExEvent != null) ExEvent = null;
//            await dataBase.RemoveUpdateItemWithoutParametersAsync(cmdText, "Материал используется в калькуляциях себестоимости");
//            ExEvent?.Invoke();
//        }

//        public async Task EditMaterial(Material material, string value)
//        {
//            var cmdText = FactoryClass.GetCommandTextToUpdateItems(material, value);
//            if (ExEvent != null) ExEvent = null;
//            await dataBase.RemoveUpdateItemWithoutParametersAsync(cmdText, "Материал с таким же наименованием уже существует");
//            ExEvent?.Invoke();
//        }

//        public static WorkSpace GetWorkSpace()
//        {
//            return workSpace;
//        }

        
//    }
//}
