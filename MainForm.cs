using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting
{
    public partial class MainForm : Form
    {
        public bool LoginCompleted { get; set; }
        private Button RemaindersButton { get; }
        private Button CalculationsButton { get; }
        private Button PreOrdersButton { get; }
        private Button OrdersButton { get; }

        private List<Button> MainButtonsHashSet { get; }
        private List<Control> ActiveTempControls { get; }
        private List<Control> ActiveItemTempControls { get; }
        private Button NextListButton { get; }
        private Button PreviousListButton { get; }
        private Button AllItems { get; }
        private Button AddItem { get; }
        private TextBox SearchNameLine { get; }
        private Button SignIn { get; } 
        private int Offset { get; set; }
        private EventHandler AddAction { get; set; }
        private EventHandler ItemAction { get; set; }
        private IFormFactory FormFactory { get; }
        private BlockItemsCollectionCreator Block { get; set; }
         
        public MainForm(BlockItemsCollectionCreator[] blocks, IFormFactory formFactory)
        {
            this.FormFactory = formFactory;

            MainButtonsHashSet = new List<Button>();
            ActiveTempControls = new List<Control>();
            ActiveItemTempControls = new List<Control>();

            AutoScroll = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Size = new Size(600, 600);

            SignIn = new Button();
            SignIn.AutoSize = true;
            SignIn.Click += SignInOnClick;
            SignIn.Text = "Войти в базу данных";
            Controls.Add(SignIn);

            RemaindersButton = new Button();
            RemaindersButton.Location = new Point(SignIn.Location.X + SignIn.Width + 10, SignIn.Location.Y);
            RemaindersButton.Enabled = false;
            RemaindersButton.AutoSize = true;
            RemaindersButton.Text = "Остатки материалов";
            RemaindersButton.Tag = blocks[0];
            RemaindersButton.Click += MaterialsButtonOnClick;
            MainButtonsHashSet.Add(RemaindersButton);
            Controls.Add(RemaindersButton);

            CalculationsButton = new Button();
            CalculationsButton.Location = new Point(RemaindersButton.Location.X + RemaindersButton.Width + 10, RemaindersButton.Location.Y);
            CalculationsButton.Enabled = false;
            CalculationsButton.AutoSize = true;
            CalculationsButton.Text = "Калькуляции";
            CalculationsButton.Tag = blocks[1];
            CalculationsButton.Click += CalculationsButtonOnClick;
            MainButtonsHashSet.Add(CalculationsButton);
            Controls.Add(CalculationsButton);

            PreOrdersButton = new Button();
            PreOrdersButton.Location = new Point(CalculationsButton.Location.X + CalculationsButton.Width + 10, CalculationsButton.Location.Y);
            PreOrdersButton.Enabled = false;
            PreOrdersButton.AutoSize = true;
            PreOrdersButton.Text = "Предзаказы";
            PreOrdersButton.Tag = blocks[2];
            PreOrdersButton.Click += PreOrdersButtonOnClick;
            MainButtonsHashSet.Add(PreOrdersButton);
            Controls.Add(PreOrdersButton);

            OrdersButton = new Button();
            OrdersButton.Location = new Point(PreOrdersButton.Location.X + PreOrdersButton.Width + 10, PreOrdersButton.Location.Y);
            OrdersButton.Enabled = false;
            OrdersButton.AutoSize = true;
            OrdersButton.Text = "Заказы";
            OrdersButton.Tag = blocks[3];
            OrdersButton.Click += OrdersButtonOnClick;
            MainButtonsHashSet.Add(OrdersButton);
            Controls.Add(OrdersButton);

            AddItem = new Button();
            AddItem.Location = new Point(10, SignIn.Location.Y + SignIn.Height + 25);
            AddItem.Text = "Добавить";
            AddItem.AutoSize = true;
            AddItem.Enabled = false;
            ActiveTempControls.Add(AddItem);
            Controls.Add(AddItem);

            SearchNameLine = new TextBox();
            SearchNameLine.Location = new Point(10, AddItem.Location.Y + AddItem.Height + 10);
            SearchNameLine.TextChanged += NameLine_TextChanged;
            SearchNameLine.Width = 450;
            SearchNameLine.Tag = TypeOfItem.Material;
            SearchNameLine.Enabled = false;
            ActiveTempControls.Add(SearchNameLine);
            Controls.Add(SearchNameLine);

            AllItems = new Button();
            AllItems.Location = new Point(10, SearchNameLine.Location.Y + SearchNameLine.Height + 10);
            AllItems.Click += AllItems_Click;
            AllItems.AutoSize = true;
            AllItems.Text = "Показать";
            AllItems.Tag = TypeOfItem.Material;
            AllItems.Enabled = false;
            ActiveTempControls.Add(AllItems);
            Controls.Add(AllItems);

            PreviousListButton = new Button();
            PreviousListButton.Text = "Назад";
            PreviousListButton.Location = new Point(20 + AllItems.Location.X + AllItems.Width, AllItems.Location.Y);
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

        private void SignInOnClick(object sender, EventArgs e)
        {
            var loginForm = FormFactory.CreateLoginForm(this);
            loginForm.ShowDialog();
            if(!LoginCompleted) return;
            Controls.Remove(SignIn);

            foreach (var button in MainButtonsHashSet)
                button.Enabled = true;
        }

        private void MaterialsButtonOnClick(object sender, EventArgs e)
        {
            ShowEmptyList("Введите наименование");
            var control = (Control) sender;
            Block = (BlockItemsCollectionCreator) control.Tag;

            if (AddAction != null)
            {
                AddItem.Click -= AddAction;
            }

            AddAction = AddMaterialButtonOnClick;
            AddItem.Click += AddAction;
            
            ItemAction = ItemMaterialLabel_Click;
            
            AddItem.Text = "Добавить материал";
            AllItems.Text = "Показать все материалы";
            foreach (var ctrl in ActiveTempControls)
                ctrl.Enabled = true;

            PreviousListButton.Tag = 0;
            PreviousListButton.Location = new Point(20 + AllItems.Location.X + AllItems.Width, AllItems.Location.Y);

            NextListButton.Tag = 0;
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
        }

        private void CalculationsButtonOnClick(object sender, EventArgs e)
        {
            ShowEmptyList("Введите наименование");
            var control = (Control)sender;
            Block = (BlockItemsCollectionCreator)control.Tag;

            if (AddAction != null)
            {
                AddItem.Click -= AddAction;
            }

            AddAction = AddCalculationButtonOnClick;
            AddItem.Click += AddAction;

            ItemAction = ItemCalcLabel_Click;
            
            AddItem.Text = "Добавить калькуляцию";
            AllItems.Text = "Показать все калькуляции";
            foreach (var ctrl in ActiveTempControls)
                ctrl.Enabled = true;

            PreviousListButton.Tag = 0;
            PreviousListButton.Location = new Point(20 + AllItems.Location.X + AllItems.Width, AllItems.Location.Y);

            NextListButton.Tag = 0;
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
        }

        private void PreOrdersButtonOnClick(object sender, EventArgs e)
        {
            ShowEmptyList("Введите наименование");
            var control = (Control)sender;
            Block = (BlockItemsCollectionCreator)control.Tag;

            if (AddAction != null)
            {
                AddItem.Click -= AddAction;
            }

            AddAction = AddPreOrderButtonOnClick;
            AddItem.Click += AddAction;

            ItemAction = ItemPreOrderLabel_Click;
            
            AddItem.Text = "Добавить предзаказ";
            AllItems.Text = "Показать все предзаказы";
            foreach (var ctrl in ActiveTempControls)
                ctrl.Enabled = true;

            PreviousListButton.Tag = 0;
            PreviousListButton.Location = new Point(20 + AllItems.Location.X + AllItems.Width, AllItems.Location.Y);

            NextListButton.Tag = 0;
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
        }

        private void OrdersButtonOnClick(object sender, EventArgs e)
        {
            ShowEmptyList("Введите наименование");
            var control = (Control)sender;
            Block = (BlockItemsCollectionCreator)control.Tag;

            if (AddAction != null)
            {
                AddItem.Click -= AddAction;
            }

            //AddAction = AddOrderButtonOnClick;
            //AddItem.Click += AddAction;

            ItemAction = ItemOrderLabel_Click;

            AddItem.Text = "Добавить";
            AllItems.Text = "Показать все заказы";
            foreach (var ctrl in ActiveTempControls)
                ctrl.Enabled = true;
            AddItem.Enabled = false;

            PreviousListButton.Tag = 0;
            PreviousListButton.Location = new Point(20 + AllItems.Location.X + AllItems.Width, AllItems.Location.Y);

            NextListButton.Tag = 0;
            NextListButton.Location = new Point(20 + PreviousListButton.Location.X + PreviousListButton.Width, PreviousListButton.Location.Y);
        }

        private async void PreviousNext_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            var offset = (int)control.Tag;
            await ShowItems(offset);
        }

        private async void AllItems_Click(object sender, EventArgs e)
        {
            SearchNameLine.Text = "";
            await ShowItems(0);
        }

        private async void NameLine_TextChanged(object sender, EventArgs e)
        {
            if (SearchNameLine.Text == "")
            {
                DefaultPreviousNextButtons();
                ShowEmptyList("Введите наименование");
                return;
            }
            await ShowItems(0);
        }

        private async Task ShowItems(int offset)
        {
            var maxShowItemsCount = Block.LengthOfItemsList;
            var resultOfGettingItemsList = await Block.GetItemsList(offset, SearchNameLine.Text.ToLower());
            var itemsList = resultOfGettingItemsList.Item1;
            var isThereMoreOfItems = resultOfGettingItemsList.Item2;

            Offset = offset;

            if (itemsList.Count == 0)
            {
                if (offset > 0)
                {
                    offset -= maxShowItemsCount;
                    Offset = offset;
                    resultOfGettingItemsList = await Block.GetItemsList(offset, SearchNameLine.Text.ToLower());
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
            var lastControl = (Control) AllItems;
            
            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            foreach (var item in itemsList)
                lastControl = DisplayItems(item, lastControl);
        }

        private void ShowEmptyList(string message)
        {
            DefaultPreviousNextButtons();
            if (ActiveItemTempControls.Count > 0) ClearActiveItemTempControls();
            var lastControl = AllItems;
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
            itemLabel.Width = 200;
            itemLabel.Click += ItemAction;
            Controls.Add(itemLabel);
            ActiveItemTempControls.Add(itemLabel);

            itemLabel.Text = item.Name;
            itemLabel.Tag = item;
            
            return itemLabel;
        }

        private async void ItemMaterialLabel_Click(object sender, EventArgs e)
        {
            var label = (Label) sender;
            var material = (IMaterial) label.Tag;
            
            var form = FormFactory.CreateMaterialForm(material, false);

            form.ShowDialog();

            await ShowItems(Offset);
        }

        private async void AddMaterialButtonOnClick(object sender, EventArgs e)
        {
            var form = FormFactory.CreateAddMaterialForm();
            form.ShowDialog();

            await ShowItems(Offset);
        }


        private async void AddCalculationButtonOnClick(object sender, EventArgs e)
        {
            var form = FormFactory.CreateAddCalculationForm();
            form.ShowDialog();

            await ShowItems(Offset);
        }

        private async void ItemCalcLabel_Click(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var calculation = (ICalculation)label.Tag;

            var form = FormFactory.CreateCalculationForm(calculation);

            form.ShowDialog();

            await ShowItems(Offset);
        }

        private async void AddPreOrderButtonOnClick(object sender, EventArgs e)
        {
            var form = FormFactory.CreateAddPreOrderForm();
            form.ShowDialog();

            await ShowItems(Offset);
        }

        private async void ItemPreOrderLabel_Click(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var preOrder = (IPreOrder)label.Tag;

            var form = FormFactory.CreatePreOrderForm(preOrder);

            form.ShowDialog();

            await ShowItems(Offset);
        }

        private async void ItemOrderLabel_Click(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var order = (IOrder)label.Tag;

            var form = FormFactory.CreateOrderForm(order);

            form.ShowDialog();

            await ShowItems(Offset);
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
