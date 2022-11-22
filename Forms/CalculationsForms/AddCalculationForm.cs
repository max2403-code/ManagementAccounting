using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;
using Npgsql;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class AddCalculationForm : Form
    {
        private Button AddButton { get; }
        private Button CloseButton { get; }
        private TextBox NameLine { get; }
        private List<Button> Buttons { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IItemsFactory ItemsFactory { get; }
        private IFormFactory FormFactory { get; }


        public AddCalculationForm(IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory, IFormFactory formFactory)
        {
            FormFactory = formFactory;
            ItemsFactory = itemsFactory;
            InputOperations = inputOperations;
            Size = new Size(500, 500);
            Buttons = new List<Button>();
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление калькуляции";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            nameLabel.Width = 150;
            nameLabel.Text = "Наименование кальк-ии:";
            Controls.Add(nameLabel);

            NameLine = new TextBox();
            NameLine.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            NameLine.Width = 200;
            Controls.Add(NameLine);

            AddButton = new Button();
            AddButton.Location = new Point(50, nameLabel.Location.Y + nameLabel.Height + 25);
            AddButton.Text = "Добавить";
            AddButton.AutoSize = true;
            AddButton.Click += AddButtonOnClick;
            Controls.Add(AddButton);
            Buttons.Add(AddButton);

            CloseButton = new Button();
            CloseButton.Location = new Point(AddButton.Location.X + AddButton.Width + 20, AddButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += (sender, args) => Close();
            Controls.Add(CloseButton);
            Buttons.Add(CloseButton);
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var isNameCorrect = InputOperations.TryGetNotEmptyName(NameLine.Text, 50, out var name);

            if (!isNameCorrect)
            {
                MessageBox.Show("Наименование введено неверно", "Внимание");
                EnableButtons();
                return;
            }
            
            var calculation = ItemsFactory.CreateCalculation(name) as EditingBlockItemDB;

            try
            {
                await calculation.AddItemToDataBase();
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
                return;
            }

            var calculationForm = FormFactory.CreateCalculationForm((ICalculation) calculation);
            calculationForm.ShowDialog();
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
