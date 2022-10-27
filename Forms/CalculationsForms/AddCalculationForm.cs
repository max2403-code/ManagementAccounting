using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class AddCalculationForm : Form
    {
        private Button addButton { get; }
        private Button closeButton { get; }
        private TextBox nameLine { get; }
        private IOperationsWithUserInput inputOperations { get; }
        private IItemsFactory itemsFactory { get; }

        public AddCalculationForm(IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory)
        {
            this.itemsFactory = itemsFactory;
            this.inputOperations = inputOperations;
            Size = new Size(500, 500);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление калькуляции";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            nameLabel.Width = 100;
            nameLabel.Text = "Наименование калькуляции:";
            Controls.Add(nameLabel);

            nameLine = new TextBox();
            nameLine.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            nameLine.Width = 200;
            Controls.Add(nameLine);

            addButton = new Button();
            addButton.Location = new Point(50, nameLabel.Location.Y + nameLabel.Height + 25);
            addButton.Text = "Добавить";
            addButton.AutoSize = true;
            addButton.Click += AddButtonOnClick;
            Controls.Add(addButton);

            closeButton = new Button();
            closeButton.Location = new Point(addButton.Location.X + addButton.Width + 20, addButton.Location.Y);
            closeButton.Text = "Отмена";
            closeButton.AutoSize = true;
            closeButton.Click += CloseButtonOnClick;
            Controls.Add(closeButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var name = inputOperations.GetNotEmptyName(nameLine.Text, 50);

                var calculation = itemsFactory.CreateCalculation(name) as EditingBlockItemDB;
                await calculation.AddItemToDataBase();
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
