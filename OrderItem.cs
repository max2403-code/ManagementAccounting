using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace ManagementAccounting
{
    public class OrderItem : IOrderItem
    {
        public event Action ExceptionEvent;
        private ICalculationItem _calculationItem;
        private readonly IDataBase _dataBase;
        private readonly IBlockItemsFactory _blockItemFactory;

        public int OrderId { get; }
        public string Name => _calculationItem.Name;
        public int Index { get; }

        public OrderItem(ICalculationItem calculationItem, int orderId, int index, IDataBase dataBase, IBlockItemsFactory itemsFactory)
        {
            _calculationItem = calculationItem;
            OrderId = orderId;
            Index = index;
            _dataBase = dataBase;
            _blockItemFactory = itemsFactory;
        }

        public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            throw new NotImplementedException();
        }

        public Task AddItemToDataBase()
        {
            throw new NotImplementedException();
        }

        public Task EditItemInDataBase(params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task RemoveItemFromDataBase()
        {
            throw new NotImplementedException();
        }
    }
}
