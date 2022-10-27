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
        private int _offset { get; set; }
        private Label nameLabel { get; }
        private Label quantityLabel { get; }
        private Label dateLabel { get; }
        private Label minPrice { get; }
        private Label maxPrice { get; }
        private Label isAvailablePrice { get; }




        //private TextBox nameLine { get; }
        private Button nextListButton { get; }
        private Button previousListButton { get; }
        private Button calculateItemButton { get; }
        private Button editPreOrderButton { get; }
        private Button removePreOrderButton { get; }
        private List<Control> activeItemTempControls { get; }
        private IPreOrder preOrder { get; }
        private IFormFactory formFactory { get; }
        private BlockItemsCollectionCreator creator { get; }
        private IOperationsWithUserInput _inputOperations { get; }
        private IPreOrderCostPrice preOrderCostPrice { get; }

        public PreOrderForm(IPreOrder preOrder, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory, IPreOrderCostPrice preOrderCostPrice)
        {
            this.preOrderCostPrice = preOrderCostPrice;
            this.preOrder = preOrder;
            _inputOperations = inputOperations;
            this.formFactory = formFactory;
            creator = creatorFactory.CreatePreOrderItemCollectionCreator(preOrder, 5);
            activeItemTempControls = new List<Control>();

            Size = new Size(900, 600);

            nameLabel = new Label();
            nameLabel.Text = preOrder.Name;
            nameLabel.Width = 200;
            //nameLabel.Click += NameLine_TextChanged;
            nameLabel.Location = new Point(10, 10);
            Controls.Add(nameLabel);

            quantityLabel = new Label();
            quantityLabel.Text = preOrder.Quantity.ToString();
            quantityLabel.Width = 200;
            quantityLabel.Location = new Point(nameLabel.Location.X, nameLabel.Location.Y + nameLabel.Height + 15);
            Controls.Add(quantityLabel);

            dateLabel = new Label();
            dateLabel.Text = preOrder.CreationDate.ToString();
            dateLabel.Width = 200;
            dateLabel.Location = new Point(quantityLabel.Location.X, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(dateLabel);

            minPrice = new Label();
            minPrice.Width = 200;
            minPrice.Location = new Point(dateLabel.Location.X, dateLabel.Location.Y + dateLabel.Height + 15);
            Controls.Add(minPrice);

            maxPrice = new Label();
            maxPrice.Width = 200;
            maxPrice.Location = new Point(minPrice.Location.X, minPrice.Location.Y + minPrice.Height + 15);
            Controls.Add(maxPrice);

            isAvailablePrice = new Label();
            isAvailablePrice.Width = 200;
            isAvailablePrice.Location = new Point(maxPrice.Location.X, maxPrice.Location.Y + maxPrice.Height + 15);
            Controls.Add(isAvailablePrice);

            calculateItemButton = new Button();
            calculateItemButton.Text = "Рассчитать";
            calculateItemButton.AutoSize = true;
            calculateItemButton.Location = new Point(isAvailablePrice.Location.X, isAvailablePrice.Location.Y + isAvailablePrice.Height + 15);
            calculateItemButton.Click += CalculateItemButton_Click;
            Controls.Add(calculateItemButton);

            editPreOrderButton = new Button();
            editPreOrderButton.Text = "Изменить предзаказ";
            editPreOrderButton.Location = new Point(calculateItemButton.Location.X + calculateItemButton.Width + 15,
                calculateItemButton.Location.Y);
            editPreOrderButton.Click += EditPreOrderButtonOnClick;
            Controls.Add(editPreOrderButton);

            removePreOrderButton = new Button();
            removePreOrderButton.Text = "Удалить предзаказ";
            removePreOrderButton.Location = new Point(editPreOrderButton.Location.X + editPreOrderButton.Width + 15,
                editPreOrderButton.Location.Y);
            removePreOrderButton.Click += RemovePreOrderButtonOnClick;
            Controls.Add(removePreOrderButton);

            //nameLine = new TextBox();
            //nameLine.Location = new Point(calculateItemButton.Location.X, calculateItemButton.Location.Y + calculateItemButton.Height + 15);
            //nameLine.Width = 300;
            //nameLine.TextChanged += NameLine_TextChanged;
            //Controls.Add(nameLine);

            previousListButton = new Button();
            previousListButton.Text = "Назад";
            previousListButton.Location = new Point(20 + calculateItemButton.Location.X, calculateItemButton.Location.Y + calculateItemButton.Height + 15);
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

        private async void RemovePreOrderButtonOnClick(object? sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить предзаказ?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;
            await ((BlockItemDB)preOrder).RemoveItemFromDataBase();
            Close();
        }

        private void EditPreOrderButtonOnClick(object? sender, EventArgs e)
        {
            var editForm = formFactory.CreateEditPreOrderForm(preOrder);
            editForm.ShowDialog();
            nameLabel.Text = preOrder.Name;
            dateLabel.Text = preOrder.CreationDate.ToShortDateString();
            quantityLabel.Text = preOrder.Quantity.ToString();
        }

        private async void CalculateItemButton_Click(object sender, EventArgs e)
        {
            var result = await preOrderCostPrice.GetPreOrderCostPrice(preOrder);
            minPrice.Text = result.Item1.ToString();
            maxPrice.Text = result.Item2.ToString();
            isAvailablePrice.Text = result.Item3 ? "Цена актуальна" : "Материалов не хватает";

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
            var maxShowItemsCount = creator.LengthOfItemsList;
            var resultOfGettingItemsList = await creator.GetItemsList(offset, "");
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            _offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    _offset = offset;

                    resultOfGettingItemsList = await creator.GetItemsList(offset, "");
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
            itemLabel.Width = 200;
            //itemLabel.AutoSize = true;
            //itemLabel.Click += ItemLabel_Click;
            Controls.Add(itemLabel);
            activeItemTempControls.Add(itemLabel);

            itemLabel.Text = item.Name;
            itemLabel.Tag = item;

            var preOrderItem = (IPreOrderItem) itemLabel.Tag;
            var minUnitC = new Label();
            minUnitC.Text = preOrderItem.MaterialUnitСonsumption.ToString();
            minUnitC.Location = new Point(itemLabel.Width + itemLabel.Location.X + 10, itemLabel.Location.Y);
            minUnitC.Width = 50;
            Controls.Add(minUnitC);
            activeItemTempControls.Add(minUnitC);

            var minUniyP = new Label();
            minUniyP.Text = preOrderItem.MinUnitPrice.ToString();
            minUniyP.Location = new Point(minUnitC.Width + minUnitC.Location.X + 10, minUnitC.Location.Y);
            minUniyP.Width = 50;
            Controls.Add(minUniyP);
            activeItemTempControls.Add(minUniyP);

            var maxUniyP = new Label();
            maxUniyP.Text = preOrderItem.MaxUnitPrice.ToString();
            maxUniyP.Location = new Point(minUniyP.Width + minUniyP.Location.X + 10, minUniyP.Location.Y);
            maxUniyP.Width = 50;
            Controls.Add(maxUniyP);
            activeItemTempControls.Add(maxUniyP);

            var minC = new Label();
            minC.Text = preOrderItem.MaterialСonsumption.ToString();
            minC.Location = new Point(maxUniyP.Width + maxUniyP.Location.X + 10, maxUniyP.Location.Y);
            minC.Width = 50;
            Controls.Add(minC);
            activeItemTempControls.Add(minC);

            var minP = new Label();
            minP.Text = preOrderItem.MinPrice.ToString();
            minP.Location = new Point(minC.Width + minC.Location.X + 10, minC.Location.Y);
            minP.Width = 50;
            Controls.Add(minP);
            activeItemTempControls.Add(minP);

            var maxP = new Label();
            maxP.Text = preOrderItem.MaxPrice.ToString();
            maxP.Location = new Point(minP.Width + minP.Location.X + 10, minP.Location.Y);
            maxP.Width = 50;
            Controls.Add(maxP);
            activeItemTempControls.Add(maxP);


            return itemLabel;
        }

        //private void ItemLabel_Click(object sender, EventArgs e)
        //{
        //    var control = (Control)sender;
        //    var calculationItem = (ICalculationItem)control.Tag;
        //    var receivingForm = formFactory.CreateCalculationItemForm(calculationItem);
        //    receivingForm.ShowDialog();

        //    ShowItems(_offset);
        //}


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
