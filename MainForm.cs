using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagementAccounting
{
    public partial class MainForm : Form
    {
        public bool LoginCompleted { get; set; }
    
        private IBlockItemFormsCollection _formsCollection { get; }


        private Label label { get; }
        private Button remaindersButton { get; }
        private List<Button> mainButtonsHashSet { get; }
        private List<Control> activeTempControls { get; }
        private List<Control> activeItemTempControls { get; }
        private Button nextListButton { get; }
        private Button previousListButton { get; }
        private Button allItems { get; }
        private Button addItem { get; }
        private TextBox searchNameLine { get; set; }
        private Button signIn { get; } 

        private int _offset { get; set; }


        private IOperationsWithUserInput _inputOperations { get; }
        private IDataBase _dataBase { get; }
        private IProgramBlock activeBlock { get; set; }
         
        public MainForm(IProgramBlock[] blocks, IDataBase dataBase, IBlockItemFormsCollection formsCollection, IOperationsWithUserInput inputOperations)
        {
            _dataBase = dataBase;

            _inputOperations = inputOperations;
            _formsCollection = formsCollection;

            mainButtonsHashSet = new List<Button>();
            activeTempControls = new List<Control>();
            activeItemTempControls = new List<Control>();

            AutoScroll = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Size = new Size(600, 600);

            signIn = new Button();
            signIn.AutoSize = true;
            signIn.Click += SignInOnClick;
            signIn.Text = "Войти в базу данных";
            Controls.Add(signIn);

            remaindersButton = new Button();
            remaindersButton.Location = new Point(signIn.Location.X + signIn.Width + 10, signIn.Location.Y);
            remaindersButton.Enabled = false;
            remaindersButton.AutoSize = true;
            remaindersButton.Text = "Остатки материалов";
            remaindersButton.Tag = (Remainders) blocks[0];
            remaindersButton.Click += RemaindersButtonOnClick;
            mainButtonsHashSet.Add(remaindersButton);
            Controls.Add(remaindersButton);

            addItem = new Button();
            addItem.Location = new Point(10, signIn.Location.Y + signIn.Height + 25);
            addItem.Click += AddButtonOnClick;
            addItem.Text = "Добавить";
            addItem.AutoSize = true;
            addItem.Enabled = false;
            activeTempControls.Add(addItem);
            Controls.Add(addItem);

            searchNameLine = new TextBox();
            searchNameLine.Location = new Point(10, addItem.Location.Y + addItem.Height + 10);
            searchNameLine.TextChanged += NameLine_TextChanged;
            searchNameLine.Width = 450;
            searchNameLine.Tag = TypeOfItem.Material;
            searchNameLine.Enabled = false;
            activeTempControls.Add(searchNameLine);
            Controls.Add(searchNameLine);

            allItems = new Button();
            allItems.Location = new Point(10, searchNameLine.Location.Y + searchNameLine.Height + 10);
            allItems.Click += AllItems_Click;
            allItems.AutoSize = true;
            allItems.Text = "Показать";
            allItems.Tag = TypeOfItem.Material;
            allItems.Enabled = false;
            activeTempControls.Add(allItems);
            Controls.Add(allItems);

            previousListButton = new Button();
            previousListButton.Text = "Назад";
            previousListButton.Location = new Point(20 + allItems.Location.X + allItems.Width, allItems.Location.Y);
            previousListButton.Enabled = false;
            previousListButton.Click += PreviousNext_Click;
            Controls.Add(previousListButton);

            nextListButton = new Button();
            nextListButton.Text = "Вперед";
            nextListButton.Location = new Point(20 + previousListButton.Location.X + previousListButton.Width, previousListButton.Location.Y);
            nextListButton.Enabled = false;
            nextListButton.Click += PreviousNext_Click;
            Controls.Add(nextListButton);

            //var scrollBar = new VScrollBar();
            //scrollBar.Dock = DockStyle.Right;
            //Controls.Add(scrollBar);


        }

        private void ScrollBarOnScroll(object sender, ScrollEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SignInOnClick(object? sender, EventArgs e)
        {
            var loginForm = new LoginForm(this, _dataBase);
            loginForm.ShowDialog();
            if(!LoginCompleted) return;
            Controls.Remove(signIn);

            foreach (var button in mainButtonsHashSet)
                button.Enabled = true;
        }

        private void RemaindersButtonOnClick(object? sender, EventArgs e)
        {
            ShowEmptyList("Введите наименование");
            var control = (Control) sender;
            activeBlock = (IProgramBlock) control.Tag;
            addItem.Text = "Добавить материал";
            addItem.Tag = activeBlock.ItemTypeName;
            allItems.Text = "Показать все материалы";
            foreach (var ctrl in activeTempControls)
                ctrl.Enabled = true;

            previousListButton.Tag = 0;
            previousListButton.Location = new Point(20 + allItems.Location.X + allItems.Width, allItems.Location.Y);

            nextListButton.Tag = 0;
            nextListButton.Location = new Point(20 + previousListButton.Location.X + previousListButton.Width, previousListButton.Location.Y);
        }

        public bool CheckNextOffset(List<IBlockItem> itemsList)
        {
            var indexOfLastItem = activeBlock.LengthOfItemsList;
            var lengthOfItemList = indexOfLastItem + 1;

            if (itemsList.Count != lengthOfItemList) return false;
            itemsList.RemoveAt(indexOfLastItem);
            return true;
        }

        public bool CheckPreviousOffset(int offset)
        {
            return offset > 0;
        }

        private async void PreviousNext_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var offset = (int)control.Tag;
            await ShowItems(offset);
        }

        private async void AllItems_Click(object sender, EventArgs e)
        {
            searchNameLine.Text = "";
            await ShowItems(0);
        }

        private async void NameLine_TextChanged(object sender, EventArgs e)
        {
            if (searchNameLine.Text == "")
            {
                DefaultPreviousNextButtons();
                ShowEmptyList("Введите наименование");
                return;
            }
            await ShowItems(0);
        }

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = activeBlock.LengthOfItemsList;
            var itemsList = await activeBlock.GetItemsList(offset, searchNameLine.Text.ToLower());
            _offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    _offset = offset;
                    itemsList = await activeBlock.GetItemsList(offset, searchNameLine.Text); 
                }
                else
                {
                    ShowEmptyList("Список пуст");
                    return;
                }
            }

            if (CheckNextOffset(itemsList))
            {
                nextListButton.Enabled = true;
                nextListButton.Tag = offset + maxShowItemsCount;
            }
            else
                DefaultNextButton();

            if (CheckPreviousOffset(offset))
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
            var lastControl = (Control) allItems;
            
            if (activeItemTempControls.Count > 0) ClearActiveItemTempControls();
            foreach (var item in itemsList)
                lastControl = DisplayItems(item, lastControl);
        }

        private void ShowEmptyList(string message)
        {
            DefaultPreviousNextButtons();
            if (activeItemTempControls.Count > 0) ClearActiveItemTempControls();
            var lastControl = allItems;
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
            itemLabel.Width = 100;
            itemLabel.Click += ItemLabel_Click;
            Controls.Add(itemLabel);
            activeItemTempControls.Add(itemLabel);

            itemLabel.Text = item.Name;
            itemLabel.Tag = item;
            
            return itemLabel;
        }

        private void ItemLabel_Click(object sender, EventArgs e)
        {
            var label = (Label) sender;
            var form = _formsCollection.GetItemForm(label.Tag.GetType(), label.Tag, _inputOperations);

            form.ShowDialog();

            ShowItems(_offset);
        }

        

        private void AddButtonOnClick(object? sender, EventArgs e)
        {
            var button = (Button)sender;
            var form = _formsCollection.GetAddItemForm((string)button.Tag, activeBlock, _inputOperations);
            form.ShowDialog();

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
