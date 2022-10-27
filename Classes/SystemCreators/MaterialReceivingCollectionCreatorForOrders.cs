using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class MaterialReceivingCollectionCreatorForOrders : MaterialReceivingCollectionCreator
    {
        private IOrderItem orderItem { get; }
        public MaterialReceivingCollectionCreatorForOrders(IOrderItem orderItem, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(orderItem.Material, lengthOfItemsList, dataBase, itemsFactory)
        {
            this.orderItem = orderItem;
        }

        private protected override string GetCommandText(int offset, string searchCriterion)
        {
            return
                $"SELECT * FROM materialreceiving WHERE MaterialIdmr = {orderItem.Material.Index} AND Remaindermr > 0 AND ReceiveDateMR <= '{orderItem.Order.CreationDate.ToString("dd/MM/yyyy")}' ORDER BY ReceiveDatemr, IdMR OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}
