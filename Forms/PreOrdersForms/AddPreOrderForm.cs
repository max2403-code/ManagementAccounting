using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;
using Npgsql;

namespace ManagementAccounting.Forms.PreOrdersForms
{
    public partial class AddPreOrderForm : Form
    {
        //private int _offset { get; set; }
        private BlockItemsCollectionCreator Creator { get; }
        private TextBox NameLine { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button AddButton { get; }
        private Button AllCalculations { get; }

        private Button CloseButton { get; }
        private Label CalculationValue { get; }
        private TextBox QuantityValue { get; }
        private TextBox DateValue { get; }
        private List<Button> Buttons { get; }
        private List<Control> ActiveItemTempControls { get; }

        private IOperationsWithUserInput InputOperations { get; }
        private IItemsFactory ItemsFactory { get; }


        public AddPreOrderForm(IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory, ICreatorFactory creatorFactory)
        {
            Buttons = new List<Button>();
            this.ItemsFactory = itemsFactory;
            this.InputOperations = inputOperations;
            Creator = creatorFactory.CreateCalculationCollectionCreator(5);
            ActiveItemTempControls = new List<Control>();
            Size = new Size(500, 500);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление предзаказа";
            Controls.Add(topLabel);

            var calculationLabel = new Label();
            calculationLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            calculationLabel.Width = 150;
            calculationLabel.Text = "Наименование кальк-ии:";
            Controls.Add(calculationLabel);

            CalculationValue = new Label();
            CalculationValue.Location = new Point(calculationLabel.Location.X + calculationLabel.Width + 10, calculationLabel.Location.Y);
            CalculationValue.Width = 200;
            Controls.Add(CalculationValue);

            var quantityLabel = new Label();
            quantityLabel.Location = new Point(10, calculationLabel.Location.Y + calculationLabel.Height + 50);
            quantityLabel.Width = 150;
            quantityLabel.Text = "Количество, шт.:";
            Controls.Add(quantityLabel);

            QuantityValue = new TextBox();
            QuantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            QuantityValue.Width = 200;
            Controls.Add(QuantityValue);

            var dateLabel = new Label();
            dateLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 50);
            dateLabel.Width = 150;
            dateLabel.Text = "Дата создания:";
            Controls.Add(dateLabel);

            DateValue = new TextBox();
            DateValue.Location = new Point(dateLabel.Location.X + dateLabel.Width + 10, dateLabel.Location.Y);
            DateValue.Width = 200;
            Controls.Add(DateValue);

            AddButton = new Button();
            AddButton.Location = new Point(50, DateValue.Location.Y + DateValue.Height + 25);
            AddButton.Text = "Добавить";
            AddButton.AutoSize = true;
            AddButton.Click += AddButtonOnClick;
            Controls.Add(AddButton);

            CloseButton = new Button();
            CloseButton.Location = new Point(AddButton.Location.X + AddButton.Width + 20, AddButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += CloseButtonOnClick;
            Controls.Add(CloseButton);

            NameLine = new TextBox();
            NameLine.Location = new Point(AddButton.Location.X, AddButton.Location.Y + AddButton.Height + 15);
            NameLine.Width = 300;
            NameLine.TextChanged += NameLine_TextChanged;
            Controls.Add(NameLine);

            AllCalculations = new Button();
            AllCalculations.Text = "Все калькуляции";
            AllCalculations.AutoSize = true;
            AllCalculations.Location = new Point(NameLine.Location.X, NameLine.Location.Y + NameLine.Height + 15);
            AllCalculations.Click += AllCalculationsOnClick ;
            Controls.Add(AllCalculations);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + AllCalculations.Location.X + AllCalculations.Width, AllCalculations.Location.Y );
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

        private async void AllCalculationsOnClick(object? sender, EventArgs e)
        {
            NameLine.Text = "";
            try
            {
                await ShowItems(0);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
            }
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var calculation = CalculationValue.Tag;
            var isQuantityCorrect = InputOperations.TryGetPositiveInt(QuantityValue.Text, out var quantity);
            var isDateCorrect = InputOperations.TryGetCorrectData(DateValue.Text, out var date);

            if (calculation == null || !isDateCorrect || !isQuantityCorrect)
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                EnableButtons();
                return;
            }
            try
            {
                var preOrder = ItemsFactory.CreatePreOrder((ICalculation)calculation, quantity, date) as EditingBlockItemDB;
                await preOrder.AddItemToDataBase();
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
                return;
            }
            Close();
        }

        private async void PreviousNext_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var offset = (int)control.Tag;

            try
            {
                await ShowItems(offset);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
            }
        }

        private async void NameLine_TextChanged(object sender, EventArgs e)
        {
            if (NameLine.Text == "")
            {
                ShowEmptyList("Введите название материала");
                return;
            }

            try
            {
                await ShowItems(0);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
            }
        }

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = Creator.LengthOfItemsList;
            var resultOfGettingItemsList = await Creator.GetItemsList(offset, NameLine.Text.ToLower());
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    resultOfGettingItemsList = await Creator.GetItemsList(offset, NameLine.Text.ToLower());
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

            RefreshActiveItemTempControl(itemsList);
        }

        private void RefreshActiveItemTempControl(List<IBlockItem> itemsList)
        {
            var lastControl = (Control)PreviousListButton;

            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            foreach (var item in itemsList)
                lastControl = DisplayItems(item, lastControl);
        }

        private void ShowEmptyList(string message)
        {
            DefaultPreviousNextButtons();
            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            var lastControl = PreviousListButton;
            var label = new Label();
            label.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 25);
            label.Text = message;
            label.AutoSize = true;
            Controls.Add(label);
            ActiveItemTempControls.Add(label);
        }

        private Control DisplayItems(IBlockItem item, Control lastControl)
        {
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 10);
            itemLabel.AutoSize = true;
            itemLabel.Click += ItemLabel_Click;
            Controls.Add(itemLabel);
            ActiveItemTempControls.Add(itemLabel);

            itemLabel.Text = item.Name;
            itemLabel.Tag = item;

            return itemLabel;
        }
        private void ItemLabel_Click(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var calculation = (ICalculation)label.Tag;
            this.CalculationValue.Tag = calculation;
            this.CalculationValue.Text = calculation.Name;
            NameLine.Text = "";
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
        private void EnableButtons()
        {
            foreach (var button in Buttons)
                button.Enabled = true;
        }

        private void DisableButtons()
        {
            foreach (var button in Buttons)
                button.Enabled = false;
        }
    }
}
