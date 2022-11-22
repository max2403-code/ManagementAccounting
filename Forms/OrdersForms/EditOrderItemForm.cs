using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.Common;
using ManagementAccounting.Classes.SystemCreators;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Items;
using Npgsql;

namespace ManagementAccounting.Forms.OrdersForms
{
    public partial class EditOrderItemForm : Form
    {
        private Button EditButton { get; }
        private Button CloseButton { get; }
        private TextBox ConsumptionValue { get; }
        private IOrderItem OrderItem { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IItemsFactory ItemsFactory { get; }
        private ISystemOrderItemOperations OrderItemOperations { get; }



        public EditOrderItemForm(IOrderItem orderItem, ISystemOrderItemOperations orderItemOperations, IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory)
        {
            InputOperations = inputOperations;
            OrderItem = orderItem;
            ItemsFactory = itemsFactory;
            OrderItemOperations = orderItemOperations;

            Size = new Size(400, 600);


            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение статьи заказа";
            Controls.Add(topLabel);

            var consumptionLabel = new Label();
            consumptionLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            consumptionLabel.Width = 80;
            consumptionLabel.Text = $"Расход мат-а, {InputOperations.TranslateType(OrderItem.Material.Unit.ToString())}:";
            Controls.Add(consumptionLabel);

            ConsumptionValue = new TextBox();
            ConsumptionValue.Location = new Point(consumptionLabel.Location.X + consumptionLabel.Width + 10, consumptionLabel.Location.Y);
            ConsumptionValue.Width = 200;
            ConsumptionValue.Text = OrderItem.TotalConsumption.ToString();
            Controls.Add(ConsumptionValue);


            EditButton = new Button();
            EditButton.Location = new Point(50, consumptionLabel.Location.Y + consumptionLabel.Height + 25);
            EditButton.Text = "Изменить";
            EditButton.AutoSize = true;
            EditButton.Click += EditButtonOnClick;
            Controls.Add(EditButton);

            CloseButton = new Button();
            CloseButton.Location = new Point(EditButton.Location.X + EditButton.Width + 20, EditButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += (sender, args) => Close();
            Controls.Add(CloseButton);
        }

        private async void EditButtonOnClick(object sender, EventArgs e)
        {
            var isConsumptionCorrect = InputOperations.TryGetPositiveDecimal(ConsumptionValue.Text, out var consumption);

            if (!isConsumptionCorrect)
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                return;
            }

            var oldOrderItem = ItemsFactory.CreateOrderItem(OrderItem.Order, OrderItem.Material,
                OrderItem.TotalConsumption, OrderItem.TotalConsumption, OrderItem.Index);
            var newOrderItem = ItemsFactory.CreateOrderItem(OrderItem.Order, OrderItem.Material,
                consumption, consumption, OrderItem.Index);

            try
            {
                await OrderItemOperations.Edit(oldOrderItem, newOrderItem);
            }
            catch (OrderItemOperationException exception)
            {
                try
                {
                    await OrderItemOperations.Default(newOrderItem, oldOrderItem);
                }
                catch (OrderItemOperationException)
                {
                    MessageBox.Show("Если ошибка произощла здесь, то это печально", "Внимание");
                    return;
                }
                catch (NpgsqlException npgsqlException)
                {
                    MessageBox.Show(npgsqlException.Message, "Внимание");
                    return;
                }

                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
        }
    }
}
