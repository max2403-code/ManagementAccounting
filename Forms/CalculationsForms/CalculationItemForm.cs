using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class CalculationItemForm : Form
    {
        private ICalculationItem CalculationItem { get; set; }
        private IOperationsWithUserInput InputOperations { get; }
        private Label TopLabel { get; }

        //private TextBox dateTextBox { get; }
        private Label QuantityValue { get; }
        private Label MaterialNameValue { get; }
        private Button EditReceivingButton { get; }
        private Button RemoveReceivingButton { get; }
        private IFormFactory FormFactory { get; }

        public CalculationItemForm(ICalculationItem calculationItem, IFormFactory formFactory, IOperationsWithUserInput inputOperations)
        {
            FormFactory = formFactory;
            InputOperations = inputOperations;
            CalculationItem = calculationItem;

            Size = new Size(400, 600);

            TopLabel = new Label();
            TopLabel.TextAlign = ContentAlignment.MiddleCenter;
            TopLabel.Dock = DockStyle.Top;
            TopLabel.Text = "Статья калькуляции";
            Controls.Add(TopLabel);

            var materialNameLabel = new Label();
            materialNameLabel.Text = "Материал:";
            materialNameLabel.Width = 100;
            materialNameLabel.Location = new Point(10, TopLabel.Location.Y + TopLabel.Height + 15);
            Controls.Add(materialNameLabel);

            MaterialNameValue = new Label();
            MaterialNameValue.Location = new Point(materialNameLabel.Location.X + materialNameLabel.Width + 15, materialNameLabel.Location.Y);
            MaterialNameValue.Text = calculationItem.Material.Name;
            MaterialNameValue.Width = 200;
            Controls.Add(MaterialNameValue);


            var quantityLabel = new Label();
            quantityLabel.Text = "Норма расхода:";
            quantityLabel.Location = new Point(10, materialNameLabel.Location.Y + materialNameLabel.Height + 15);
            Controls.Add(quantityLabel);

            QuantityValue = new Label();
            QuantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            QuantityValue.Text = string.Join(' ', calculationItem.Consumption.ToString(), InputOperations.TranslateType(calculationItem.Material.Unit.ToString())) ;
            QuantityValue.AutoSize = true;
            Controls.Add(QuantityValue);

            EditReceivingButton = new Button();
            EditReceivingButton.Text = "Изменить";
            EditReceivingButton.Location = new Point(30, quantityLabel.Location.Y + quantityLabel.Height + 15);
            EditReceivingButton.Click += EditCalcItemButtonOnClick;
            Controls.Add(EditReceivingButton);

            RemoveReceivingButton = new Button();
            RemoveReceivingButton.Text = "Удалить";
            RemoveReceivingButton.Location = new Point(EditReceivingButton.Location.X + EditReceivingButton.Width + 15, EditReceivingButton.Location.Y);
            RemoveReceivingButton.Click += RemoveCalcItemButtonOnClick;
            Controls.Add(RemoveReceivingButton);
        }

        private async void RemoveCalcItemButtonOnClick(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить статью калькуляции?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;

            try
            {
                await ((BlockItemDB)CalculationItem).RemoveItemFromDataBase();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                return;
            }
            
            Close();
        }

        private void EditCalcItemButtonOnClick(object sender, EventArgs e)
        {
            var editReceivingForm = FormFactory.CreateEditCalculationItemForm(CalculationItem);
            editReceivingForm.ShowDialog();
            QuantityValue.Text = string.Join(' ', CalculationItem.Consumption.ToString(), InputOperations.TranslateType(CalculationItem.Material.Unit.ToString()));
        }


    }
}
