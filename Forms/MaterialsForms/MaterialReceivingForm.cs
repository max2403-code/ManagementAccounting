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

namespace ManagementAccounting.Forms.RemaindersForms
{
    public partial class MaterialReceivingForm : Form
    {
        private IMaterialReceiving MaterialReceiving { get; set; }
        private Label TopLabel { get; }

        //private TextBox dateTextBox { get; }
        private Label QuantityValue { get; }
        private Label CostValue { get; }
        private Label PriceValue { get; }

        private Label RemainderValue { get; }
        private Label NoteValue { get; }
        private Label MaterialNameValue { get; }
        private Button EditReceivingButton { get; }
        private Button RemoveReceivingButton { get; }
        private IFormFactory FormFactory { get; }
        private IOperationsWithUserInput OperationsWithUserInput { get; }
        private ISystemMaterialReceivingOperations MaterialReceivingOperations { get; }




        public MaterialReceivingForm(IMaterialReceiving materialReceiving, IFormFactory formFactory, IOperationsWithUserInput operationsWithUserInput, ISystemMaterialReceivingOperations materialReceivingOperations)
        {
            FormFactory = formFactory;
            OperationsWithUserInput = operationsWithUserInput;
            MaterialReceiving = materialReceiving;
            MaterialReceivingOperations = materialReceivingOperations;

            Size = new Size(400, 600);

            TopLabel = new Label();
            TopLabel.TextAlign = ContentAlignment.MiddleCenter;
            TopLabel.Dock = DockStyle.Top;
            TopLabel.Text = MaterialReceiving.Name;
            Controls.Add(TopLabel);

            var materialNameLabel = new Label();
            materialNameLabel.Text = "Материал:";
            materialNameLabel.Width = 130;
            materialNameLabel.Location = new Point(10, TopLabel.Location.Y + TopLabel.Height + 15);
            Controls.Add(materialNameLabel);

            MaterialNameValue = new Label();
            MaterialNameValue.Location = new Point(materialNameLabel.Location.X + materialNameLabel.Width + 15, materialNameLabel.Location.Y);
            MaterialNameValue.Text = materialReceiving.Material.Name;
            MaterialNameValue.Width = 200;
            Controls.Add(MaterialNameValue);


            var quantityLabel = new Label();
            quantityLabel.Text = "Количество:";
            quantityLabel.Width = 130;
            quantityLabel.Location = new Point(10, materialNameLabel.Location.Y + materialNameLabel.Height + 15);
            Controls.Add(quantityLabel);

            QuantityValue = new Label();
            QuantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            QuantityValue.Text = string.Join(' ', MaterialReceiving.Quantity.ToString(), OperationsWithUserInput.TranslateType(MaterialReceiving.Material.Unit.ToString())) ;
            QuantityValue.AutoSize = true;
            Controls.Add(QuantityValue);

            var costLabel = new Label();
            costLabel.Text = "Стоимость:";
            costLabel.Width = 130;
            costLabel.Location = new Point(10, quantityLabel.Location.Y + quantityLabel.Height + 15);
            Controls.Add(costLabel);

            CostValue = new Label();
            CostValue.Location = new Point(costLabel.Location.X + costLabel.Width + 15, costLabel.Location.Y);
            CostValue.Text = string.Join(' ', MaterialReceiving.Cost.ToString(), "руб.") ;
            CostValue.AutoSize = true;
            Controls.Add(CostValue);

            var priceLabel = new Label();
            priceLabel.Text = "Цена за ед.:";
            priceLabel.Width = 130;
            priceLabel.Location = new Point(10, costLabel.Location.Y + costLabel.Height + 15);
            Controls.Add(priceLabel);

            PriceValue = new Label();
            PriceValue.Location = new Point(priceLabel.Location.X + priceLabel.Width + 15, priceLabel.Location.Y);
            PriceValue.Text = string.Join(' ', MaterialReceiving.Price.ToString(), "руб.") ;
            PriceValue.AutoSize = true;
            Controls.Add(PriceValue);

            var remainderLabel = new Label();
            remainderLabel.Text = "Остаток на складе:";
            remainderLabel.Width = 130;
            remainderLabel.Location = new Point(10, priceLabel.Location.Y + priceLabel.Height + 15);
            Controls.Add(remainderLabel);

            RemainderValue = new Label();
            RemainderValue.Location = new Point(remainderLabel.Location.X + remainderLabel.Width + 15, remainderLabel.Location.Y);
            RemainderValue.Text = string.Join(' ', MaterialReceiving.Remainder.ToString(), OperationsWithUserInput.TranslateType(MaterialReceiving.Material.Unit.ToString())) ;
            RemainderValue.AutoSize = true;
            Controls.Add(RemainderValue);

            var noteLabel = new Label();
            noteLabel.Text = "Заметка:";
            noteLabel.Width = 130;
            noteLabel.Location = new Point(10, remainderLabel.Location.Y + remainderLabel.Height + 15);
            Controls.Add(noteLabel);

            NoteValue = new Label();
            NoteValue.Location = new Point(noteLabel.Location.X + noteLabel.Width + 15, noteLabel.Location.Y);
            NoteValue.Text = MaterialReceiving.Note;
            Controls.Add(NoteValue);

            EditReceivingButton = new Button();
            EditReceivingButton.Text = "Изменить";
            EditReceivingButton.Location = new Point(30, noteLabel.Location.Y + noteLabel.Height + 15);
            EditReceivingButton.Click += EditReceivingButtonOnClick;
            Controls.Add(EditReceivingButton);

            RemoveReceivingButton = new Button();
            RemoveReceivingButton.Text = "Удалить";
            RemoveReceivingButton.Location = new Point(EditReceivingButton.Location.X + EditReceivingButton.Width + 15, EditReceivingButton.Location.Y);
            RemoveReceivingButton.Click += RemoveReceivingButtonOnClick;
            Controls.Add(RemoveReceivingButton);
        }

        private async void RemoveReceivingButtonOnClick(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить поступление?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;

            try
            {
                await MaterialReceivingOperations.Remove(MaterialReceiving);
            }
            catch(OrderItemOperationException exception)
            {
                await MaterialReceivingOperations.Insert(MaterialReceiving, true);

                MessageBox.Show(exception.Message, "Внимание");
                return;
            }

            Close();
        }

        private void EditReceivingButtonOnClick(object sender, EventArgs e)
        {
            var editReceivingForm = FormFactory.CreateEditMaterialReceivingForm(MaterialReceiving, this);
            editReceivingForm.ShowDialog();

            TopLabel.Text = MaterialReceiving.Name;
            QuantityValue.Text = MaterialReceiving.Quantity.ToString();
            CostValue.Text = MaterialReceiving.Cost.ToString();
            RemainderValue.Text = MaterialReceiving.Remainder.ToString();
            NoteValue.Text = MaterialReceiving.Note;
        }

        public void UpdateMaterialReceiving(IMaterialReceiving materialReceiving)
        {
            MaterialReceiving = materialReceiving;
        }
    }
}
