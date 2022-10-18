//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Npgsql;
//using NpgsqlTypes;

//namespace ManagementAccounting
//{
//    public class Order : IOrder
//    {
//        public event Action ExceptionEvent;
//        private string shortName;
//        public string Name { get; }
//        public int Index { get; private set; }
//        public string ItemTypeName { get; }
//        public int LengthOfItemsList { get; }
//        public DateTime CreationDate { get; private set; }
//        public decimal Quantity { get; }

//        private readonly IDataBase _dataBase;
//        private readonly IItemsFactory _itemsFactory;

        

//        public Order(string name, DateTime creationDate, decimal quantity, int index,  IDataBase dataBase, IItemsFactory itemsFactory)
//        {
//            ItemTypeName = "orderitem";
//            CreationDate = creationDate;
//            Quantity = quantity;
//            Index = index;
//            shortName = name;
//            Name = string.Join(' ', name, " от ", CreationDate.ToString("dd/MM/yyyy"));
//            _dataBase = dataBase;
//            _itemsFactory = itemsFactory;
//            LengthOfItemsList = 5;
//        }

//        public async Task<List<IBlockItem>> GetItemsList(int offset, params string[] selectionCriterion)
//        {
//            var commandText = $"SELECT * FROM orderitems, remainders WHERE orderitems.OrderIdOI = {Index} AND orderitems.MaterialIdOI = remainders.IdM ORDER BY remainders.MaterialTypeM, remainders.MaterialNameM OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
//            var itemsList = await _dataBase.ExecuteReaderAsync(GetItemFromDataBase, commandText);

//            return itemsList;
//        }

//        public IBlockItem GetItemFromDataBase(DbDataRecord item)
//        {

//            var order = this;
//            var materialType = (MaterialType)(int)item["MaterialTypeM"];
//            var materialName = (string)item["MaterialNameM"];
//            var unitOfMaterial = (UnitOfMaterial)(int)item["UnitM"];
//            var materialIndex = (int)item["IdM"];

//            var material = _itemsFactory.CreateMaterial(materialType, materialName, unitOfMaterial, materialIndex);
//            var materialConsumption = (decimal)item["QuantityOI"];
//            var index = (int) item["IdOI"];

//            return _itemsFactory.CreateOrderItem(order, material, materialConsumption, index);
//        }

//        public IBlockItem GetNewBlockItem(params object[] parameters)
//        {
//            //var order = (IOrder)parameters[0];
//            var order = this;
//            var material = (IMaterial)parameters[0];
//            var materialConsumption = (decimal)parameters[1];
            
//            return _itemsFactory.CreateOrderItem(order, material, materialConsumption);
//        }

//        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
//        {
//            cmd.Parameters.AddWithValue("CreationDateO", NpgsqlDbType.Timestamp, CreationDate);
//            cmd.Parameters.AddWithValue("OrderNameO", NpgsqlDbType.Varchar, 50, shortName);
//            cmd.Parameters.AddWithValue("QuantityO", NpgsqlDbType.Numeric, Quantity);
//            cmd.Parameters.AddWithValue("SearchNameO", NpgsqlDbType.Varchar, 64, Name);

//        }

//        public async Task AddItemToDataBase()
//        {
//            var commandText = "INSERT INTO orders (CreationDateO, OrderNameO, QuantityO) VALUES (@CreationDateO, @OrderNameO, @QuantityO) RETURNING IdO;";
//            if (ExceptionEvent != null) ExceptionEvent = null;
//            await _dataBase.ExecuteIdReaderAsync(this, AssignIndex, commandText, "Время изготовления данного заказа совпадает с временем другого заказа");

//            ExceptionEvent?.Invoke();
//        }

//        private void AssignIndex(DbDataRecord item)
//        {
//            Index = (int)item["IdO"];
//        }

//        public async Task RemoveItemFromDataBase()
//        {
//            var commandText = $"DELETE FROM orders WHERE ido = {Index}";
//            if (ExceptionEvent != null) ExceptionEvent = null;
//            await _dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Проблема с БД");
//            ExceptionEvent?.Invoke();
//        }
//    }
//}
