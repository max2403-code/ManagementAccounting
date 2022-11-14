using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.Common;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Forms.OrdersForms
{
    public partial class OrderForm : Form
    {
        private Label NameValue { get; }
        private Label Quantity { get; }
        private Label Date { get; }
        private Label UnitPrice { get; }
        private Label Price { get; }
        private Label TableColumnName { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button CalculateItemButton { get; }
        private Button EditOrderButton { get; }
        private Button RemoveOrderButton { get; }
        private List<Control> ActiveItemTempControls { get; }
        private IOrder Order { get; set; }
        private IFormFactory FormFactory { get; }
        private BlockItemsCollectionCreator Creator { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IOrderCostPrice OrderCostPrice { get; }
        private IOrderItemCostPrice OrderItemCostPrice { get; }
        private ISystemOrderOperations OrderOperations { get; }
        private IFromPreOrderToOrderConverter Converter { get; }



        public OrderForm(IOrder order, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory, IOrderCostPrice orderCostPrice, IOrderItemCostPrice orderItemCostPrice, ISystemOrderOperations orderOperations, IFromPreOrderToOrderConverter converter)
        {
            Converter = converter;
            OrderOperations = orderOperations;
            OrderItemCostPrice = orderItemCostPrice;
            OrderCostPrice = orderCostPrice;
            Order = order;
            InputOperations = inputOperations;
            FormFactory = formFactory;
            Creator = creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            ActiveItemTempControls = new List<Control>();

            Size = new Size(900, 600);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Заказ";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Text = "Наименование:";
            nameLabel.Width = 150;
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 20);
            Controls.Add(nameLabel);

            NameValue = new Label();
            NameValue.Text = order.Name;
            NameValue.Width = 200;
            NameValue.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            Controls.Add(NameValue);

            var quantityLabel = new Label();
            quantityLabel.Text = "Количество изделий:";
            quantityLabel.Width = 150;
            quantityLabel.Location = new Point(10, nameLabel.Location.Y + nameLabel.Height + 20);
            Controls.Add(quantityLabel);

            Quantity = new Label();
            Quantity.Text = string.Join(' ', order.Quantity.ToString(), inputOperations.TranslateType(UnitOfMaterial.pcs.ToString()));
            Quantity.AutoSize = true;
            Quantity.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            Controls.Add(Quantity);

            var dateLabel = new Label();
            dateLabel.Text = "Дата создания:";
            dateLabel.Width = 150;
            dateLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 20);
            Controls.Add(dateLabel);

            Date = new Label();
            Date.Text = order.CreationDate.ToString("dd/MM/yyyy");
            Date.Width = 200;
            Date.Location = new Point(dateLabel.Location.X + dateLabel.Width + 10, dateLabel.Location.Y);
            Controls.Add(Date);

            var unitPriceLabel = new Label();
            unitPriceLabel.Text = "Себестоимость ед.:";
            unitPriceLabel.Width = 150;
            unitPriceLabel.Location = new Point(10, dateLabel.Location.Y + dateLabel.Height + 20);
            Controls.Add(unitPriceLabel);

            UnitPrice = new Label();
            UnitPrice.Width = 100;
            UnitPrice.AutoSize = true;
            UnitPrice.Location = new Point(unitPriceLabel.Location.X + unitPriceLabel.Width + 10, unitPriceLabel.Location.Y);
            Controls.Add(UnitPrice);

            var priceLabel = new Label();
            priceLabel.Text = "Себестоимость:";
            priceLabel.Width = 150;
            priceLabel.Location = new Point(10, unitPriceLabel.Location.Y + unitPriceLabel.Height + 20);
            Controls.Add(priceLabel);

            Price = new Label();
            Price.Width = 100;
            Price.AutoSize = true;
            Price.Location = new Point(priceLabel.Location.X + priceLabel.Width + 10, priceLabel.Location.Y);
            Controls.Add(Price);

            CalculateItemButton = new Button();
            CalculateItemButton.Text = "Рассчитать";
            CalculateItemButton.AutoSize = true;
            CalculateItemButton.Location = new Point(priceLabel.Location.X, priceLabel.Location.Y + priceLabel.Height + 15);
            CalculateItemButton.Click += CalculateItemButton_Click;
            Controls.Add(CalculateItemButton);

            EditOrderButton = new Button();
            EditOrderButton.Text = "Изменить заказ";
            EditOrderButton.Location = new Point(CalculateItemButton.Location.X + CalculateItemButton.Width + 15,
                CalculateItemButton.Location.Y);
            EditOrderButton.Click += EditOrderButtonOnClick;
            Controls.Add(EditOrderButton);

            RemoveOrderButton = new Button();
            RemoveOrderButton.Text = "Удалить заказ";
            RemoveOrderButton.Location = new Point(EditOrderButton.Location.X + EditOrderButton.Width + 15,
                EditOrderButton.Location.Y);
            RemoveOrderButton.Click += RemoveOrderButtonOnClick;
            Controls.Add(RemoveOrderButton);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + CalculateItemButton.Location.X, CalculateItemButton.Location.Y + CalculateItemButton.Height + 15);
            PreviousListButton.Enabled = false;
            PreviousListButton.Click += PreviousNext_Click;
            Controls.Add(PreviousListButton);

            NextListButton = new Button();
            NextListButton.Text = "Вперед";
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
            NextListButton.Enabled = false;
            NextListButton.Click += PreviousNext_Click;
            Controls.Add(NextListButton);

            TableColumnName = new Label();
            TableColumnName.Location = new Point(10, PreviousListButton.Location.Y + PreviousListButton.Height + 10);
            TableColumnName.Text = "Название материала";
            TableColumnName.Width = 200;
            Controls.Add(TableColumnName);

            var unitConsumptionColumnName = new Label();
            unitConsumptionColumnName.Location = new Point(10 + TableColumnName.Location.X + TableColumnName.Width, TableColumnName.Location.Y);
            unitConsumptionColumnName.Text = "Норма расхода на ед.";
            unitConsumptionColumnName.Width = 80;
            Controls.Add(unitConsumptionColumnName);

            var unitPriceColumnName = new Label();
            unitPriceColumnName.Location = new Point(10 + unitConsumptionColumnName.Location.X + unitConsumptionColumnName.Width, unitConsumptionColumnName.Location.Y);
            unitPriceColumnName.Text = "Цена мат-а за ед.";
            unitPriceColumnName.Width = 80;
            Controls.Add(unitPriceColumnName);

            var consumptionColumnName = new Label();
            consumptionColumnName.Location = new Point(10 + unitPriceColumnName.Location.X + unitPriceColumnName.Width, unitPriceColumnName.Location.Y);
            consumptionColumnName.Text = "Норма расхода";
            consumptionColumnName.Width = 80;
            Controls.Add(consumptionColumnName);

            var priceColumnName = new Label();
            priceColumnName.Location = new Point(10 + consumptionColumnName.Location.X + consumptionColumnName.Width, consumptionColumnName.Location.Y);
            priceColumnName.Text = "Цена мат-а";
            priceColumnName.Width = 80;
            Controls.Add(priceColumnName);
        }
        public void UpdateOrder(IOrder newOrder)
        {
            Order = newOrder;
        }

        private async void RemoveOrderButtonOnClick(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить заказ?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;
            try
            {
                await OrderOperations.Remove(Order);
                await Converter.RemoveOrder(Order);
            }
            catch (OrderItemOperationException exception)
            {
                await OrderOperations.Insert(Order);

                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
        }

        private async void EditOrderButtonOnClick(object sender, EventArgs e)
        {
            //var editForm = formFactory.CreateEditOrderForm(order);
            //editForm.ShowDialog();
            NameValue.Text = Order.Name;
            Date.Text = Order.CreationDate.ToString("dd/MM/yyyy");

            await Calculate();
        }

        private async void CalculateItemButton_Click(object sender, EventArgs e)
        {
            await Calculate();
        }

        private async Task Calculate()
        {
            var priceArray = await OrderCostPrice.GetOrderCostPrice(Order);
            UnitPrice.Text = string.Join(' ', Math.Round(priceArray[0], 2).ToString(), "руб.")  ;
            Price.Text = string.Join(' ', Math.Round(priceArray[1], 2).ToString(), "руб.");
            
            await ShowItems(0);
        }
        private async void PreviousNext_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var offset = (int)control.Tag;

            await ShowItems(offset);
        }

        private async void NameLine_TextChanged(object sender, EventArgs e)
        {
            await ShowItems(0);
        }

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = Creator.LengthOfItemsList;
            var resultOfGettingItemsList = await Creator.GetItemsList(offset, "");
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    resultOfGettingItemsList = await Creator.GetItemsList(offset, "");
                    itemsList = resultOfGettingItemsList.Item1;
                    isThereMoreOfItems = resultOfGettingItemsList.Item2;
                }
                else
                {
                    ShowEmptyList("Список пуст");
                    return;
                }
            }

            if (isThereMoreOfItems)
            {
                NextListButton.Enabled = true;
                NextListButton.Tag = offset + maxShowItemsCount;
            }
            else
                DefaultNextButton();

            if (offset > 0)
            {
                PreviousListButton.Enabled = true;
                PreviousListButton.Tag = offset - maxShowItemsCount;
            }
            else
                DefaultPreviousButton();

            await RefreshActiveItemTempControl(itemsList);
        }

        private async Task RefreshActiveItemTempControl(List<IBlockItem> itemsList)
        {
            var lastControl = (Control)TableColumnName;

            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            foreach (var item in itemsList)
                lastControl = await DisplayItems(item, lastControl);
        }

        private void ShowEmptyList(string message)
        {
            DefaultPreviousNextButtons();
            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            var lastControl = TableColumnName;
            var label = new Label();
            label.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
            label.Text = message;
            label.AutoSize = true;
            Controls.Add(label);
            ActiveItemTempControls.Add(label);
        }

        private async Task<Control> DisplayItems(IBlockItem item, Control lastControl)
        {
            var orderItem = (IOrderItem) item;
            var prices = await OrderItemCostPrice.GetOrderItemCostPrice(orderItem);
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
            itemLabel.MaximumSize = new Size(200, 0);
            itemLabel.AutoSize = true;
            Controls.Add(itemLabel);
            ActiveItemTempControls.Add(itemLabel);

            itemLabel.Text = orderItem.Name;
            itemLabel.Tag = orderItem;
            
            var unitConsumption = new Label();
            unitConsumption.Text = string.Join(' ', Math.Round(orderItem.UnitConsumption, 2).ToString(), InputOperations.TranslateType(orderItem.Material.Unit.ToString()));
            unitConsumption.Location = new Point(200 + itemLabel.Location.X + 10, itemLabel.Location.Y);
            unitConsumption.Width = 80;
            Controls.Add(unitConsumption);
            ActiveItemTempControls.Add(unitConsumption);

            var unitItemPrice = new Label();
            unitItemPrice.Text = string.Join(' ', Math.Round(prices[0], 2).ToString(), "руб.") ;
            unitItemPrice.Location = new Point(unitConsumption.Width + unitConsumption.Location.X + 10, unitConsumption.Location.Y);
            unitItemPrice.Width = 80;
            Controls.Add(unitItemPrice);
            ActiveItemTempControls.Add(unitItemPrice);

            var consumption = new Label();
            consumption.Text = string.Join(' ', Math.Round(orderItem.TotalConsumption, 2).ToString(), InputOperations.TranslateType(orderItem.Material.Unit.ToString())) ;
            consumption.Location = new Point(unitItemPrice.Width + unitItemPrice.Location.X + 10, unitItemPrice.Location.Y);
            consumption.Width = 80;
            Controls.Add(consumption);
            ActiveItemTempControls.Add(consumption);

            var itemPrice = new Label();
            itemPrice.Text = string.Join(' ', Math.Round(prices[1], 2).ToString(), "руб.") ;
            itemPrice.Location = new Point(consumption.Width + consumption.Location.X + 10, consumption.Location.Y);
            itemPrice.Width = 80;
            Controls.Add(itemPrice);
            ActiveItemTempControls.Add(itemPrice);

            return itemLabel;
        }

        private void ClearActiveItemTempControls()
        {
            foreach (var activeItemControl in ActiveItemTempControls)
            {
                Controls.Remove(activeItemControl);
            }
            ActiveItemTempControls.Clear();
        }

        private void DefaultPreviousButton()
        {
            PreviousListButton.Enabled = false;
            PreviousListButton.Tag = null;
        }
        private void DefaultNextButton()
        {
            NextListButton.Enabled = false;
            NextListButton.Tag = null;
        }
        private void DefaultPreviousNextButtons()
        {
            DefaultPreviousButton();
            DefaultNextButton();
        }
    }
}
