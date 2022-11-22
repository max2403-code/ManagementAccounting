using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagementAccounting.Interfaces.Common;
using Npgsql;

namespace ManagementAccounting
{
    public partial class LoginForm : Form
    {
        private MainForm MainForm { get; }
        private TextBox LoginTBox { get; }
        private TextBox PasswordTBox { get; }
        private Button OkButton { get; }
        private Button CloseButton { get; }
        private ISignIn SignIn { get; }

        public LoginForm(MainForm mainForm, ISignIn signIn)
        {
            SignIn = signIn;
            MainForm = mainForm;
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
            
            LoginTBox = new TextBox();
            LoginTBox.Location = new Point(loginLabel.Location.X + loginLabel.Width + 10, loginLabel.Location.Y);
            LoginTBox.Width = 290;
            LoginTBox.Text = "postgres";
            Controls.Add(LoginTBox);
            
            var passwordLabel = new Label();
            passwordLabel.Location = new Point(5, loginLabel.Location.Y + loginLabel.Height + 10);
            passwordLabel.Width = 50;
            passwordLabel.Text = "Пароль";
            Controls.Add(passwordLabel);
            
            PasswordTBox = new TextBox();
            PasswordTBox.Location = new Point(passwordLabel.Location.X + passwordLabel.Width + 10, passwordLabel.Location.Y);
            PasswordTBox.Width = 290;
            PasswordTBox.Text = "user";
            Controls.Add(PasswordTBox);
            
            OkButton = new Button();
            OkButton.Location = new Point(110, PasswordTBox.Location.Y + PasswordTBox.Height + 20);
            OkButton.AutoSize = true;
            OkButton.Text = "Ок";
            OkButton.Click += OkButtonOnClick;
            Controls.Add(OkButton);
            
            CloseButton = new Button();
            CloseButton.Location = new Point(210, OkButton.Location.Y);
            CloseButton.AutoSize = true;
            CloseButton.Text = "Отмена";
            CloseButton.Click += CloseButtonOnClick;
            Controls.Add(CloseButton);
        }

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void OkButtonOnClick(object sender, EventArgs e)
        {
            var login = LoginTBox.Text;
            var password = PasswordTBox.Text;
            OkButton.Enabled = false;
            CloseButton.Enabled = false;
            try
            {
                await SignIn.SigningIn(login, password);
            }
            catch (NpgsqlException exception)
            {
                
                MessageBox.Show(exception.Message, "Внимание!");
                OkButton.Enabled = true;
                CloseButton.Enabled = true;
                return;
            }

            MainForm.LoginCompleted = true;
            Close();
        }
    }
}
