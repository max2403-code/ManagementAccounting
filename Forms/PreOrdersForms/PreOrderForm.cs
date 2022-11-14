using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Forms.PreOrdersForms
{
    public partial class PreOrderForm : Form
    {
        public bool IsConvertingPasssedSuccessfully { get; set; }
        private Label NameValue { get; }
        private Label Quantity { get; }
        private Label Date { get; }
        private Label MinUnitPrice { get; }
        private Label MaxUnitPrice { get; }
        private Label MinPrice { get; }
        private Label MaxPrice { get; }
        private Label TableColumnName { get; }
        private Label IsAvailablePrice { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button CalculateItemButton { get; }
        private Button CreateOrderButton { get; }
        private Button EditPreOrderButton { get; }
        private Button RemovePreOrderButton { get; }
        private List<Control> ActiveItemTempControls { get; }
        private IPreOrder PreOrder { get; }
        private IFormFactory FormFactory { get; }
        private BlockItemsCollectionCreator Creator { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IPreOrderCostPrice PreOrderCostPrice { get; }
        private TextBox OrderDateTextBox { get; }
        private IEmptyCalculationChecker EmptyCalculationChecker { get; }
        private IOrder Order { get; set; }
        private IFromPreOrderToOrderConverter Converter { get; }


        public PreOrderForm(IPreOrder preOrder, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory, IPreOrderCostPrice preOrderCostPrice, IEmptyCalculationChecker emptyCalculationChecker, IFromPreOrderToOrderConverter fromPreOrderToOrderConverter)
        {
            Converter = fromPreOrderToOrderConverter;
            EmptyCalculationChecker = emptyCalculationChecker;
            PreOrderCostPrice = preOrderCostPrice;
            PreOrder = preOrder;
            InputOperations = inputOperations;
            FormFactory = formFactory;
            Creator = creatorFactory.CreatePreOrderItemCollectionCreator(preOrder, 5);
            ActiveItemTempControls = new List<Control>();

            Size = new Size(1200, 700);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Предзаказ";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Text = "Наименование:";
            nameLabel.Width = 150;
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 20);
            Controls.Add(nameLabel);

            NameValue = new Label();
            NameValue.Text = preOrder.Name;
            NameValue.Width = 200;
            NameValue.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            Controls.Add(NameValue);

            var quantityLabel = new Label();
            quantityLabel.Text = "Количество изделий:";
            quantityLabel.Width = 150;
            quantityLabel.Location = new Point(10, nameLabel.Location.Y + nameLabel.Height + 10);
            Controls.Add(quantityLabel);

            Quantity = new Label();
            Quantity.Text = string.Join(' ', preOrder.Quantity.ToString(), InputOperations.TranslateType(UnitOfMaterial.pcs.ToString())) ;
            Quantity.AutoSize = true;
            Quantity.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            Controls.Add(Quantity);
            
            var dateLabel = new Label();
            dateLabel.Text = "Дата предзаказа:";
            dateLabel.Width = 150;
            dateLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 10);
            Controls.Add(dateLabel);

            Date = new Label();
            Date.Text = preOrder.CreationDate.ToString("dd/MM/yyyy");
            Date.Width = 200;
            Date.Location = new Point(dateLabel.Location.X + dateLabel.Width + 10, dateLabel.Location.Y);
            Controls.Add(Date);

            var minUnitPriceLabel = new Label();
            minUnitPriceLabel.Text = "Мин. себестоимость ед.:";
            minUnitPriceLabel.Width = 150;
            minUnitPriceLabel.Location = new Point(10, dateLabel.Location.Y + dateLabel.Height + 10);
            Controls.Add(minUnitPriceLabel);

            MinUnitPrice = new Label();
            MinUnitPrice.Width = 200;
            MinUnitPrice.AutoSize = true;
            MinUnitPrice.Location = new Point(minUnitPriceLabel.Location.X + minUnitPriceLabel.Width + 10, minUnitPriceLabel.Location.Y);
            Controls.Add(MinUnitPrice);
            
            var maxUnitPriceLabel = new Label();
            maxUnitPriceLabel.Text = "Макс. себестоимость ед.:";
            maxUnitPriceLabel.Width = 150;
            maxUnitPriceLabel.Location = new Point(10, minUnitPriceLabel.Location.Y + minUnitPriceLabel.Height + 10);
            Controls.Add(maxUnitPriceLabel);

            MaxUnitPrice = new Label();
            MaxUnitPrice.Width = 200;
            MaxUnitPrice.AutoSize = true;
            MaxUnitPrice.Location = new Point(maxUnitPriceLabel.Location.X + maxUnitPriceLabel.Width + 10, maxUnitPriceLabel.Location.Y);
            Controls.Add(MaxUnitPrice);

            var minPriceLabel = new Label();
            minPriceLabel.Text = "Мин. себестоимость:";
            minPriceLabel.Width = 150;
            minPriceLabel.Location = new Point(10, maxUnitPriceLabel.Location.Y + maxUnitPriceLabel.Height + 10);
            Controls.Add(minPriceLabel);

            MinPrice = new Label();
            MinPrice.Width = 200;
            MinPrice.AutoSize = true;
            MinPrice.Location = new Point(minPriceLabel.Location.X + minPriceLabel.Width + 10, minPriceLabel.Location.Y);
            Controls.Add(MinPrice);

            var maxPriceLabel = new Label();
            maxPriceLabel.Text = "Макс. себестоимость:";
            maxPriceLabel.Width = 150;
            maxPriceLabel.Location = new Point(10, minPriceLabel.Location.Y + minPriceLabel.Height + 10);
            Controls.Add(maxPriceLabel);

            MaxPrice = new Label();
            MaxPrice.Width = 200;
            MaxPrice.AutoSize = true;
            MaxPrice.Location = new Point(maxPriceLabel.Location.X + maxPriceLabel.Width + 10, maxPriceLabel.Location.Y);
            Controls.Add(MaxPrice);

            var orderDateLabel = new Label();
            orderDateLabel.Text = "Дата изготовления:";
            orderDateLabel.Width = 150;
            orderDateLabel.Location = new Point(10, maxPriceLabel.Location.Y + maxPriceLabel.Height + 10);
            Controls.Add(orderDateLabel);

            OrderDateTextBox = new TextBox();
            OrderDateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            OrderDateTextBox.TextChanged += OrderDateTextBox_TextChanged;
            OrderDateTextBox.Width = 100;
            OrderDateTextBox.Location = new Point(orderDateLabel.Location.X + orderDateLabel.Width + 10, orderDateLabel.Location.Y);
            Controls.Add(OrderDateTextBox);

            IsAvailablePrice = new Label();
            IsAvailablePrice.Width = 300;
            IsAvailablePrice.Location = new Point(10, orderDateLabel.Location.Y + orderDateLabel.Height + 10);
            Controls.Add(IsAvailablePrice);

            CalculateItemButton = new Button();
            CalculateItemButton.Text = "Рассчитать";
            CalculateItemButton.AutoSize = true;
            CalculateItemButton.Location = new Point(IsAvailablePrice.Location.X, IsAvailablePrice.Location.Y + IsAvailablePrice.Height + 15);
            CalculateItemButton.Click += CalculateItemButton_Click;
            Controls.Add(CalculateItemButton);

            CreateOrderButton = new Button();
            CreateOrderButton.Text = "В заказ";
            CreateOrderButton.Location = new Point(CalculateItemButton.Location.X + CalculateItemButton.Width + 15,
                CalculateItemButton.Location.Y);
            CreateOrderButton.Enabled = false;
            CreateOrderButton.Click += CreateOrderButton_Click;
            Controls.Add(CreateOrderButton);

            EditPreOrderButton = new Button();
            EditPreOrderButton.Text = "Изменить предзаказ";
            EditPreOrderButton.Location = new Point(CreateOrderButton.Location.X + CreateOrderButton.Width + 15,
                CreateOrderButton.Location.Y);
            EditPreOrderButton.Click += EditPreOrderButtonOnClick;
            Controls.Add(EditPreOrderButton);

            RemovePreOrderButton = new Button();
            RemovePreOrderButton.Text = "Удалить предзаказ";
            RemovePreOrderButton.Location = new Point(EditPreOrderButton.Location.X + EditPreOrderButton.Width + 15,
                EditPreOrderButton.Location.Y);
            RemovePreOrderButton.Click += RemovePreOrderButtonOnClick;
            Controls.Add(RemovePreOrderButton);

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
            unitConsumptionColumnName.Width = 150;
            Controls.Add(unitConsumptionColumnName);

            var unitMinPriceColumnName = new Label();
            unitMinPriceColumnName.Location = new Point(10 + unitConsumptionColumnName.Location.X + unitConsumptionColumnName.Width, unitConsumptionColumnName.Location.Y);
            unitMinPriceColumnName.Text = "Мин. цена мат-а за ед.";
            unitMinPriceColumnName.Width = 150;
            Controls.Add(unitMinPriceColumnName);

            var unitMaxPriceColumnName = new Label();
            unitMaxPriceColumnName.Location = new Point(10 + unitMinPriceColumnName.Location.X + unitMinPriceColumnName.Width, unitMinPriceColumnName.Location.Y);
            unitMaxPriceColumnName.Text = "Макс. цена мат-а за ед.";
            unitMaxPriceColumnName.Width = 150;
            Controls.Add(unitMaxPriceColumnName);

            var consumptionColumnName = new Label();
            consumptionColumnName.Location = new Point(10 + unitMaxPriceColumnName.Location.X + unitMaxPriceColumnName.Width, unitMaxPriceColumnName.Location.Y);
            consumptionColumnName.Text = "Норма расхода";
            consumptionColumnName.Width = 150;
            Controls.Add(consumptionColumnName);

            var priceMinColumnName = new Label();
            priceMinColumnName.Location = new Point(10 + consumptionColumnName.Location.X + consumptionColumnName.Width, consumptionColumnName.Location.Y);
            priceMinColumnName.Text = "Мин. цена мат-а";
            priceMinColumnName.Width = 150;
            Controls.Add(priceMinColumnName);

            var priceMaxColumnName = new Label();
            priceMaxColumnName.Location = new Point(10 + priceMinColumnName.Location.X + priceMinColumnName.Width, priceMinColumnName.Location.Y);
            priceMaxColumnName.Text = "Макс. цена мат-а";
            priceMaxColumnName.Width = 150;
            Controls.Add(priceMaxColumnName);
        }

        private async void CreateOrderButton_Click(object sender, EventArgs e)
        {
            var date = (DateTime)CreateOrderButton.Tag;
            try
            {
                Order = await Converter.Convert(PreOrder, date);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }

            try
            {
                await Converter.CreateOrderItems(Order, PreOrder);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                await Converter.RemoveOrder(Order);
                Order = null;
                return;
            }

            var creationOrderForm = FormFactory.CreateCreateOrderForm(Order, this);
            creationOrderForm.ShowDialog();

            if (IsConvertingPasssedSuccessfully)
            {
                await ((EditingBlockItemDB) PreOrder).RemoveItemFromDataBase();
                Close();
            }
            else
                Order = null;
        }

        private void OrderDateTextBox_TextChanged(object sender, EventArgs e)
        {
            DefaultPreOrder();
        }

        private void DefaultPreOrder()
        {
            ShowEmptyList("");
            CreateOrderButton.Enabled = false;
            CreateOrderButton.Tag = null;
            MinUnitPrice.Text = "";
            MaxUnitPrice.Text = "";
            MinPrice.Text = "";
            MaxPrice.Text = "";
            IsAvailablePrice.Text = "";
        }

        private async void RemovePreOrderButtonOnClick(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить предзаказ?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;

            try
            {
                await ((BlockItemDB)PreOrder).RemoveItemFromDataBase();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
        }

        private void EditPreOrderButtonOnClick(object sender, EventArgs e)
        {
            var editForm = FormFactory.CreateEditPreOrderForm(PreOrder);
            editForm.ShowDialog();
            NameValue.Text = PreOrder.Name;
            Date.Text = PreOrder.CreationDate.ToString("dd/MM/yyyy");
            Quantity.Text = string.Join(' ', PreOrder.Quantity.ToString(), InputOperations.TranslateType(UnitOfMaterial.pcs.ToString()));
            DefaultPreOrder();

            //await Calculate();
        }

        private async void CalculateItemButton_Click(object sender, EventArgs e)
        {
            var isDateCorrect = InputOperations.TryGetCorrectData(OrderDateTextBox.Text, out var date);

            if (!isDateCorrect )
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                return;
            }

            CreateOrderButton.Tag = date;
            await Calculate(date);
        }

        private async Task Calculate(DateTime orderDate)
        {
            var isCalculationNotEmpty = await EmptyCalculationChecker.IsCalculationNotEmpty(PreOrder.Calculation);
            var result = await PreOrderCostPrice.GetPreOrderCostPrice(PreOrder, orderDate);
            var priceArray = result.Item1;
            var isOrderCanBeMade = result.Item2 && isCalculationNotEmpty;
            MinUnitPrice.Text = string.Join(' ', Math.Round(priceArray[0], 2).ToString(), "руб.");
            MaxUnitPrice.Text = string.Join(' ', Math.Round(priceArray[1], 2).ToString(), "руб.");
            MinPrice.Text = string.Join(' ', Math.Round(priceArray[2], 2).ToString(), "руб.");
            MaxPrice.Text = string.Join(' ', Math.Round(priceArray[3], 2).ToString(), "руб.");
            IsAvailablePrice.Text = isOrderCanBeMade ? "Цена актуальна" : "Материалов не хватает или кальк-я не имеет мат-ов";
            CreateOrderButton.Enabled = isOrderCanBeMade;
            await ShowItems(0);
        }

        private async void PreviousNext_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var offset = (int)control.Tag;

            await ShowItems(offset);
        }

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = Creator.LengthOfItemsList;
            var resultOfGettingItemsList = await Creator.GetItemsList(offset, OrderDateTextBox.Text);
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            if (itemsList.Count == 0)
            {
                ShowEmptyList("Список пуст");
                return;
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

            RefreshActiveItemTempControl(itemsList);
        }

        private void RefreshActiveItemTempControl(List<IBlockItem> itemsList)
        {
            var lastControl = (Control)TableColumnName;

            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            foreach (var item in itemsList)
                lastControl = DisplayItems(item, lastControl);
        }

        private void ShowEmptyList(string message)
        {
            DefaultPreviousNextButtons();
            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            var lastControl = TableColumnName;
            var label = new Label();
            label.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 25);
            label.Text = message;
            label.AutoSize = true;
            Controls.Add(label);
            ActiveItemTempControls.Add(label);
        }

        private Control DisplayItems(IBlockItem item, Control lastControl)
        {
            var preOrderItem = (IPreOrderItem)item;
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 10);
            itemLabel.MaximumSize = new Size(200, 0);
            itemLabel.AutoSize = true;
            itemLabel.Text = preOrderItem.Name;
            itemLabel.Tag = preOrderItem;
            Controls.Add(itemLabel);
            ActiveItemTempControls.Add(itemLabel);

            var minUnitC = new Label();
            minUnitC.Text = string.Join(' ', Math.Round(preOrderItem.MaterialUnitСonsumption, 2).ToString(), InputOperations.TranslateType(preOrderItem.Material.Unit.ToString())) ;
            minUnitC.Location = new Point(200 + itemLabel.Location.X + 10, itemLabel.Location.Y);
            minUnitC.Width = 150;
            Controls.Add(minUnitC);
            ActiveItemTempControls.Add(minUnitC);

            var minUnitP = new Label();
            minUnitP.Text = string.Join(' ', Math.Round(preOrderItem.MinUnitPrice, 2).ToString(), "руб.") ;
            minUnitP.Location = new Point(minUnitC.Width + minUnitC.Location.X + 10, minUnitC.Location.Y);
            minUnitP.Width = 150;
            Controls.Add(minUnitP);
            ActiveItemTempControls.Add(minUnitP);

            var maxUnitP = new Label();
            maxUnitP.Text = string.Join(' ', Math.Round(preOrderItem.MaxUnitPrice, 2).ToString(), "руб.") ;
            maxUnitP.Location = new Point(minUnitP.Width + minUnitP.Location.X + 10, minUnitP.Location.Y);
            maxUnitP.Width = 150;
            Controls.Add(maxUnitP);
            ActiveItemTempControls.Add(maxUnitP);

            var consumption = new Label();
            consumption.Text = string.Join(' ', Math.Round(preOrderItem.MaterialСonsumption, 2).ToString(), InputOperations.TranslateType(preOrderItem.Material.Unit.ToString())) ;
            consumption.Location = new Point(maxUnitP.Width + maxUnitP.Location.X + 10, maxUnitP.Location.Y);
            consumption.Width = 150;
            Controls.Add(consumption);
            ActiveItemTempControls.Add(consumption);

            var minP = new Label();
            minP.Text = string.Join(' ', Math.Round(preOrderItem.MinPrice, 2).ToString(), "руб.") ;
            minP.Location = new Point(consumption.Width + consumption.Location.X + 10, consumption.Location.Y);
            minP.Width = 150;
            Controls.Add(minP);
            ActiveItemTempControls.Add(minP);

            var maxP = new Label();
            maxP.Text = string.Join(' ', Math.Round(preOrderItem.MaxPrice, 2).ToString(), "руб.") ;
            maxP.Location = new Point(minP.Width + minP.Location.X + 10, minP.Location.Y);
            maxP.Width = 150;
            Controls.Add(maxP);
            ActiveItemTempControls.Add(maxP);

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
