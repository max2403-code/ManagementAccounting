using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Forms.PreOrdersForms
{
    public partial class EditPreOrderForm : Form
    {
        private IPreOrder preOrder { get; set; }
        private Button editButton { get; }
        private Button closeButton { get; }

        private TextBox quantityValue { get; }
        private TextBox dateValue { get; }

        private IOperationsWithUserInput inputOperations { get; }

        public EditPreOrderForm(IPreOrder preOrder, IFormFactory formFactory, IOperationsWithUserInput inputOperations)
        {
            this.inputOperations = inputOperations;
            this.preOrder = preOrder;

            Size = new Size(400, 600);



            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение предзаказа";
            Controls.Add(topLabel);

            var quantityLabel = new Label();
            quantityLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            quantityLabel.Width = 100;
            quantityLabel.Text = "Количество позиций:";
            Controls.Add(quantityLabel);

            quantityValue = new TextBox();
            quantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 10, quantityLabel.Location.Y);
            quantityValue.Width = 200;
            Controls.Add(quantityValue);

            var dateLabel = new Label();
            dateLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 50);
            dateLabel.Width = 100;
            dateLabel.Text = "Дата создания:";
            Controls.Add(quantityLabel);

            dateValue = new TextBox();
            dateValue.Location = new Point(dateLabel.Location.X + dateLabel.Width + 10, dateLabel.Location.Y);
            dateValue.Width = 200;
            Controls.Add(dateValue);


            editButton = new Button();
            editButton.Location = new Point(50, dateLabel.Location.Y + dateLabel.Height + 25);
            editButton.Text = "Изменить";
            editButton.AutoSize = true;
            editButton.Click += EditButtonOnClick;
            Controls.Add(editButton);

            closeButton = new Button();
            closeButton.Location = new Point(editButton.Location.X + editButton.Width + 20, editButton.Location.Y);
            closeButton.Text = "Отмена";
            closeButton.AutoSize = true;
            closeButton.Click += CloseButtonOnClick;
            Controls.Add(closeButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var  quantity = inputOperations.GetPositiveInt(quantityValue.Text);
                var date = inputOperations.GetCorrectData(dateValue.Text);

                await ((EditingBlockItemDB)preOrder).EditItemInDataBase<IPreOrder>(quantity, date);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            Close();
        }
    }
}
