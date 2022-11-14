using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingCollectionCreatorForOrders : MaterialReceivingCollectionCreator
    {
        private IOrderItem OrderItem { get; }
        public MaterialReceivingCollectionCreatorForOrders(IOrderItem orderItem, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(orderItem.Material, lengthOfItemsList, dataBase, itemsFactory)
        {
            OrderItem = orderItem;
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {OrderItem.Material.Index} AND Remaindermr > 0 AND ReceiveDateMR <= '{OrderItem.Order.CreationDate.ToString("dd/MM/yyyy")}' ORDER BY ReceiveDatemr, IdMR OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
