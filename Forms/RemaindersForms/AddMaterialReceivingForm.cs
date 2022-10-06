using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ManagementAccounting
{
    public partial class AddMaterialReceivingForm : Form
    {
        private IMaterial _material { get; }
        private IOperationsWithUserInput _inputOperations { get; }
        private TextBox dateTextBox { get; }
        private TextBox quantityTextBox { get; }
        private TextBox costTextBox { get; }
        private TextBox remainderTextBox { get; }
        private TextBox noteTextBox { get; }
        private Button addReceivingButton { get; }
        private Button cancelButton { get; }


        public AddMaterialReceivingForm(IMaterial material, IOperationsWithUserInput inputOperations)
        {
            _material = material;
            _inputOperations = inputOperations;

            Size = new Size(400, 600);


            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Новое поступление";
            Controls.Add(topLabel);

            var dateLabel = new Label();
            dateLabel.Text = "Дата поступления";
            dateLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 25);
            Controls.Add(dateLabel);

            dateTextBox = new TextBox();
            dateTextBox.Location = new Point(dateLabel.Location.X + dateLabel.Width + 15, dateLabel.Location.Y);
            Controls.Add(dateTextBox);

            var quantityLabel = new Label();
            quantityLabel.Text = "Количество";
            quantityLabel.Location = new Point(10, dateLabel.Location.Y + dateLabel.Height + 15);
            Controls.Add(quantityLabel);

            quantityTextBox= new TextBox();
            quantityTextBox.TextChanged += QuantityTextBoxOnTextChanged;
            quantityTextBox.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            Controls.Add(quantityTextBox);

            var costLabel = new Label();
            costLabel.Text = "Стоимость";
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            costTextBox = new TextBox();
            costTextBox.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            Controls.Add(costTextBox);

            var remainderLabel = new Label();
            remainderLabel.Text = "Остаток на складе";
            remainderLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            Controls.Add(remainderLabel);

            remainderTextBox = new TextBox();
            remainderTextBox.Location = new Point(remainderLabel.Location.X + remainderLabel.Width + 15, remainderLabel.Location.Y);
            Controls.Add(remainderTextBox);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка";
            noteLabel.Location = new Point(10, remainderLabel.Location.Y + remainderLabel.Height + 15);
            Controls.Add(noteLabel);

            noteTextBox = new TextBox();
            noteTextBox.Location = new Point(noteLabel.Location.X + noteLabel.Width + 15, noteLabel.Location.Y);
            Controls.Add(noteTextBox);

            addReceivingButton = new Button();
            addReceivingButton.Text = "Добавить";
            addReceivingButton.Location = new Point(30, noteLabel.Location.Y + noteLabel.Height + 15);
            addReceivingButton.Click += AddReceivingButtonOnClick; 
            Controls.Add(addReceivingButton);

            cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Location = new Point(addReceivingButton.Location.X + addReceivingButton.Width + 15, addReceivingButton.Location.Y);
            cancelButton.Click += CancelButtonOnClick;
            Controls.Add(cancelButton);
        }

        private void QuantityTextBoxOnTextChanged(object sender, EventArgs e)
        {
            remainderTextBox.Text = quantityTextBox.Text;
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddReceivingButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dateArray = dateTextBox.Text.Split(".", StringSplitOptions.RemoveEmptyEntries);
                var date = new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[1]), int.Parse(dateArray[0]));
                var quantity = decimal.Parse(quantityTextBox.Text);
                var cost = decimal.Parse(costTextBox.Text);
                var remainder = decimal.Parse(remainderTextBox.Text);
                var note = noteTextBox.Text;
                if (remainder > quantity) throw new Exception();
                var receiving = (IMaterialReceiving)_material.GetNewBlockItem(date, quantity, cost, remainder, note, _material.Index);
                await receiving.AddItemToDataBase();

            }
            catch (Exception exception)
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                return;
            }
            Close();
        }
    }
}
