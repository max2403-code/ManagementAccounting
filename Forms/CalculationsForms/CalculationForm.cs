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
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class CalculationForm : Form
    {
        private int _offset { get; set; }
        private Label nameLabel { get; }
        //private TextBox nameLine { get; }
        private Button nextListButton { get; }
        private Button previousListButton { get; }
        private Button addCalculationItemButton { get; }
        private Button editCalculationButton { get; }
        private Button removeCalculationButton { get; }
        private List<Control> activeItemTempControls { get; }
        private ICalculation calculation { get; }
        private IFormFactory formFactory { get; }
        private CalculationItemCollectionCreator creator { get; }
        private IOperationsWithUserInput _inputOperations { get; }

        public CalculationForm(ICalculation calculation, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory)
        {
            this.calculation = calculation;
            _inputOperations = inputOperations;
            this.formFactory = formFactory;
            creator = creatorFactory.CreateCalculationItemCollectionCreator(calculation, 5);
            activeItemTempControls = new List<Control>();

            Size = new Size(400, 600);

            nameLabel = new Label();
            nameLabel.Text = calculation.Name;
            nameLabel.Width = 200;
            nameLabel.Click += NameLine_TextChanged;
            nameLabel.Location = new Point(10, 10);
            Controls.Add(nameLabel);

            

            addCalculationItemButton = new Button();
            addCalculationItemButton.Text = "Добавить статью калькуляции";
            addCalculationItemButton.Location = new Point(nameLabel.Location.X, nameLabel.Location.Y + nameLabel.Height + 15);
            addCalculationItemButton.Click += AddCalculationItemButton_Click;
            Controls.Add(addCalculationItemButton);

            editCalculationButton = new Button();
            editCalculationButton.Text = "Изменить калькуляцию";
            editCalculationButton.Location = new Point(addCalculationItemButton.Location.X + addCalculationItemButton.Width + 15,
                addCalculationItemButton.Location.Y);
            editCalculationButton.Click += EditCalculationButtonOnClick;
            Controls.Add(editCalculationButton);

            removeCalculationButton = new Button();
            removeCalculationButton.Text = "Удалить калькуляцию";
            removeCalculationButton.Location = new Point(editCalculationButton.Location.X + editCalculationButton.Width + 15,
                editCalculationButton.Location.Y);
            removeCalculationButton.Click += RemoveCalculationButtonOnClick;
            Controls.Add(removeCalculationButton);

            //nameLine = new TextBox();
            //nameLine.Location = new Point(addCalculationItemButton.Location.X, addCalculationItemButton.Location.Y + addCalculationItemButton.Height + 15);
            //nameLine.Width = 300;
            //nameLine.TextChanged += NameLine_TextChanged;
            //Controls.Add(nameLine);

            previousListButton = new Button();
            previousListButton.Text = "Назад";
            previousListButton.Location = new Point(20 + addCalculationItemButton.Location.X, addCalculationItemButton.Location.Y + addCalculationItemButton.Height + 15);
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

        private async void RemoveCalculationButtonOnClick(object? sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить калькуляцию?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;
            await ((BlockItemDB)calculation).RemoveItemFromDataBase();
            Close();
        }

        private void EditCalculationButtonOnClick(object? sender, EventArgs e)
        {
            var editForm = formFactory.CreateEditCalculationForm(calculation);
            editForm.ShowDialog();
            nameLabel.Text = calculation.Name;
        }

        private async void AddCalculationItemButton_Click(object sender, EventArgs e)
        {
            var addCalculationItemForm = formFactory.CreateAddCalculationItemForm(calculation);

            addCalculationItemForm.ShowDialog();
            await ShowItems(_offset);
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
            itemLabel.Click += ItemLabel_Click;
            Controls.Add(itemLabel);
            activeItemTempControls.Add(itemLabel);

            itemLabel.Text = item.Name;
            itemLabel.Tag = item;

            var consumptionLabel = new Label();
            consumptionLabel.Text = ((ICalculationItem) itemLabel.Tag).Consumption.ToString();
            consumptionLabel.Location = new Point(itemLabel.Width + itemLabel.Location.X + 10, itemLabel.Location.Y );
            consumptionLabel.Width = 50;
            Controls.Add(consumptionLabel);
            activeItemTempControls.Add(consumptionLabel);
            return itemLabel;
        }

        private void ItemLabel_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var calculationItem = (ICalculationItem)control.Tag;
            var receivingForm = formFactory.CreateCalculationItemForm(calculationItem);
            receivingForm.ShowDialog();

            ShowItems(_offset);
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
