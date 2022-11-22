using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Factory;
using Npgsql;

namespace ManagementAccounting
{
    public partial class AddMaterialForm : Form
    {
        private Button AddButton { get; }
        private Button CloseButton { get; }
        private ComboBox MaterialTypes { get; }
        private ComboBox UnitTypes { get; }
        private TextBox NameLine { get; }
        private List<Button> Buttons { get; }
        private IOperationsWithUserInput InputOperations { get; }
        private IItemsFactory ItemsFactory { get; }


        public AddMaterialForm(IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory)
        {
            ItemsFactory = itemsFactory;
            InputOperations = inputOperations;
            Size = new Size(400, 600);
            Buttons = new List<Button>();

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление материала";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            nameLabel.Width = 130;
            nameLabel.Text = "Наименование:";
            Controls.Add(nameLabel);

            NameLine = new TextBox();
            NameLine.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            NameLine.Width = 120;
            Controls.Add(NameLine);

            var typeLabel = new Label();
            typeLabel.Location = new Point(10, nameLabel.Location.Y + nameLabel.Height + 20);
            typeLabel.Width = 130;
            typeLabel.Text = "Тип материала:";
            Controls.Add(typeLabel);

            MaterialTypes = new ComboBox();
            MaterialTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            MaterialTypes.Location = new Point(typeLabel.Location.X + typeLabel.Width + 10, typeLabel.Location.Y);
            MaterialTypes.Items.AddRange(this.InputOperations.GetTranslateTypesNames(Enum.GetNames(typeof(MaterialType))));
            Controls.Add(MaterialTypes);

            var unitLabel = new Label();
            unitLabel.Location = new Point(10, typeLabel.Location.Y + typeLabel.Height + 20);
            unitLabel.Width = 130;
            unitLabel.Text = "Единица измерения:";
            Controls.Add(unitLabel);

            UnitTypes = new ComboBox();
            UnitTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            UnitTypes.Location = new Point(unitLabel.Location.X + unitLabel.Width + 10, unitLabel.Location.Y);
            UnitTypes.Items.AddRange(this.InputOperations.GetTranslateTypesNames(Enum.GetNames(typeof(UnitOfMaterial))));
            Controls.Add(UnitTypes);

            AddButton = new Button();
            AddButton.Location = new Point(50, unitLabel.Location.Y + unitLabel.Height + 25);
            AddButton.Text = "Добавить";
            AddButton.AutoSize = true;
            AddButton.Click += AddButtonOnClick;
            Controls.Add(AddButton);
            Buttons.Add(AddButton);

            CloseButton = new Button();
            CloseButton.Location = new Point(AddButton.Location.X + AddButton.Width + 20, AddButton.Location.Y);
            CloseButton.Text = "Отмена";
            CloseButton.AutoSize = true;
            CloseButton.Click += CloseButtonOnClick;
            Controls.Add(CloseButton);
            Buttons.Add(CloseButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }

        private async void AddButtonOnClick(object sender, EventArgs e)
        {
            DisableButtons();
            var materialType = MaterialTypes.SelectedItem;
            var unitType = UnitTypes.SelectedItem;
            var isNameCorrect = InputOperations.TryGetNotEmptyName(NameLine.Text, 50, out var name);

            if (materialType == null || unitType == null || !isNameCorrect) 
            {
                MessageBox.Show("Наименование, тип материала и(или) единица измерения введены неверно", "Внимание");
                EnableButtons();
                return;
            }

            try
            {
                var material = ItemsFactory.CreateMaterial(Enum.Parse<MaterialType>(InputOperations.TranslateType((string)materialType)), name, Enum.Parse < UnitOfMaterial > (InputOperations.TranslateType((string)unitType))) as EditingBlockItemDB;
                await material.AddItemToDataBase();
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
