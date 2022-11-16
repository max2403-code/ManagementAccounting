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
using ManagementAccounting.Forms.PreOrdersForms;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;
using Npgsql;

namespace ManagementAccounting.Forms.OrdersForms
{
    public partial class CreateOrderForm : Form
    {
        private BlockItemsCollectionCreator Creator { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button AddButton { get; }
        private Button CloseButton { get; }
        private Button AllOrderItems { get; }
        private Label OrderNameValue { get; }
        private Label OrderQuantityValue { get; }
        private Label OrderCreationDateValue { get; }
        private List<Control> ActiveItemTempControls { get; }
        private List<Control> ActiveItemTempValueControls { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IItemsFactory ItemsFactory { get; }
        private IOrder Order { get; }
        private ISystemOrderOperations OrderOperations { get; }
        private IFromPreOrderToOrderConverter Converter { get; }
        private PreOrderForm PreOrderForm { get; }


        public CreateOrderForm(IOrder order, PreOrderForm preOrderForm, IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory, ICreatorFactory creatorFactory, ISystemOrderOperations orderOperations, IFromPreOrderToOrderConverter converter)
        {
            PreOrderForm = preOrderForm;
            ItemsFactory = itemsFactory;
            InputOperations = inputOperations;
            OrderOperations = orderOperations;
            Order = order;
            Converter = converter;
            Creator = creatorFactory.CreateOrderItemCollectionCreator(order, 5);
            ActiveItemTempControls = new List<Control>();
            ActiveItemTempValueControls = new List<Control>();

            FormClosing += OnFormClosing;
            Size = new Size(500, 500);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Внесение заказа в систему";
            Controls.Add(topLabel);

            var orderNameLabel = new Label();
            orderNameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 20);
            orderNameLabel.Width = 150;
            orderNameLabel.Text = "Наименование заказа:";
            Controls.Add(orderNameLabel);

            OrderNameValue = new Label();
            OrderNameValue.Text = Order.Name;
            OrderNameValue.Location = new Point(orderNameLabel.Location.X + orderNameLabel.Width + 10, orderNameLabel.Location.Y);
            OrderNameValue.Width = 200;
            Controls.Add(OrderNameValue);

            var orderQuantityLabel = new Label();
            orderQuantityLabel.Location = new Point(10, orderNameLabel.Location.Y + orderNameLabel.Height + 10);
            orderQuantityLabel.Width = 150;
            orderQuantityLabel.Text = "Количество:";
            Controls.Add(orderQuantityLabel);

            OrderQuantityValue = new Label();
            OrderQuantityValue.Text = Order.Quantity.ToString();
            OrderQuantityValue.Location = new Point(orderQuantityLabel.Location.X + orderQuantityLabel.Width + 10, orderQuantityLabel.Location.Y);
            OrderQuantityValue.Width = 200;
            Controls.Add(OrderQuantityValue);

            var orderCreationDateLabel = new Label();
            orderCreationDateLabel.Location = new Point(10, orderQuantityLabel.Location.Y + orderQuantityLabel.Height + 10);
            orderCreationDateLabel.Width = 150;
            orderCreationDateLabel.Text = "Дата создания:";
            Controls.Add(orderCreationDateLabel);

            OrderCreationDateValue = new Label();
            OrderCreationDateValue.Text = Order.CreationDate.ToString("dd/MM/yyyy");
            OrderCreationDateValue.Location = new Point(orderCreationDateLabel.Location.X + orderCreationDateLabel.Width + 10, orderCreationDateLabel.Location.Y);
            OrderCreationDateValue.Width = 200;
            Controls.Add(OrderCreationDateValue);

            AddButton = new Button();
            AddButton.Location = new Point(50, orderCreationDateLabel.Location.Y + orderCreationDateLabel.Height + 25);
            AddButton.Text = "Добавить заказ";
            AddButton.AutoSize = true;
            AddButton.Click += AddButtonOnClick;
            Controls.Add(AddButton);
            
            CloseButton = new Button();
            CloseButton.Location = new Point(AddButton.Location.X + AddButton.Width + 20, AddButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += CloseButtonOnClick;
            Controls.Add(CloseButton);
            
            AllOrderItems = new Button();
            AllOrderItems.Text = "Показать материалы";
            AllOrderItems.AutoSize = true;
            AllOrderItems.Location = new Point(AddButton.Location.X, AddButton.Location.Y + AddButton.Height + 15);
            AllOrderItems.Click += AllOrderItemsOnClick;
            Controls.Add(AllOrderItems);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + AllOrderItems.Location.X + AllOrderItems.Width, AllOrderItems.Location.Y);
            PreviousListButton.Enabled = false;
            PreviousListButton.Click += PreviousNext_Click;
            Controls.Add(PreviousListButton);

            NextListButton = new Button();
            NextListButton.Text = "Вперед";
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
            NextListButton.Enabled = false;
            NextListButton.Click += PreviousNext_Click;
            Controls.Add(NextListButton);
        }

        private async void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (PreOrderForm.IsConvertingPasssedSuccessfully) return;

            try
            {
                await Converter.RemoveOrder(Order);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
            }
        }

