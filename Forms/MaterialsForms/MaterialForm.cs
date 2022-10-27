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

namespace ManagementAccounting
{
    public partial class MaterialForm : Form
    {
        private int _offset { get; set; }
        private Label nameLabel { get; }
        private Label typeLabel { get; }
        private TextBox nameLine { get; }
        private CheckBox checkBox { get; }
        private Button nextListButton { get; }
        private Button previousListButton { get; }
        private Button addReceivingButton { get; }
        private Button editMaterialButton { get; }
        private Button removeMaterialButton { get; }
        private List<Control> activeItemTempControls { get; }
        private IMaterial _material { get; }
        private IFormFactory formFactory{ get; }
        private MaterialReceivingCollectionCreator creator { get; }
        private MaterialReceivingNotEmptyCollectionCreator creatorNotEmpty { get; }
        private IOperationsWithUserInput _inputOperations { get; }

        public MaterialForm(IMaterial material, ICreatorFactory creatorFactory, IOperationsWithUserInput inputOperations, IFormFactory formFactory)
        {
            creatorNotEmpty = creatorFactory.CreateMaterialReceivingNotEmptyCreator(material, 5);
            creator = creatorFactory.CreateMaterialReceivingCreator(material, 5);
            _material = material;
            _inputOperations = inputOperations;
            this.formFactory = formFactory;
            activeItemTempControls = new List<Control>();

            Size = new Size(400, 600);

            nameLabel = new Label();
            nameLabel.Text = material.Name;
            nameLabel.Location = new Point(10, 10);
            Controls.Add(nameLabel);

            typeLabel = new Label();
            typeLabel.Text = inputOperations.TranslateType(material.MaterialType.ToString());
            typeLabel.Location = new Point(nameLabel.Location.X, nameLabel.Location.Y + nameLabel.Height + 15);
            Controls.Add(typeLabel);

            addReceivingButton = new Button();
            addReceivingButton.Text = "Добавить поступление";
            addReceivingButton.Location = new Point(typeLabel.Location.X, typeLabel.Location.Y + typeLabel.Height + 15);
            addReceivingButton.Click += AddReceivingButton_Click;
            Controls.Add(addReceivingButton);

            editMaterialButton = new Button();
            editMaterialButton.Text = "Изменить материал";
            editMaterialButton.Location = new Point(addReceivingButton.Location.X + addReceivingButton.Width + 15,
                addReceivingButton.Location.Y);
            editMaterialButton.Click += EditMaterialButtonOnClick;
            Controls.Add(editMaterialButton);

            removeMaterialButton = new Button();
            removeMaterialButton.Text = "Удалить материал";
            removeMaterialButton.Location = new Point(editMaterialButton.Location.X + editMaterialButton.Width + 15,
                editMaterialButton.Location.Y);
            removeMaterialButton.Click += RemoveMaterialButtonOnClick;
            Controls.Add(removeMaterialButton);

            nameLine = new TextBox();
            nameLine.Location = new Point(addReceivingButton.Location.X, addReceivingButton.Location.Y + addReceivingButton.Height + 15);
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

            checkBox = new CheckBox();
            checkBox.Text = "есть на складе";
            checkBox.AutoSize = true;
            checkBox.Location = new Point(20 + nextListButton.Location.X + nextListButton.Width, nextListButton.Location.Y);
            checkBox.CheckedChanged += NameLine_TextChanged;
            Controls.Add(checkBox);

        }

        private async void RemoveMaterialButtonOnClick(object? sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить материал?", "Внимание", MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.No) return;
            await ((BlockItemDB)_material).RemoveItemFromDataBase();
            Close();
        }

        private void EditMaterialButtonOnClick(object? sender, EventArgs e)
        {
            var editForm = formFactory.CreateEditMaterialForm(_material);
            editForm.ShowDialog();
            nameLabel.Text = _material.Name;
            typeLabel.Text = _inputOperations.TranslateType(_material.MaterialType.ToString());
        }

        private void AddReceivingButton_Click(object sender, EventArgs e)
        {
            //var addReceivingForm = new AddMaterialReceivingForm(_material, _inputOperations);
            var addReceivingForm = formFactory.CreateAddMaterialReceivingForm(_material);

            addReceivingForm.ShowDialog();
            ShowItems(_offset);
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
            var block = checkBox.Checked ? creator : creatorNotEmpty;
            var maxShowItemsCount = block.LengthOfItemsList;
            var resultOfGettingItemsList = await block.GetItemsList(offset, nameLine.Text.ToLower());
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            //var itemsList = checkBox.Checked ? await _material.GetItemsList(offset, nameLine.Text) : await _material.GetItemsList(offset, "all", nameLine.Text);
            _offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    _offset = offset;

                    resultOfGettingItemsList = await block.GetItemsList(offset, nameLine.Text.ToLower());
                    itemsList = resultOfGettingItemsList.Item1;
                    isThereMoreOfItems = resultOfGettingItemsList.Item2;
                    //itemsList = checkBox.Checked ? await _material.GetItemsList(offset, nameLine.Text) : await _material.GetItemsList(offset, "all", nameLine.Text);
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
            var control = (Control) sender;
            var receiving = (IMaterialReceiving) control.Tag;
            var receivingForm = formFactory.CreateMaterialReceivingForm(receiving);
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
