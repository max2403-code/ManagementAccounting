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

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class AddCalculationItemForm : Form
    {
        private BlockItemsCollectionCreator Creator { get; }
        private TextBox NameLine { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button AddButton { get; }
        private Button CloseButton { get; }
        private Button AllMaterials { get; }
        private Label MaterialValue { get; }
        private TextBox QuantityValue { get; }
        private Label QuantityLabel { get; }


        //private List<Button> Buttons { get; }
        private List<Control> ActiveItemTempControls { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IItemsFactory ItemsFactory { get; }
        private ICalculation Calculation { get; }


        public AddCalculationItemForm(ICalculation calculation, IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory, ICreatorFactory creatorFactory)
        {
            this.ItemsFactory = itemsFactory;
            this.InputOperations = inputOperations;
            this.Calculation = calculation;
            //Buttons = new List<Button>();
            Creator = creatorFactory.CreateMaterialCollectionCreator(5);
            ActiveItemTempControls = new List<Control>();
            Size = new Size(500, 500);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление статьи калькуляции";
            Controls.Add(topLabel);

            var materialLabel = new Label();
            materialLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            materialLabel.Width = 130;
            materialLabel.Text = "Наименование мат-а:";
            Controls.Add(materialLabel);

            MaterialValue = new Label();
            MaterialValue.Location = new Point(materialLabel.Location.X + materialLabel.Width + 10, materialLabel.Location.Y);
            MaterialValue.Width = 200;
            Controls.Add(MaterialValue);

            QuantityLabel = new Label();
            QuantityLabel.Location = new Point(10, materialLabel.Location.Y + materialLabel.Height + 50);
            QuantityLabel.Width = 130;
            QuantityLabel.Text = "Норма расхода:";
            Controls.Add(QuantityLabel);

            QuantityValue = new TextBox();
            QuantityValue.Location = new Point(QuantityLabel.Location.X + QuantityLabel.Width + 10, QuantityLabel.Location.Y);
            QuantityValue.Width = 200;
            Controls.Add(QuantityValue);

            

            AddButton = new Button();
            AddButton.Location = new Point(50, QuantityLabel.Location.Y + QuantityLabel.Height + 25);
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

            AllMaterials = new Button();
            AllMaterials.Text = "Все материалы";
            AllMaterials.AutoSize = true;
            AllMaterials.Location = new Point(NameLine.Location.X, NameLine.Location.Y + NameLine.Height + 15);
            AllMaterials.Click += AllMaterialsOnClick;
            Controls.Add(AllMaterials);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + AllMaterials.Location.X + AllMaterials.Width, NameLine.Location.Y + NameLine.Height + 15);
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

        private async void AllMaterialsOnClick(object sender, EventArgs e)
        {
            NameLine.Text = "";
            await ShowItems(0);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var material = MaterialValue.Tag;
            var isQuantityCorrect = InputOperations.TryGetPositiveDecimal(QuantityValue.Text, out var quantity);

            if (material == null || !isQuantityCorrect)
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                EnableButtons();
                return;
            }
            try
            {
                var calculationItem = ItemsFactory.CreateCalculationItem((IMaterial)material, quantity, Calculation.Index) as EditingBlockItemDB;
                await calculationItem.AddItemToDataBase();
            }
            catch (Exception exception)
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

            await ShowItems(offset);
        }

        private async void NameLine_TextChanged(object sender, EventArgs e)
        {
            if (NameLine.Text == "")
            {
                ShowEmptyList("Введите название материала");
                return;
            }
            await ShowItems(0);
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
            label.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
            label.Text = message;
            label.AutoSize = true;
            Controls.Add(label);
            ActiveItemTempControls.Add(label);
        }

        private Control DisplayItems(IBlockItem item, Control lastControl)
        {
            var itemLabel = new Label();
            itemLabel.Location = new Point(10, lastControl.Location.Y + lastControl.Height + 15);
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
            var material = (IMaterial)label.Tag;
            MaterialValue.Tag = material;
            MaterialValue.Text = material.Name;
            QuantityLabel.Text = $"Норма расхода, {InputOperations.TranslateType(material.Unit.ToString())}:";
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
            foreach (var control in Controls)
            {
                if (control is Button button && !(button.Equals(NextListButton)  || button.Equals(PreviousListButton) ))
                    button.Enabled = true;
            }
        }

        private void DisableButtons()
        {
            foreach (var control in Controls)
            {
                if (control is Button button && !(button.Equals(NextListButton) || button.Equals(PreviousListButton)))
                    button.Enabled = false;
            }
        }
    }
}