        private async void AllOrderItemsOnClick(object sender, EventArgs e)
        {
            try
            {
                await ShowItems(0);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
            }
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            //DisableButtons();
            
            try
            {
                if(await IsAssignOrderItemsPassedSuccessfully())
                    await OrderOperations.Insert(Order);
            }
            catch (OrderItemOperationException exception)
            {
                try
                {
                    await OrderOperations.Remove(Order);
                }
                catch (OrderItemOperationException)
                {
                    MessageBox.Show("Если ошибка произощла здесь, то это печально", "Внимание");
                    return;
                }
                catch (NpgsqlException npgsqlException)
                {
                    MessageBox.Show(npgsqlException.Message, "Внимание");
                    return;
                }
                MessageBox.Show(exception.Message, "Внимание");
                //EnableButtons();
                return;
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }

            PreOrderForm.IsConvertingPasssedSuccessfully = true;
            Close();
        }

        private async void PreviousNext_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var offset = (int)control.Tag;

            try
            {
                if (await IsAssignOrderItemsPassedSuccessfully())
                    await ShowItems(offset);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
            }
        }

        private async Task<bool> IsAssignOrderItemsPassedSuccessfully()
        {
            foreach (var control in ActiveItemTempValueControls)
            {
                var orderItem = (IOrderItem)control.Tag;
                var tryGetConsumption = InputOperations.TryGetPositiveDecimal(control.Text, out var consumption);
                if (!tryGetConsumption)
                {
                    control.Text = "";
                    MessageBox.Show("Введены некорректные данные", "Внимание");
                    //EnableButtons();
                    return false;
                }

                if (Math.Abs(orderItem.TotalConsumption - consumption) >= (decimal) 1e-9)
                {
                    await ((EditingBlockItemDB)orderItem).EditItemInDataBase<IOrderItem>(consumption, consumption);
                }
            }

            return true;
        }

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = Creator.LengthOfItemsList;
            var resultOfGettingItemsList = await Creator.GetItemsList(offset, "");
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

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

            RefreshActiveItemTempControl(itemsList);
        }

        private void RefreshActiveItemTempControl(List<IBlockItem> itemsList)
        {
            var lastControl = (Control)PreviousListButton;

            if (ActiveItemTempControls.Count > 0)
            {
                ClearActiveItemTempControls();
                ClearActiveItemTempValueControls();
            }
            foreach (var item in itemsList)
                lastControl = DisplayItems(item, lastControl);
        }

        private Control DisplayItems(IBlockItem item, Control lastControl)
        {
            var orderItem = (IOrderItem) item;
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
            itemLabel.MaximumSize = new Size(200, 0);
            itemLabel.AutoSize = true;
            itemLabel.Text = orderItem.Name;
            Controls.Add(itemLabel);
            ActiveItemTempControls.Add(itemLabel);

            var unitOfMaterial = new Label();
            unitOfMaterial.Location = new Point(itemLabel.Location.X + 100 + 5, itemLabel.Location.Y);
            unitOfMaterial.Text = InputOperations.TranslateType(orderItem.Material.Unit.ToString());
            Controls.Add(unitOfMaterial);
            ActiveItemTempControls.Add(unitOfMaterial);

            var consumptionValue = new TextBox();
            consumptionValue.Text = orderItem.TotalConsumption.ToString();
            consumptionValue.Width = 70;
            consumptionValue.Location = new Point(unitOfMaterial.Location.X + unitOfMaterial.Width + 10, unitOfMaterial.Location.Y);
            consumptionValue.Tag = orderItem;
            Controls.Add(consumptionValue);
            ActiveItemTempValueControls.Add(consumptionValue);
            
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

        private void ClearActiveItemTempValueControls()
        {
            foreach (var activeItemControl in ActiveItemTempValueControls)
            {
                Controls.Remove(activeItemControl);
            }
            ActiveItemTempValueControls.Clear();
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
