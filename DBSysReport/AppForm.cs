using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using DBSysCore;
using DBSysCore.Model;

namespace DBSysReport
{
    public partial class AppForm : Form
    {
        private List<Staff> allStaff;
        private Staff newStaff;

        private List<Challenge> allChallenges;

        public AppForm()
        {
            InitializeComponent();
        }

        private void GenerateNewStaff()
        {
            newStaff = new Staff()
            {
                surname = "+ добавить",
                firstName = ".",
                patronymicName = "",
                post = "operator",
                department = "",
                login = "",
                password = ""
            };
        }

        // setup post choises
        private void InitPostsItems()
        {
            string[] posts = { "admin", "operator", "tester" };
            postInput.Items.AddRange(posts);
        }

        // show existing users in user list
        private void UpdateUsersList()
        {
            usersList.BeginUpdate();

            usersList.Items.Clear();

            // select all users from db
            allStaff = Core.GetAllUsers();
            // option for creating new user
            GenerateNewStaff();
            usersList.Items.Add(newStaff);
            foreach (Staff staff in allStaff)
                usersList.Items.Add(staff);
            usersList.EndUpdate();
        }

        private void AppForm_Load(object sender, EventArgs e)
        {
            // call login form, if unauthorized
            if (!Session.IsLoggedIn())
            {
                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog(this) != DialogResult.OK)
                    Close();
            }

            if (Session.RequireGrants(UserGrants.Admin))
            {
                UpdateUsersList();
                InitPostsItems();
            }
            else
            {
                // if current user is not admin - remove unnesesary panels
                usersPanelBox.Hide();
                dumpFilePanelBox.Hide();
                sqlPanelBox.Hide();

                reportPanelBox.Dock = DockStyle.Fill;
            }

            // setup current dump file
            Session.Open();
            currentDumpFileInput.Text = Session.sessionData.filename;
            Session.Close();

            // setup report panel
            InitReportPanel();
        }

        private void InitReportPanel()
        {
            StatusCode status = Core.GetAllChallenges(out allChallenges);

            switch (status)
            {
                case StatusCode.Ok:
                    if (allChallenges.Count == 0)
                        challengeDates.Items.Add("нет данных");
                    else
                        challengeDates.Items.AddRange(allChallenges.ToArray());
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"При попытке инициализации панели генерации отчётов " +
                        $"произошла ошибка.\n\nКод ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                    break;
            }
        }

        private void LogoutAndExit(object sender, EventArgs e)
        {
            if (MessageBox.Show("Действительно выйти из учётной записи?\nВы также выйдете из программы.",
                "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Session.Open();
                Session.Logout();
                Session.Close();
                Close();
            }
        }

        private void usersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // fill out data in form
            Staff selectedStaff = (Staff)(usersList.SelectedItem);

            // if new staff selected - fill special
            if (selectedStaff == newStaff)
            {
                surnameInput.Text = "Фамилия";
                nameInput.Text = "Имя";
                patronymicNameInput.Text = "Отчество";

                postInput.SelectedItem = "operator";
                departmentInput.Text = "отдел";

                loginInput.Text = "логин";
                passwordInput.Text = "пароль";

                // switch enable state for button
                setEmployerDataButton.Enabled = false;
                addEmployerDataButton.Enabled = true;
            }
            else
            {
                surnameInput.Text = selectedStaff.surname;
                nameInput.Text = selectedStaff.firstName;
                patronymicNameInput.Text = selectedStaff.patronymicName;

                postInput.SelectedItem = selectedStaff.post;
                departmentInput.Text = selectedStaff.department;

                loginInput.Text = selectedStaff.login;

                // switch enable state for button
                setEmployerDataButton.Enabled = true;
                addEmployerDataButton.Enabled = false;
            }

        }

        private void setEmployerDataButton_Click(object sender, EventArgs e)
        {
            Staff selectedStaff = (Staff)(usersList.SelectedItem);

            // check if all inputed dataa is valid (skip for now)
            // TODO: check input!

            selectedStaff.surname = surnameInput.Text;
            selectedStaff.firstName = nameInput.Text;
            selectedStaff.patronymicName = patronymicNameInput.Text;

            selectedStaff.post = (string)postInput.SelectedItem;
            selectedStaff.department = departmentInput.Text;

            if (selectedStaff == newStaff)
            {
                selectedStaff.login = loginInput.Text;
                selectedStaff.password = passwordInput.Text;
            }
            else if (passwordInput.Text.Length != 0)
            {
                if (MessageBox.Show("Подтвердите смену логина и пароля",
                    "Смена логина и пароля", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    selectedStaff.login = loginInput.Text;
                    selectedStaff.password = Utils.GetHash(passwordInput.Text);
                }
            }

            StatusCode status = Core.UpdateStaff(selectedStaff);
            if (status != StatusCode.Ok)
            {
                MessageBox.Show($"Произошла ошибка при попытке сохранить данные!\n\nКод ошибки:\n{status}",
                    "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void selectDumpFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.Filter = "dump files (*.db)|*.db";
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                currentDumpFileInput.Text = filePath;

                Session.Open();
                Session.sessionData.filename = filePath;
                Session.Close();
            }
        }

        private void LoadStaticTests(object sender, EventArgs e)
        {
            StatusCode status = Core.LoadStaticTests();
            if (status != StatusCode.Ok)
            {
                MessageBox.Show("Не удалось загрузить тестовые данные!\n" +
                    "Проверьте наличие файла статических тестов и его корректность.\n\n" +
                    $"Код ошибки: {status}",
                    "Ошибка загрузки", MessageBoxButtons.OK);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Session.Open();

                // send sql statement
                string query = sqlInput.Text;
                query = query.Replace("\"", "'");
                sqlInput.Text = "";

                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sqlite3.exe",
                        Arguments = $"{Session.sessionData.filename} \"{query}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    line = Utils.StdEncToUTF8(line);
                    sqlOutput.Text += line + "\n";
                }

                process.WaitForExit();

                Session.Close();
            }
        }

        private void addEmployerDataButton_Click(object sender, EventArgs e)
        {
            newStaff.GenerateId();
            setEmployerDataButton_Click(sender, e);
            UpdateUsersList();
        }

        private void challengeDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allChallenges.Count != 0)
                createReportButton.Enabled = true;
        }

        private void createReportButton_Click(object sender, EventArgs e)
        {
            Challenge challenge = (Challenge)challengeDates.SelectedItem;
            string fileName;
            StatusCode status = Core.GenerateReport(challenge, out fileName);

            switch (status)
            {
                case StatusCode.Ok:
                    MessageBox.Show("Отчёт создан успешно", "Успех", MessageBoxButtons.OK);
                    pdfViewer.src = fileName;
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"Произошла ошибка при попытке создать отчёт.\n\nКод ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                    break;
            }
        }
    }
}
