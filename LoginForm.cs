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
    
    public partial class LoginForm : Form
    {
        private MainForm _mainForm { get; }
        private TextBox loginTBox { get; }
        private TextBox passwordTBox { get; }
        private IDataBase _dataBase { get; }

        public LoginForm(MainForm mainForm, IDataBase dataBase)
        {
            _dataBase = dataBase;
            _mainForm = mainForm;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Size = new Size(400, 300);
            var topLabel = new Label();
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            topLabel.Dock = DockStyle.Top;
            topLabel.Text = "Введите логин и пароль для входа в PostgreSQL";
            topLabel.Font = new Font(topLabel.Font, FontStyle.Bold);
            Controls.Add(topLabel);
            var loginLabel = new Label();
            loginLabel.Location = new Point(5, topLabel.Location.Y + topLabel.Height + 5);
            loginLabel.Width = 50;
            loginLabel.Text = "Логин";
            Controls.Add(loginLabel);
            loginTBox = new TextBox();
            loginTBox.Location = new Point(loginLabel.Location.X + loginLabel.Width + 10, loginLabel.Location.Y);
            loginTBox.Width = 290;
            loginTBox.Text = "postgres";
            Controls.Add(loginTBox);
            var passwordLabel = new Label();
            passwordLabel.Location = new Point(5, loginLabel.Location.Y + loginLabel.Height + 10);
            passwordLabel.Width = 50;
            passwordLabel.Text = "Пароль";
            Controls.Add(passwordLabel);
            passwordTBox = new TextBox();
            passwordTBox.Location = new Point(passwordLabel.Location.X + passwordLabel.Width + 10, passwordLabel.Location.Y);
            passwordTBox.Width = 290;
            passwordTBox.Text = "user";
            Controls.Add(passwordTBox);
            var okButton = new Button();
            okButton.Location = new Point(110, passwordTBox.Location.Y + passwordTBox.Height + 20);
            okButton.AutoSize = true;
            okButton.Text = "Ок";
            okButton.Click += OkButtonOnClick;
            Controls.Add(okButton);
            var closeButton = new Button();
            closeButton.Location = new Point(210, okButton.Location.Y);
            closeButton.AutoSize = true;
            closeButton.Text = "Отмена";
            closeButton.Click += CloseButtonOnClick;
            Controls.Add(closeButton);
        }

        private void CloseButtonOnClick(object? sender, EventArgs e)
        {
            this.Close();
        }

        private async void OkButtonOnClick(object? sender, EventArgs e)
        {
            var login = loginTBox.Text;
            var password = passwordTBox.Text;

            try
            {
                await _dataBase.SignIn(login, password);
            }
            catch (NpgsqlException exception)
            {
                MessageBox.Show("Неверный логин и(или) пароль", "Внимание!");
                return;
            }

            _mainForm.LoginCompleted = true;
            Close();
        }
    }
}
