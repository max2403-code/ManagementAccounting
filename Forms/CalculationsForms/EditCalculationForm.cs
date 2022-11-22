﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Items;
using Npgsql;

namespace ManagementAccounting.Forms.CalculationsForms
{
    public partial class EditCalculationForm : Form
    {
        private Button EditButton { get; }
        private Button CloseButton { get; }
        private List<Button> Buttons { get; }
        private TextBox NameLine { get; }
        private ICalculation Calculation { get; }
        private IOperationsWithUserInput InputOperations { get; }

        public EditCalculationForm(ICalculation calculation, IOperationsWithUserInput inputOperations)
        {
            InputOperations = inputOperations;
            Calculation = calculation;
            Buttons = new List<Button>();

            Size = new Size(400, 600);


            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение калькуляции";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            nameLabel.Width = 100;
            nameLabel.Text = "Наименование:";
            Controls.Add(nameLabel);

            NameLine = new TextBox();
            NameLine.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            NameLine.Width = 200;
            Controls.Add(NameLine);

            
            EditButton = new Button();
            EditButton.Location = new Point(50, nameLabel.Location.Y + nameLabel.Height + 25);
            EditButton.Text = "Изменить";
            EditButton.AutoSize = true;
            EditButton.Click += EditButtonOnClick;
            Controls.Add(EditButton);
            Buttons.Add(EditButton);

            CloseButton = new Button();
            CloseButton.Location = new Point(EditButton.Location.X + EditButton.Width + 20, EditButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += (sender, args) => Close();
            Controls.Add(CloseButton);
            Buttons.Add(CloseButton);
        }

        private async void EditButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var isNameCorrect = InputOperations.TryGetNotEmptyName(NameLine.Text, 50, out var name);

            if (!isNameCorrect)
            {
                MessageBox.Show("Наименование введено неверно", "Внимание");
                EnableButtons();
                return;
            }

            try
            {
                await ((EditingBlockItemDB)Calculation).EditItemInDataBase<ICalculation>(name);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show(exception.Message, "Внимание");
                EnableButtons();
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