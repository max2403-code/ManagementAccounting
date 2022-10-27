﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Classes.ItemCreators
{
    public class OrderItemCollectionCreatorForOrders : BlockItemsCollectionCreator
    {
        private IItemsFactory itemsFactory { get; }
        private IOrderItem orderItem { get; }


        public OrderItemCollectionCreatorForOrders(IOrderItem orderItem, int lengthOfItemsList, IDataBase dataBase, IItemsFactory itemsFactory) : base(lengthOfItemsList, dataBase)
        {
            this.orderItem = orderItem;
            this.itemsFactory = itemsFactory;
        }

        private protected override IBlockItem GetItemFromDataBase(DbDataRecord item)
        {
            var shortName = (string)item["OrderNameO"];
            var quantity = (int)item["QuantityO"];
            var creationDate = (DateTime)item["CreationDateO"];
            var orderIndex = (int)item["IdO"];

            var newOrder = itemsFactory.CreateOrder(shortName, creationDate, quantity, orderIndex);
            var consumption = (decimal)item["ConsumptionOI"];
            var totalConsumption = (decimal)item["TotalConsumptionOI"];

            var index = (int)item["IdOI"];

            return itemsFactory.CreateOrderItem(newOrder, orderItem.Material, consumption, totalConsumption, index);
        }

        private protected override string GetCommandText(int offset, string searchCriterion) // searchCriterion ">" or ">="
        {
            return $"SELECT * FROM orderitems AS oi, orders AS o WHERE o.CreationDateO {searchCriterion} '{orderItem.Order.CreationDate.ToString("dd/MM/yyyy")}' AND oi.MaterialIdOI = {orderItem.Material.Index} AND oi.OrderIdOI = o.IdO ORDER BY o.CreationDateO, oi.idoi OFFSET {offset} ROWS FETCH NEXT {LengthOfItemsList + 1} ROWS ONLY;";
        }
    }
}