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

namespace ManagementAccounting.Forms.PreOrdersForms
{
    public partial class AddPreOrderForm : Form
    {
        private int _offset { get; set; }
        private BlockItemsCollectionCreator creator { get; }
        private TextBox nameLine { get; }
        private Button nextListButton { get; }
        private Button previousListButton { get; }
        private Button addButton { get; }
        private Button closeButton { get; }
        private Label calculationValue { get; }
        private TextBox quantityValue { get; }
        private TextBox dateValue { get; }

        private List<Control> activeItemTempControls { get; }

        private IOperationsWithUserInput inputOperations { get; }
        private IItemsFactory itemsFactory { get; }
        private ICalculation calculation { get; }


        public AddPreOrderForm(IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory, ICreatorFactory creatorFactory)
        {
            this.itemsFactory = itemsFactory;
            this.inputOperations = inputOperations;
            this.calculation = calculation;
            creator = creatorFactory.CreateCalculationCollectionCreator(5);
            activeItemTempControls = new List<Control>();
            Size = new Size(500, 500);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление предзаказа";
            Controls.Add(topLabel);

            var calculationLabel = new Label();
            calculationLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            calculationLabel.Width = 100;
            calculationLabel.Text = "Наименование калькуляции:";
            Controls.Add(calculationLabel);

            calculationValue = new Label();
            calculationValue.Location = new Point(calculationLabel.Location.X + calculationLabel.Width + 10, calculationLabel.Location.Y);
            calculationValue.Width = 200;
            Controls.Add(calculationValue);

            var quantityLabel = new Label();
            quantityLabel.Location = new Point(10, calculationLabel.Location.Y + calculationLabel.Height + 50);
            quantityLabel.Width = 100;
            quantityLabel.Text = "Количество:";
            Controls.Add(quantityLabel);

            quantityValue = new TextBox();
            quantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            quantityValue.Width = 200;
            Controls.Add(quantityValue);

            var dateLabel = new Label();
            dateLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 50);
            dateLabel.Width = 100;
            dateLabel.Text = "Дата создания:";
            Controls.Add(dateLabel);

            dateValue = new TextBox();
            dateValue.Location = new Point(dateLabel.Location.X + dateLabel.Width + 10, dateLabel.Location.Y);
            dateValue.Width = 200;
            Controls.Add(dateValue);

            addButton = new Button();
            addButton.Location = new Point(50, dateValue.Location.Y + dateValue.Height + 25);
            addButton.Text = "Добавить";
            addButton.AutoSize = true;
            addButton.Click += AddButtonOnClick;
            Controls.Add(addButton);

            closeButton = new Button();
            closeButton.Location = new Point(addButton.Location.X + addButton.Width + 20, addButton.Location.Y);
            closeButton.Text = "Отмена";
            closeButton.AutoSize = true;
            closeButton.Click += CloseButtonOnClick;
            Controls.Add(closeButton);

            nameLine = new TextBox();
            nameLine.Location = new Point(addButton.Location.X, addButton.Location.Y + addButton.Height + 15);
            nameLine.Width = 300;
            nameLine.TextChanged += NameLine_TextChanged;
            Controls.Add(nameLine);


            previousListButton = new Button();
            previousListButton.Text = "Назад";
            previousListButton.Location = new Point(20 + nameLine.Location.X, nameLine.Location.Y + nameLine.Height + 15);
            previousListButton.Enabled = false;
            previousListButton.Click += PreviousNext_Click;
            Controls.Add(previousListButton);

            nextListButton = new Button();
            nextListButton.Text = "Вперед";
            nextListButton.Location = new Point(20 + previousListButton.Location.X + previousListButton.Width, previousListButton.Location.Y);
            nextListButton.Enabled = false;
            nextListButton.Click += PreviousNext_Click;
            Controls.Add(nextListButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            var calculation = calculationValue.Tag;
            if (calculation == null)
            {
                MessageBox.Show("Не выбрана калькуляция", "Внимание");
                return;
            }
            try
            {
                var quantity = inputOperations.GetPositiveInt(this.quantityValue.Text);
                var date = inputOperations.GetCorrectData(dateValue.Text);
                var calculationItem = itemsFactory.CreatePreOrder((ICalculation)calculation, quantity, date) as EditingBlockItemDB;
                await calculationItem.AddItemToDataBase();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
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
            var maxShowItemsCount = creator.LengthOfItemsList;
            var resultOfGettingItemsList = await creator.GetItemsList(offset, nameLine.Text.ToLower());
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            _offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    _offset = offset;

                    resultOfGettingItemsList = await creator.GetItemsList(offset, nameLine.Text.ToLower());
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
                nextListButton.Enabled = true;
                nextListButton.Tag = offset + maxShowItemsCount;
            }
            else
                DefaultNextButton();

            if (offset > 0)
            {
                previousListButton.Enabled = true;
                previousListButton.Tag = offset - maxShowItemsCount;
            }
            else
                DefaultPreviousButton();

            RefreshActiveItemTempControl(itemsList);
        }

        private void RefreshActiveItemTempControl(List<IBlockItem> itemsList)
        {
            var lastControl = (Control)previousListButton;

            if (activeItemTempControls.Count > 0) ClearActiveItemTempControls();
            foreach (var item in itemsList)
                lastControl = DisplayItems(item, lastControl);
        }

        private void ShowEmptyList(string message)
        {
            DefaultPreviousNextButtons();
            if (activeItemTempControls.Count > 0) ClearActiveItemTempControls();
            var lastControl = previousListButton;
            var label = new Label();
            label.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 25);
            label.Text = message;
            label.AutoSize = true;
            Controls.Add(label);
            activeItemTempControls.Add(label);
        }

        private Control DisplayItems(IBlockItem item, Control lastControl)
        {
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 10);
            itemLabel.AutoSize = true;
            itemLabel.Click += ItemLabel_Click;
            Controls.Add(itemLabel);
            activeItemTempControls.Add(itemLabel);

            itemLabel.Text = item.Name;
            itemLabel.Tag = item;

            return itemLabel;
        }
        private void ItemLabel_Click(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var calculation = (ICalculation)label.Tag;
            this.calculationValue.Tag = calculation;
            this.calculationValue.Text = calculation.Name;
            nameLine.Text = "";
        }

        private void ClearActiveItemTempControls()
        {
            foreach (var activeItemControl in activeItemTempControls)
            {
                Controls.Remove(activeItemControl);
            }
            activeItemTempControls.Clear();
        }

        private void DefaultPreviousButton()
        {
            previousListButton.Enabled = false;
            previousListButton.Tag = null;
        }
        private void DefaultNextButton()
        {
            nextListButton.Enabled = false;
            nextListButton.Tag = null;
        }
        private void DefaultPreviousNextButtons()
        {
            DefaultPreviousButton();
            DefaultNextButton();
        }
    }
}
