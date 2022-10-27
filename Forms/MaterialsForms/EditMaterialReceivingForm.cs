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

namespace ManagementAccounting.Forms.RemaindersForms
{
    public partial class EditMaterialReceivingForm : Form
    {
        private IMaterialReceiving _materialReceiving { get; set; }
        private IMaterialReceiving newMaterialReceiving { get; set; }
        private IOperationsWithUserInput inputOperations { get; }
        private ISystemMaterialReceivingOperations materialReceivingOperations { get; }
        private IItemsFactory itemsFactory { get; }
        private TextBox dateValue { get; }
        private TextBox quantityValue { get; }
        private TextBox costValue { get; }
        private TextBox noteValue { get; }
        private Button editButton { get; }
        private Button cancelButton { get; }
        private MaterialReceivingForm materialReceivingForm { get; }

        public EditMaterialReceivingForm(IMaterialReceiving materialReceiving, MaterialReceivingForm materialReceivingForm, IItemsFactory itemsFactory, IOperationsWithUserInput inputOperations, ISystemMaterialReceivingOperations materialReceivingOperations)
        {
            this.materialReceivingForm = materialReceivingForm;
            this.inputOperations = inputOperations;
            this.itemsFactory = itemsFactory;
            this.materialReceivingOperations = materialReceivingOperations;

            _materialReceiving = materialReceiving;

            Size = new Size(400, 600);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение поступления";
            Controls.Add(topLabel);

            var dateLabel = new Label();
            dateLabel.Text = "Дата";
            dateLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 15);
            Controls.Add(dateLabel);

            dateValue = new TextBox();
            dateValue.Location = new Point(dateLabel.Location.X + dateLabel.Width + 15, dateLabel.Location.Y);
            dateValue.Text = _materialReceiving.Date.ToString("dd/MM/yyyy");
            Controls.Add(dateValue);


            var quantityLabel = new Label();
            quantityLabel.Text = "Количество";
            quantityLabel.Location = new Point(10, dateValue.Location.Y + dateValue.Height + 15);
            Controls.Add(quantityLabel);

            quantityValue = new TextBox();
            quantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            quantityValue.Text = _materialReceiving.Quantity.ToString();
            Controls.Add(quantityValue);

            var costLabel = new Label();
            costLabel.Text = "Стоимость";
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            costValue = new TextBox();
            costValue.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            costValue.Text = _materialReceiving.Cost.ToString();
            Controls.Add(costValue);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка";
            noteLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            Controls.Add(noteLabel);

            noteValue = new TextBox();
            noteValue.Location = new Point(noteLabel.Location.X + noteLabel.Width + 15, noteLabel.Location.Y);
            noteValue.Text = _materialReceiving.Note;
            Controls.Add(noteValue);

            editButton = new Button();
            editButton.Text = "Изменить";
            editButton.Location = new Point(30, noteLabel.Location.Y + noteLabel.Height + 15);
            editButton.Click += EditButtonOnClick;
            Controls.Add(editButton);

            cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.Location = new Point(editButton.Location.X + editButton.Width + 15, editButton.Location.Y);
            cancelButton.Click += CancelButtonOnClick;
            Controls.Add(cancelButton);
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var date = inputOperations.GetCorrectData(dateValue.Text);
                var quantity = inputOperations.GetPositiveDecimal(quantityValue.Text);
                var cost = inputOperations.GetPositiveDecimalorZero(costValue.Text);
                var remainder = quantity;
                var note = inputOperations.GetNotEmptyName(noteValue.Text, 50);
                newMaterialReceiving =
                    itemsFactory.CreateMaterialReceiving(_materialReceiving.Material, date, quantity, cost, remainder, note, _materialReceiving.Index);
                await materialReceivingOperations.Edit(_materialReceiving, newMaterialReceiving);
                materialReceivingForm.UpdateMeterialReceiving(newMaterialReceiving);
            }
            catch (OrderItemOperationException exception)
            {
                await materialReceivingOperations.Default(newMaterialReceiving, _materialReceiving);

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
