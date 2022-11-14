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
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Forms.OrdersForms
{
    public partial class EditOrderForm : Form
    {
        private IOrder Order { get; }
        private Button EditButton { get; }
        private Button CloseButton { get; }
        //private TextBox QuantityValue { get; }
        private TextBox DateValue { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private ISystemOrderOperations OrderOperations { get; }
        private IItemsFactory ItemsFactory { get; }
        private OrderForm OrderForm { get; }

        public EditOrderForm(IOrder order, OrderForm orderForm, IOperationsWithUserInput inputOperations, ISystemOrderOperations orderOperations, IItemsFactory itemsFactory)
        {
            InputOperations = inputOperations;
            Order = order;
            ItemsFactory = itemsFactory;
            OrderOperations = orderOperations;
            Size = new Size(400, 600);



            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение заказа";
            Controls.Add(topLabel);

            //var quantityLabel = new Label();
            //quantityLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            //quantityLabel.Width = 130;
            //quantityLabel.Text = "Кол-во изделий, шт:";
            //Controls.Add(quantityLabel);

            //QuantityValue = new TextBox();
            //QuantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            //QuantityValue.Width = 200;
            //Controls.Add(QuantityValue);

            var dateLabel = new Label();
            dateLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 25);
            dateLabel.Width = 130;
            dateLabel.Text = "Дата создания:";
            Controls.Add(dateLabel);

            DateValue = new TextBox();
            DateValue.Text = Order.CreationDate.ToString("dd/MM/yyyy");
            DateValue.Location = new Point(dateLabel.Location.X + dateLabel.Width + 10, dateLabel.Location.Y);
            DateValue.Width = 200;
            Controls.Add(DateValue);


            EditButton = new Button();
            EditButton.Location = new Point(50, dateLabel.Location.Y + dateLabel.Height + 25);
            EditButton.Text = "Изменить";
            EditButton.AutoSize = true;
            EditButton.Click += EditButtonOnClick;
            Controls.Add(EditButton);

            CloseButton = new Button();
            CloseButton.Location = new Point(EditButton.Location.X + EditButton.Width + 20, EditButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += CloseButtonOnClick;
            Controls.Add(CloseButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void EditButtonOnClick(object sender, EventArgs e)
        {
            //var isQuantityCorrect = InputOperations.TryGetPositiveInt(QuantityValue.Text, out var quantity);
            var isDateCorrect = InputOperations.TryGetCorrectData(DateValue.Text, out var date);

            if (!isDateCorrect)
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                return;
            }

            var newOrder = ItemsFactory.CreateOrder(Order.ShortName, date, Order.Quantity, Order.Index);

            try
            {
                await OrderOperations.Edit(Order, newOrder);
                OrderForm.UpdateOrder(newOrder);
            }
            catch (OrderItemOperationException exception)
            {
                await OrderOperations.Default(newOrder, Order);
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
        }
    }
}
