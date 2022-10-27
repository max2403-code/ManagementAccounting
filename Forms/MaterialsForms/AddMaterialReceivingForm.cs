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
        private IMaterial material { get; }
        private IOperationsWithUserInput inputOperations { get; }
        private TextBox dateTextBox { get; }
        private TextBox quantityTextBox { get; }
        private TextBox costTextBox { get; }
        private TextBox remainderTextBox { get; }
        private TextBox noteTextBox { get; }
        private Button addReceivingButton { get; }
        private Button cancelButton { get; }
        private IItemsFactory itemsFactory { get; }
        private ISystemMaterialReceivingOperations materialReceivingOperations { get; }
        private IMaterialReceiving materialReceiving { get; set; }

        public AddMaterialReceivingForm(IMaterial material, IItemsFactory itemsFactory, IOperationsWithUserInput inputOperations, ISystemMaterialReceivingOperations materialReceivingOperations)
        {
            this.materialReceivingOperations = materialReceivingOperations;
            this.itemsFactory = itemsFactory;
            this.material = material;
            this.inputOperations = inputOperations;

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
            quantityTextBox.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            Controls.Add(quantityTextBox);

            var costLabel = new Label();
            costLabel.Text = "Стоимость";
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            costTextBox = new TextBox();
            costTextBox.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            Controls.Add(costTextBox);

            //var remainderLabel = new Label();
            //remainderLabel.Text = "Остаток на складе";
            //remainderLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            //Controls.Add(remainderLabel);

            //remainderTextBox = new TextBox();
            //remainderTextBox.Location = new Point(remainderLabel.Location.X + remainderLabel.Width + 15, remainderLabel.Location.Y);
            //Controls.Add(remainderTextBox);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка";
            noteLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
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


        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddReceivingButtonOnClick(object sender, EventArgs e)
        {
            try
            {
               
                var date = inputOperations.GetCorrectData(dateTextBox.Text);
                var quantity = inputOperations.GetPositiveDecimal(quantityTextBox.Text);
                var cost = inputOperations.GetPositiveDecimalorZero(costTextBox.Text);
                var remainder = quantity;
                var note = inputOperations.GetNotEmptyName(noteTextBox.Text, 50) ;
                materialReceiving =
                    itemsFactory.CreateMaterialReceiving(material, date, quantity, cost, remainder, note);
                await materialReceivingOperations.Insert(materialReceiving);
            }
            catch (OrderItemOperationException exception)
            {
                await materialReceivingOperations.Remove(materialReceiving);

                MessageBox.Show(exception.Message, "Внимание");
                return;
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
