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
using ManagementAccounting.Forms.RemaindersForms;
using ManagementAccounting.Interfaces.Factory;
using Npgsql;

namespace ManagementAccounting
{
    public partial class MaterialForm : Form
    {
        private int Offset { get; set; }
        private Label NameValue { get; }
        private Label TypeValue { get; }
        private TextBox NameLine { get; }
        private CheckBox CheckBox { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button AddReceivingButton { get; }
        private Button EditMaterialButton { get; }
        private Button RemoveMaterialButton { get; }
        private Button CloseButton { get; }
        private bool IsCallFromOtherBlocks { get; }
        private List<Control> ActiveItemTempControls { get; }
        private IMaterial Material { get; }
        private IFormFactory FormFactory{ get; }
        private MaterialReceivingCollectionCreator Creator { get; }
        private MaterialReceivingNotEmptyCollectionCreator CreatorNotEmpty { get; }
        private IOperationsWithUserInput InputOperations { get; }

        public MaterialForm(IMaterial material, bool isCallFromOtherBlocks, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory)
        {
            CreatorNotEmpty = creatorFactory.CreateMaterialReceivingNotEmptyCreator(material, 5);
            Creator = creatorFactory.CreateMaterialReceivingCreator(material, 5);
            Material = material;
            InputOperations = inputOperations;
            FormFactory = formFactory;
            IsCallFromOtherBlocks = isCallFromOtherBlocks;
            ActiveItemTempControls = new List<Control>();

            Size = new Size(400, 600);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Материал";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Text = "Наименование:";
            nameLabel.Width = 100;
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 20);
            Controls.Add(nameLabel);

            NameValue = new Label();
            NameValue.Text = material.Name;
            NameValue.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            Controls.Add(NameValue);

            var typeLabel = new Label();
            typeLabel.Text = "Тип:";
            typeLabel.Width = 100;
            typeLabel.Location = new Point(10, nameLabel.Location.Y + nameLabel.Height + 20);
            Controls.Add(typeLabel);

            TypeValue = new Label();
            TypeValue.Text = inputOperations.TranslateType(material.MaterialType.ToString());
            TypeValue.Location = new Point(typeLabel.Location.X + typeLabel.Width + 10, typeLabel.Location.Y);
            Controls.Add(TypeValue);

            AddReceivingButton = new Button();
            AddReceivingButton.Text = "Добавить пост-е";
            AddReceivingButton.AutoSize = true;
            AddReceivingButton.Location = new Point(typeLabel.Location.X, typeLabel.Location.Y + typeLabel.Height + 15);
            AddReceivingButton.Click += AddReceivingButton_Click;
            Controls.Add(AddReceivingButton);

            EditMaterialButton = new Button();
            EditMaterialButton.Text = "Изменить мат-л";
            EditMaterialButton.AutoSize = true;
            EditMaterialButton.Location = new Point(AddReceivingButton.Location.X + AddReceivingButton.Width + 15,
                AddReceivingButton.Location.Y);
            EditMaterialButton.Click += EditMaterialButtonOnClick;
            Controls.Add(EditMaterialButton);

            RemoveMaterialButton = new Button();
            RemoveMaterialButton.Text = "Удалить мат-л";
            RemoveMaterialButton.AutoSize = true;
            RemoveMaterialButton.Location = new Point(EditMaterialButton.Location.X + EditMaterialButton.Width + 15,
                EditMaterialButton.Location.Y);
            RemoveMaterialButton.Click += RemoveMaterialButtonOnClick;
            Controls.Add(RemoveMaterialButton);

            CloseButton = new Button();
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Location = new Point(RemoveMaterialButton.Location.X + RemoveMaterialButton.Width + 15, RemoveMaterialButton.Location.Y);
            CloseButton.Click += (sender, args) => Close();
            Controls.Add(CloseButton);

            NameLine = new TextBox();
            NameLine.Location = new Point(AddReceivingButton.Location.X, AddReceivingButton.Location.Y + AddReceivingButton.Height + 15);
            NameLine.Width = 300;
            NameLine.TextChanged += NameLine_TextChanged;
            Controls.Add(NameLine);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + NameLine.Location.X, NameLine.Location.Y + NameLine.Height + 15);
            PreviousListButton.Enabled = false;
            PreviousListButton.Click += PreviousNext_Click;
            Controls.Add(PreviousListButton);

            NextListButton = new Button();
            NextListButton.Text = "Вперед";
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
            NextListButton.Enabled = false;
            NextListButton.Click += PreviousNext_Click;
            Controls.Add(NextListButton);

            CheckBox = new CheckBox();
            CheckBox.Text = "есть на складе";
            CheckBox.AutoSize = true;
            CheckBox.Location = new Point(20 + NextListButton.Location.X + NextListButton.Width, NextListButton.Location.Y);
            CheckBox.CheckedChanged += NameLine_TextChanged;
            Controls.Add(CheckBox);

            if (isCallFromOtherBlocks)
            {
                AddReceivingButton.Enabled = false;
                EditMaterialButton.Enabled = false;
                RemoveMaterialButton.Enabled = false;
            }

        }

        private async void RemoveMaterialButtonOnClick(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить материал?", "Внимание", MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.No) return;
            try
            {
                await ((BlockItemDB)Material).RemoveItemFromDataBase();
            }
            catch(NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }

            Close();
        }

        private void EditMaterialButtonOnClick(object sender, EventArgs e)
        {
            var editForm = FormFactory.CreateEditMaterialForm(Material);
            editForm.ShowDialog();
            NameValue.Text = Material.Name;
            TypeValue.Text = InputOperations.TranslateType(Material.MaterialType.ToString());
        }

        private async void AddReceivingButton_Click(object sender, EventArgs e)
        {
            var addReceivingForm = FormFactory.CreateAddMaterialReceivingForm(Material);
            addReceivingForm.ShowDialog();
            
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
            var block = CheckBox.Checked ? CreatorNotEmpty : Creator;
            var maxShowItemsCount = block.LengthOfItemsList;
            var resultOfGettingItemsList = await block.GetItemsList(offset, NameLine.Text.ToLower());
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            Offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    Offset = offset;

                    resultOfGettingItemsList = await block.GetItemsList(offset, NameLine.Text.ToLower());
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
        private async void ItemLabel_Click(object sender, EventArgs e)
        {
            var control = (Control) sender;
            var receiving = (IMaterialReceiving) control.Tag;
            var receivingForm = FormFactory.CreateMaterialReceivingForm(receiving, IsCallFromOtherBlocks);
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
