using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class EditCalculationForm : Form
    {
        private Button editButton { get; }
        private Button closeButton { get; }
        
        private TextBox nameLine { get; }
        private ICalculation calculation { get; }
        private IOperationsWithUserInput inputOperations { get; }

        public EditCalculationForm(ICalculation calculation, IOperationsWithUserInput inputOperations)
        {
            this.inputOperations = inputOperations;
            this.calculation = calculation;

            Size = new Size(400, 600);


            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение калькуляции";
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

            
            editButton = new Button();
            editButton.Location = new Point(50, nameLabel.Location.Y + nameLabel.Height + 25);
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
                var name = inputOperations.GetNotEmptyName(nameLine.Text, 50);

                await ((EditingBlockItemDB)calculation).EditItemInDataBase<ICalculation>(name);
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
