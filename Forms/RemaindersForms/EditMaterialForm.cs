using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ManagementAccounting
{
    public partial class EditMaterialForm : Form
    {
        private Button editButton { get; }
        private Button closeButton { get; }
        private ComboBox materialTypes { get; }
        private TextBox nameLine { get; }
        private IMaterial _material { get; }
        private IOperationsWithUserInput _inputOperations { get; }

        public EditMaterialForm(IMaterial material, IOperationsWithUserInput inputOperations)
        {
            _inputOperations = inputOperations;
            _material = material;
            
            Size = new Size(400, 600);


            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Изменение материала";
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

            editButton = new Button();
            editButton.Location = new Point(50, typeLabel.Location.Y + typeLabel.Height + 25);
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
            var materialType = materialTypes.SelectedItem;
            var name = nameLine.Text;
            if (!_inputOperations.IsNameCorrect(name) || materialType == null)
            {
                MessageBox.Show("Наименование материала и(или) тип введены неверно", "Внимание");
                return;
            }

            try
            {
                await _material.EditItemInDataBase(Enum.Parse<MaterialType>(_inputOperations.TranslateType((string)materialType)), name);
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
