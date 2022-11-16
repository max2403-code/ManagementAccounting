using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;
using Npgsql;

namespace ManagementAccounting.Forms.PreOrdersForms
{
    public partial class EditPreOrderForm : Form
    {
        private IPreOrder PreOrder { get; set; }
        private Button EditButton { get; }
        private Button CloseButton { get; }
        private TextBox QuantityValue { get; }
        private TextBox DateValue { get; }
        private List<Button> Buttons { get; }
        private IOperationsWithUserInput InputOperations { get; }

        public EditPreOrderForm(IPreOrder preOrder, IOperationsWithUserInput inputOperations)
        {
            InputOperations = inputOperations;
            PreOrder = preOrder;
            Buttons = new List<Button>();
            Size = new Size(400, 600);



            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение предзаказа";
            Controls.Add(topLabel);

            var quantityLabel = new Label();
            quantityLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            quantityLabel.Width = 130;
            quantityLabel.Text = "Кол-во изделий, шт:";
            Controls.Add(quantityLabel);

            QuantityValue = new TextBox();
            QuantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            QuantityValue.Width = 200;
            Controls.Add(QuantityValue);

            var dateLabel = new Label();
            dateLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 50);
            dateLabel.Width = 130;
            dateLabel.Text = "Дата создания:";
            Controls.Add(dateLabel);

            DateValue = new TextBox();
            DateValue.Text = PreOrder.CreationDate.ToString("dd/MM/yyyy");
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
            DisableButtons();
            var isQuantityCorrect = InputOperations.TryGetPositiveInt(QuantityValue.Text, out var quantity);
            var isDateCorrect = InputOperations.TryGetCorrectData(DateValue.Text, out var date);

            if (!isDateCorrect || !isQuantityCorrect)
            {
                MessageBox.Show("Введены некорректные данные", "Внимание");
                EnableButtons();
                return;
            }
            try
            {
                await ((EditingBlockItemDB)PreOrder).EditItemInDataBase<IPreOrder>(quantity, date);
            }
            catch (NpgsqlException exception)
            {
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
