using System;
using System.Windows.Forms;

using DBSysCore;

namespace DBSysReport
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        private void tryAuth(object sender, EventArgs e)
        {
            string login = inputLogin.Text;
            string pass = inputPass.Text;

            if (login == "" || pass == "")
            {
                MessageBox.Show("Введите значения в поля логин и пароль",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StatusCode result = Core.Login(login, pass);
            switch (result)
            {
                case StatusCode.Ok:
                    DialogResult = DialogResult.OK;
                    Close();
                    break;

                case StatusCode.LoginNoRegisteredUsers:
                    MessageBox.Show("В системе нет зарегистрированных пользователей",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case StatusCode.LoginInvalidLogin:
                    MessageBox.Show("Пользователь с данным логином не найден!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case StatusCode.LoginInvalidPass:
                    MessageBox.Show("Неверный пароль!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;


                default:
                    MessageBox.Show($"Возникла непредвиденная ошибка.\n\nКод: {result}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
