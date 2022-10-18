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
        private Button addButton { get; }
        private Button closeButton { get; }
        private ComboBox materialTypes { get; }
        private ComboBox unitTypes { get; }
        private TextBox nameLine { get; }
        private IOperationsWithUserInput inputOperations { get; }
        private IItemsFactory itemsFactory { get; }


        public AddMaterialForm(IOperationsWithUserInput inputOperations, IItemsFactory itemsFactory)
        {
            this.itemsFactory = itemsFactory;
            this.inputOperations = inputOperations;
            Size = new Size(400, 600);

            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Добавление материала";
            Controls.Add(topLabel);

            var nameLabel = new Label();
            nameLabel.Location = new Point(10, topLabel.Location.Y + topLabel.Height + 50);
            nameLabel.Width = 100;
            nameLabel.Text = "Наименование материала:";
            Controls.Add(nameLabel);

            nameLine = new TextBox();
            nameLine.Location = new Point(nameLabel.Location.X + nameLabel.Width + 10, nameLabel.Location.Y);
            nameLine.Width = 200;
            Controls.Add(nameLine);

            var typeLabel = new Label();
            typeLabel.Location = new Point(10, nameLabel.Location.Y + nameLabel.Height + 20);
            typeLabel.Width = 100;
            typeLabel.Text = "Тип материала:";
            Controls.Add(typeLabel);

            materialTypes = new ComboBox();
            materialTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            materialTypes.Location = new Point(typeLabel.Location.X + typeLabel.Width + 10, typeLabel.Location.Y);
            materialTypes.Items.AddRange(this.inputOperations.GetTranslateTypesNames(Enum.GetNames(typeof(MaterialType))));
            Controls.Add(materialTypes);

            var unitLabel = new Label();
            unitLabel.Location = new Point(10, typeLabel.Location.Y + typeLabel.Height + 20);
            unitLabel.Width = 100;
            unitLabel.Text = "Единица измерения:";
            Controls.Add(unitLabel);

            unitTypes = new ComboBox();
            unitTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            unitTypes.Location = new Point(unitLabel.Location.X + unitLabel.Width + 10, unitLabel.Location.Y);
            unitTypes.Items.AddRange(this.inputOperations.GetTranslateTypesNames(Enum.GetNames(typeof(UnitOfMaterial))));
            Controls.Add(unitTypes);

            addButton = new Button();
            addButton.Location = new Point(50, unitLabel.Location.Y + unitLabel.Height + 25);
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
            var materialType = materialTypes.SelectedItem;
            var unitType = unitTypes.SelectedItem;
            var name = nameLine.Text;
            if (!inputOperations.IsNameCorrect(name) || materialType == null || unitType == null) 
            {
                MessageBox.Show("Наименование материала и(или) тип введены неверно", "Внимание");
                return;
            }

            try
            {
                var material = itemsFactory.CreateMaterial(Enum.Parse<MaterialType>(inputOperations.TranslateType((string)materialType)), name, Enum.Parse < UnitOfMaterial > (inputOperations.TranslateType((string)unitType)));
                await material.AddItemToDataBase();
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
