using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Forms.RemaindersForms
{
    public partial class MaterialReceivingForm : Form
    {
        private IMaterialReceiving _materialReceiving { get; set; }
        private IOperationsWithUserInput _inputOperations { get; }
        private Label topLabel { get; }

        //private TextBox dateTextBox { get; }
        private Label quantityValue { get; }
        private Label costValue { get; }
        private Label remainderValue { get; }
        private Label noteValue { get; }
        private Label materialNameValue { get; }
        private Button editReceivingButton { get; }
        private Button removeReceivingButton { get; }
        private IFormFactory formFactory { get; }



        public MaterialReceivingForm(IMaterialReceiving materialReceiving, IFormFactory formFactory, IOperationsWithUserInput inputOperations)
        {
            this.formFactory = formFactory;
            _inputOperations = inputOperations;
            _materialReceiving = materialReceiving;

            Size = new Size(400, 600);

            topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = _materialReceiving.Name;
            Controls.Add(topLabel);

            var materialNameLabel = new Label();
            materialNameLabel.Text = "Материал";
            materialNameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 15);
            Controls.Add(materialNameLabel);

            materialNameValue = new Label();
            materialNameValue.Location = new Point(materialNameLabel.Location.X + materialNameLabel.Width + 15, materialNameLabel.Location.Y);
            materialNameValue.Text = materialReceiving.Material.Name;
            materialNameValue.Width = 200;
            Controls.Add(materialNameValue);


            var quantityLabel = new Label();
            quantityLabel.Text = "Количество";
            quantityLabel.Location = new Point(10, materialNameLabel.Location.Y + materialNameLabel.Height + 15);
            Controls.Add(quantityLabel);

            quantityValue = new Label();
            quantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            quantityValue.Text = _materialReceiving.Quantity.ToString();
            Controls.Add(quantityValue);

            var costLabel = new Label();
            costLabel.Text = "Стоимость";
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            costValue = new Label();
            costValue.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            costValue.Text = _materialReceiving.Cost.ToString();
            Controls.Add(costValue);

            var remainderLabel = new Label();
            remainderLabel.Text = "Остаток на складе";
            remainderLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            Controls.Add(remainderLabel);

            remainderValue = new Label();
            remainderValue.Location = new Point(remainderLabel.Location.X + remainderLabel.Width + 15, remainderLabel.Location.Y);
            remainderValue.Text = _materialReceiving.Remainder.ToString();
            Controls.Add(remainderValue);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка";
            noteLabel.Location = new Point(10, remainderLabel.Location.Y + remainderLabel.Height + 15);
            Controls.Add(noteLabel);

            noteValue = new Label();
            noteValue.Location = new Point(noteLabel.Location.X + noteLabel.Width + 15, noteLabel.Location.Y);
            noteValue.Text = _materialReceiving.Note;
            Controls.Add(noteValue);

            editReceivingButton = new Button();
            editReceivingButton.Text = "Изменить";
            editReceivingButton.Location = new Point(30, noteLabel.Location.Y + noteLabel.Height + 15);
            editReceivingButton.Click += EditReceivingButtonOnClick;
            Controls.Add(editReceivingButton);

            removeReceivingButton = new Button();
            removeReceivingButton.Text = "Удалить";
            removeReceivingButton.Location = new Point(editReceivingButton.Location.X + editReceivingButton.Width + 15, editReceivingButton.Location.Y);
            removeReceivingButton.Click += RemoveReceivingButtonOnClick;
            Controls.Add(removeReceivingButton);
        }

        private async void RemoveReceivingButtonOnClick(object? sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить поступление?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;
            await ((BlockItemDB)_materialReceiving).RemoveItemFromDataBase();
            Close();
        }

        private void EditReceivingButtonOnClick(object? sender, EventArgs e)
        {
            var editReceivingForm = formFactory.CreateEditMaterialReceivingForm(_materialReceiving, this);
            editReceivingForm.ShowDialog();

            topLabel.Text = _materialReceiving.Name;
            quantityValue.Text = _materialReceiving.Quantity.ToString();
            costValue.Text = _materialReceiving.Cost.ToString();
            remainderValue.Text = _materialReceiving.Remainder.ToString();
            noteValue.Text = _materialReceiving.Note;
        }

        public void UpdateMeterialReceiving(IMaterialReceiving materialReceiving)
        {
            _materialReceiving = materialReceiving;
        }
    }
}
