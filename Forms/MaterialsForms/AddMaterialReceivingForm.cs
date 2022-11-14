using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.Common;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting
{
    public partial class AddMaterialReceivingForm : Form
    {
        private List<Button> Buttons { get; }
        private IMaterial Material { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private TextBox DateTextBox { get; }
        private TextBox QuantityTextBox { get; }
        private TextBox CostTextBox { get; }
        private TextBox RemainderTextBox { get; }
        private TextBox NoteTextBox { get; }
        private Button AddReceivingButton { get; }
        private Button CloseButton { get; }
        private IItemsFactory ItemsFactory { get; }
        private ISystemMaterialReceivingOperations MaterialReceivingOperations { get; }
        private IMaterialReceiving MaterialReceiving { get; set; }

        public AddMaterialReceivingForm(IMaterial material, IItemsFactory itemsFactory, IOperationsWithUserInput inputOperations, ISystemMaterialReceivingOperations materialReceivingOperations)
        {
            MaterialReceivingOperations = materialReceivingOperations;
            ItemsFactory = itemsFactory;
            Material = material;
            InputOperations = inputOperations;
            Buttons = new List<Button>();
            Size = new Size(400, 600);


            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Новое поступление";
            Controls.Add(topLabel);

            var dateLabel = new Label();
            dateLabel.Text = "Дата пост-я:";
            dateLabel.Width = 100;
            dateLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 25);
            Controls.Add(dateLabel);

            DateTextBox = new TextBox();
            DateTextBox.Location = new Point(dateLabel.Location.X + dateLabel.Width + 15, dateLabel.Location.Y);
            Controls.Add(DateTextBox);

            var quantityLabel = new Label();
            quantityLabel.Text = $"Количество, {InputOperations.TranslateType(Material.Unit.ToString())}:";
            quantityLabel.Width = 100;
            quantityLabel.Location = new Point(10, dateLabel.Location.Y + dateLabel.Height + 15);
            Controls.Add(quantityLabel);

            QuantityTextBox= new TextBox();
            QuantityTextBox.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            Controls.Add(QuantityTextBox);

            var costLabel = new Label();
            costLabel.Text = "Стоимость, руб.:";
            costLabel.Width = 100;
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            CostTextBox = new TextBox();
            CostTextBox.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            Controls.Add(CostTextBox);

            //var remainderLabel = new Label();
            //remainderLabel.Text = "Остаток на складе";
            //remainderLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            //Controls.Add(remainderLabel);

            //remainderTextBox = new TextBox();
            //remainderTextBox.Location = new Point(remainderLabel.Location.X + remainderLabel.Width + 15, remainderLabel.Location.Y);
            //Controls.Add(remainderTextBox);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка:";
            noteLabel.Width = 100;
            noteLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            Controls.Add(noteLabel);

            NoteTextBox = new TextBox();
            NoteTextBox.Location = new Point(noteLabel.Location.X + noteLabel.Width + 15, noteLabel.Location.Y);
            Controls.Add(NoteTextBox);

            AddReceivingButton = new Button();
            AddReceivingButton.Text = "Добавить";
            AddReceivingButton.Location = new Point(30, noteLabel.Location.Y + noteLabel.Height + 15);
            AddReceivingButton.Click += AddReceivingButtonOnClick; 
            Controls.Add(AddReceivingButton);

            CloseButton = new Button();
            CloseButton.Text = "Отмена";
            CloseButton.Location = new Point(AddReceivingButton.Location.X + AddReceivingButton.Width + 15, AddReceivingButton.Location.Y);
            CloseButton.Click += CancelButtonOnClick;
            Controls.Add(CloseButton);
        }


        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddReceivingButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var isDateCorrect = InputOperations.TryGetCorrectData(DateTextBox.Text, out var date);
            var isQuantityCorrect = InputOperations.TryGetPositiveDecimal(QuantityTextBox.Text, out var quantity);
            var isCostCorrect = InputOperations.TryGetPositiveDecimalOrZero(CostTextBox.Text, out var cost);
            var remainder = quantity;
            var isNoteCorrect = InputOperations.TryGetNotEmptyName(NoteTextBox.Text, 50, out var note);

            if (!isDateCorrect || !isQuantityCorrect || !isCostCorrect || !isNoteCorrect)
            {
                MessageBox.Show("Поля заполнены неверно", "Внимание");
                EnableButtons();
                return;
            }

            try
            {
                MaterialReceiving =
                    ItemsFactory.CreateMaterialReceiving(Material, date, quantity, cost, remainder, note);
                await MaterialReceivingOperations.Insert(MaterialReceiving);
            }
            catch (OrderItemOperationException exception)
            {
                await MaterialReceivingOperations.Remove(MaterialReceiving);
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
                return;
            }
            
            Close();
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
