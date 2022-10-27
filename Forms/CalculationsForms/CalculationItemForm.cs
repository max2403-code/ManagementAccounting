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
        private ICalculationItem calculationItem { get; set; }
        private IOperationsWithUserInput _inputOperations { get; }
        private Label topLabel { get; }

        //private TextBox dateTextBox { get; }
        private Label quantityValue { get; }
        
        private Label materialNameValue { get; }
        private Button editReceivingButton { get; }
        private Button removeReceivingButton { get; }
        private IFormFactory formFactory { get; }

        public CalculationItemForm(ICalculationItem calculationItem, IFormFactory formFactory, IOperationsWithUserInput inputOperations)
        {
            this.formFactory = formFactory;
            _inputOperations = inputOperations;
            this.calculationItem = calculationItem;

            Size = new Size(400, 600);

            topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = calculationItem.Name;
            Controls.Add(topLabel);

            var materialNameLabel = new Label();
            materialNameLabel.Text = "Материал";
            materialNameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 15);
            Controls.Add(materialNameLabel);

            materialNameValue = new Label();
            materialNameValue.Location = new Point(materialNameLabel.Location.X + materialNameLabel.Width + 15, materialNameLabel.Location.Y);
            materialNameValue.Text = calculationItem.Material.Name;
            materialNameValue.Width = 200;
            Controls.Add(materialNameValue);


            var quantityLabel = new Label();
            quantityLabel.Text = "Количество";
            quantityLabel.Location = new Point(10, materialNameLabel.Location.Y + materialNameLabel.Height + 15);
            Controls.Add(quantityLabel);

            quantityValue = new Label();
            quantityValue.Location = new Point(quantityLabel.Location.X + quantityLabel.Width + 15, quantityLabel.Location.Y);
            quantityValue.Text = calculationItem.Consumption.ToString();
            Controls.Add(quantityValue);

            

            editReceivingButton = new Button();
            editReceivingButton.Text = "Изменить";
            editReceivingButton.Location = new Point(30, quantityLabel.Location.Y + quantityLabel.Height + 15);
            editReceivingButton.Click += EditCalcItemButtonOnClick;
            Controls.Add(editReceivingButton);

            removeReceivingButton = new Button();
            removeReceivingButton.Text = "Удалить";
            removeReceivingButton.Location = new Point(editReceivingButton.Location.X + editReceivingButton.Width + 15, editReceivingButton.Location.Y);
            removeReceivingButton.Click += RemoveCalcItemButtonOnClick;
            Controls.Add(removeReceivingButton);
        }

        private async void RemoveCalcItemButtonOnClick(object? sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Удалить статью калькуляции?", "Внимание", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;
            await ((BlockItemDB)calculationItem).RemoveItemFromDataBase();
            Close();
        }

        private void EditCalcItemButtonOnClick(object? sender, EventArgs e)
        {
            var editReceivingForm = formFactory.CreateEditCalculationItemForm(calculationItem);
            editReceivingForm.ShowDialog();

            quantityValue.Text = calculationItem.Consumption.ToString();
            
        }

        
    }
}
