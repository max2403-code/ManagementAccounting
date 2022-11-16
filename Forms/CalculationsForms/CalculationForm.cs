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
using Npgsql;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class CalculationForm : Form
    {
        private int Offset { get; set; }
        private Label NameValue { get; }
        private Label TableColumnName { get; }

        //private TextBox nameLine { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button AddCalculationItemButton { get; }
        private Button EditCalculationButton { get; }
        private Button ShowCalculationItemButton { get; }
        private Button RemoveCalculationButton { get; }
        private Button CloseButton { get; }

        private List<Control> ActiveItemTempControls { get; }
        private ICalculation Calculation { get; }
        private IFormFactory FormFactory { get; }
        private CalculationItemCollectionCreator Creator { get; }
        private IOperationsWithUserInput InputOperations { get; }

        public CalculationForm(ICalculation calculation, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory)
        {
            Calculation = calculation;
            InputOperations = inputOperations;
            FormFactory = formFactory;
            Creator = creatorFactory.CreateCalculationItemCollectionCreator(calculation, 5);
            ActiveItemTempControls = new List<Control>();

            Size = new Size(500, 600);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Калькуляция";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Text = "Наименование:";
            nameLabel.Width = 100;
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 20);
            Controls.Add(nameLabel);

            NameValue = new Label();
            NameValue.Text = calculation.Name;
            NameValue.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            Controls.Add(NameValue);

            //nameLabel = new Label();
            //nameLabel.Text = calculation.Name;
            //nameLabel.Width = 200;
            //nameLabel.Click += NameLine_TextChanged;
            //nameLabel.Location = new Point(10, 10);
            //Controls.Add(nameLabel);

            ShowCalculationItemButton = new Button();
            ShowCalculationItemButton.Text = "Показать статьи калькуляции";
            ShowCalculationItemButton.AutoSize = true;
            ShowCalculationItemButton.Location = new Point(nameLabel.Location.X, nameLabel.Location.Y + nameLabel.Height + 15);
            ShowCalculationItemButton.Click += NameLine_TextChanged;
            Controls.Add(ShowCalculationItemButton);

            AddCalculationItemButton = new Button();
            AddCalculationItemButton.Text = "Добавить статью кальк-и";
            AddCalculationItemButton.AutoSize = true;
            AddCalculationItemButton.Location = new Point(ShowCalculationItemButton.Location.X, ShowCalculationItemButton.Location.Y + ShowCalculationItemButton.Height + 15);
            AddCalculationItemButton.Click += AddCalculationItemButton_Click;
            Controls.Add(AddCalculationItemButton);

            EditCalculationButton = new Button();
            EditCalculationButton.Text = "Изменить кальк-ю";
            EditCalculationButton.AutoSize = true;
            EditCalculationButton.Location = new Point(AddCalculationItemButton.Location.X + AddCalculationItemButton.Width + 15,
                AddCalculationItemButton.Location.Y);
            EditCalculationButton.Click += EditCalculationButtonOnClick;
            Controls.Add(EditCalculationButton);

            RemoveCalculationButton = new Button();
            RemoveCalculationButton.Text = "Удалить кальк-ю";
            RemoveCalculationButton.AutoSize = true;
            RemoveCalculationButton.Location = new Point(EditCalculationButton.Location.X + EditCalculationButton.Width + 15,
                EditCalculationButton.Location.Y);
            RemoveCalculationButton.Click += RemoveCalculationButtonOnClick;
            Controls.Add(RemoveCalculationButton);

            //nameLine = new TextBox();
            //nameLine.Location = new Point(addCalculationItemButton.Location.X, addCalculationItemButton.Location.Y + addCalculationItemButton.Height + 15);
            //nameLine.Width = 300;
            //nameLine.TextChanged += NameLine_TextChanged;
            //Controls.Add(nameLine);

            CloseButton = new Button();
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Location = new Point(RemoveCalculationButton.Location.X + RemoveCalculationButton.Width + 15, RemoveCalculationButton.Location.Y);
            CloseButton.Click += (sender, args) => Close();
            Controls.Add(CloseButton);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + AddCalculationItemButton.Location.X, AddCalculationItemButton.Location.Y + AddCalculationItemButton.Height + 15);
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
        }

        private async void RemoveCalculationButtonOnClick(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить калькуляцию?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;
            try
            {
                await ((BlockItemDB)Calculation).RemoveItemFromDataBase();
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
        }

        private void EditCalculationButtonOnClick(object sender, EventArgs e)
        {
            var editForm = FormFactory.CreateEditCalculationForm(Calculation);
            editForm.ShowDialog();
            NameValue.Text = Calculation.Name;
        }

        private async void AddCalculationItemButton_Click(object sender, EventArgs e)
        {
            var addCalculationItemForm = FormFactory.CreateAddCalculationItemForm(Calculation);

            addCalculationItemForm.ShowDialog();
            
            try
            {
                await ShowItems(Offset);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
            }
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
            }
        }

        private async void NameLine_TextChanged(object sender, EventArgs e)
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

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = Creator.LengthOfItemsList;
            var resultOfGettingItemsList = await Creator.GetItemsList(offset, "");
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            Offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    Offset = offset;

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
            label.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
            label.Text = message;
            label.AutoSize = true;
            Controls.Add(label);
            ActiveItemTempControls.Add(label);
        }

        private Control DisplayItems(IBlockItem item, Control lastControl)
        {
            var calculationItem = (ICalculationItem) item;
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
            itemLabel.MaximumSize = new Size(200, 0);
            itemLabel.AutoSize = true;
            itemLabel.Click += ItemLabel_Click;
            Controls.Add(itemLabel);
            ActiveItemTempControls.Add(itemLabel);

            itemLabel.Text = calculationItem.Name;
            itemLabel.Tag = calculationItem;

            var consumptionLabel = new Label();
            consumptionLabel.Text = string.Join(' ', calculationItem.Consumption.ToString(), InputOperations.TranslateType(calculationItem.Material.Unit.ToString())) ;
            consumptionLabel.Location = new Point(200 + itemLabel.Location.X + 10, itemLabel.Location.Y );
            consumptionLabel.AutoSize = true;
            Controls.Add(consumptionLabel);
            ActiveItemTempControls.Add(consumptionLabel);

            return itemLabel;
        }

        private async void ItemLabel_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var calculationItem = (ICalculationItem)control.Tag;
            var receivingForm = FormFactory.CreateCalculationItemForm(calculationItem);
            receivingForm.ShowDialog();

            try
            {
                await ShowItems(Offset);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
            }
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
