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

namespace ManagementAccounting.Forms.RemaindersForms
{
    public partial class EditMaterialReceivingForm : Form
    {
        private IMaterialReceiving MaterialReceiving { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private ISystemMaterialReceivingOperations MaterialReceivingOperations { get; }
        private IItemsFactory ItemsFactory { get; }
        private List<Button> Buttons { get; }
        private TextBox DateValue { get; }
        private TextBox QuantityValue { get; }
        private TextBox CostValue { get; }
        private TextBox NoteValue { get; }
        private Button EditButton { get; }
        private Button CloseButton { get; }

        public EditMaterialReceivingForm(IMaterialReceiving materialReceiving, IItemsFactory itemsFactory, IOperationsWithUserInput inputOperations, ISystemMaterialReceivingOperations materialReceivingOperations)
        {
            InputOperations = inputOperations;
            ItemsFactory = itemsFactory;
            MaterialReceivingOperations = materialReceivingOperations;
            Buttons = new List<Button>();
            MaterialReceiving = materialReceiving;

            Size = new Size(400, 600);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение поступления";
            Controls.Add(topLabel);

            var dateLabel = new Label();
            dateLabel.Text = "Дата:";
            dateLabel.Width = 130;
            dateLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 15);
            Controls.Add(dateLabel);

            DateValue = new TextBox();
            DateValue.Location = new Point(dateLabel.Location.X + dateLabel.Width + 15, dateLabel.Location.Y);
            DateValue.Text = MaterialReceiving.Date.ToString("dd/MM/yyyy");
            Controls.Add(DateValue);


            var quantityLabel = new Label();
            quantityLabel.Text = $"Количество, {InputOperations.TranslateType(MaterialReceiving.Material.Unit.ToString())}:";
            quantityLabel.Width = 130;
            quantityLabel.Location = new Point(10, DateValue.Location.Y + DateValue.Height + 15);
            Controls.Add(quantityLabel);

            QuantityValue = new TextBox();
            QuantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            QuantityValue.Text = MaterialReceiving.Quantity.ToString();
            Controls.Add(QuantityValue);

            var costLabel = new Label();
            costLabel.Text = "Стоимость, руб.:";
            costLabel.Width = 130;
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            CostValue = new TextBox();
            CostValue.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            CostValue.Text = MaterialReceiving.Cost.ToString();
            Controls.Add(CostValue);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка:";
            noteLabel.Width = 130;
            noteLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            Controls.Add(noteLabel);

            NoteValue = new TextBox();
            NoteValue.Location = new Point(noteLabel.Location.X + noteLabel.Width + 15, noteLabel.Location.Y);
            NoteValue.Text = MaterialReceiving.Note;
            Controls.Add(NoteValue);

            EditButton = new Button();
            EditButton.Text = "Изменить";
            EditButton.Location = new Point(30, noteLabel.Location.Y + noteLabel.Height + 15);
            EditButton.Click += EditButtonOnClick;
            Controls.Add(EditButton);

            CloseButton = new Button();
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true; 
            CloseButton.Location = new Point(EditButton.Location.X + EditButton.Width + 15, EditButton.Location.Y);
            CloseButton.Click += (sender, args) => Close();
            Controls.Add(CloseButton);
        }

        private async void EditButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var isDateCorrect = InputOperations.TryGetCorrectData(DateValue.Text, out var date);
            var isQuantityCorrect = InputOperations.TryGetPositiveDecimal(QuantityValue.Text, out var quantity);
            var isCostCorrect = InputOperations.TryGetPositiveDecimalOrZero(CostValue.Text, out var cost);
            var remainder = quantity;
            var isNoteCorrect = InputOperations.TryGetNotEmptyName(NoteValue.Text, 50, out var note);

            if (!isDateCorrect || !isQuantityCorrect || !isCostCorrect || !isNoteCorrect)
            {
                MessageBox.Show("Поля заполнены неверно", "Внимание");
                EnableButtons();
                return;
            }

            var oldMaterialReceiving = ItemsFactory.CreateMaterialReceiving(MaterialReceiving.Material,
                MaterialReceiving.Date, MaterialReceiving.Quantity, MaterialReceiving.Cost, MaterialReceiving.Quantity,
                MaterialReceiving.Note, MaterialReceiving.Index);
            var newMaterialReceiving = ItemsFactory.CreateMaterialReceiving(MaterialReceiving.Material, date, quantity, cost, remainder, note, MaterialReceiving.Index);
            try
            {
                await MaterialReceivingOperations.Edit(oldMaterialReceiving, newMaterialReceiving);
            }
            catch (OrderItemOperationException exception)
            {
                try
                {
                    await MaterialReceivingOperations.Default(newMaterialReceiving, oldMaterialReceiving);
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
                EnableButtons();
                return;
            }
            catch (NpgsqlException npgsqlException)
            {
                MessageBox.Show(npgsqlException.Message, "Внимание");
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
