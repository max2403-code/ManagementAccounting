﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Classes.Common
{
    public class OrderItemCostPrice : IOrderItemCostPrice
    {
        private ICreatorFactory creatorFactory { get; }
        public OrderItemCostPrice(ICreatorFactory creatorFactory)
        {
            this.creatorFactory = creatorFactory;
        }
        public async Task<decimal> GetOrderItemCostPrice(IOrderItem orderItem)
        {
            var orderItemCostPrice = 0m;
            var orderMaterialReceivingCreator =
                creatorFactory.CreateOrderMaterialReceivingCollectionCreator(orderItem, 5);
            var offset = 0;

            while (true)
            {
                var resultOfGettingItemsList = await orderMaterialReceivingCreator.GetItemsList(offset, "");
                var itemsList = resultOfGettingItemsList.Item1;
                var isThereMoreOfItems = resultOfGettingItemsList.Item2;
                
                offset += orderMaterialReceivingCreator.LengthOfItemsList;

                foreach (var orderMaterialReceiving in itemsList.Cast<IOrderMaterialReceiving>())
                {
                    var materialReceiving = orderMaterialReceiving.MaterialReceiving;
                    orderItemCostPrice += orderMaterialReceiving.Consumption * materialReceiving.Price;
                }
                if (!isThereMoreOfItems) break;
            }

            return orderItemCostPrice;
        }
    }
}
