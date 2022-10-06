using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace ManagementAccounting
{
    public partial class AddMaterialForm : Form
    {
        private Button addButton { get; }
        private Button closeButton { get; }
        private ComboBox materialTypes { get; }
        private TextBox nameLine { get; }
        private IProgramBlock _programBlock { get; }
        private IOperationsWithUserInput _inputOperations { get; }
        
        public AddMaterialForm(IProgramBlock block, IOperationsWithUserInput inputOperations)
        {
            _inputOperations = inputOperations;
            _programBlock = block;
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
            materialTypes.Items.AddRange(_inputOperations.GetTranslateTypesNames(Enum.GetNames(typeof(MaterialType))));
            Controls.Add(materialTypes);

            addButton = new Button();
            addButton.Location = new Point(50, typeLabel.Location.Y + typeLabel.Height + 25);
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
            var name = nameLine.Text;
            if (!_inputOperations.IsNameCorrect(name) || materialType == null)
            {
                MessageBox.Show("Наименование материала и(или) тип введены неверно", "Внимание");
                return;
            }

            try
            {
                var material = (IMaterial)_programBlock.GetNewBlockItem(Enum.Parse<MaterialType>(_inputOperations.TranslateType((string)materialType)), name);
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
