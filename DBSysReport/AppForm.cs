using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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

            // load available test types in statistics tab
            InitModuleNames();
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

        private void InitModuleNames()
        {
            // walk through the first sequnce and gather all needed information
            StatusCode status = Core.GetModules(out List<Module> modules);

            switch (status)
            {
                case StatusCode.Ok:
                    moduleComboBox.Items.Clear();
                    foreach (Module module in modules)
                        moduleComboBox.Items.Add(module);
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"При попытке инициализации панели выбора модуля " +
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
                selectedStaff.password = Utils.GetHash(passwordInput.Text);
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
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "dump files (*.db)|*.db",
                RestoreDirectory = true
            };

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
            StatusCode status = Core.GenerateReport(challenge, out string fileName);

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

        private void ShowStatistics(object sender, EventArgs e)
        {
            // setup labels
            DateTime beginDate = beginDateTimePicker.Value;
            DateTime endDate = endDateTimePicker.Value;
            periodLabel1.Text = $"В период с {beginDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
            periodLabel2.Text = $"В период с {beginDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}\n" +
                $"В модуле {moduleComboBox.SelectedItem}";
            periodLabel3.Text = $"В период с {beginDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}\n" +
                $"Команды \"{commandComboBox.SelectedItem}\"\n" +
                $"В модуле {moduleComboBox.SelectedItem}";

            // grab all nessesary data for tables and graphs
            List<TestDynamic> testsList;
            StatusCode status;

            if (allTimeCheckBox.Checked)
                status = Core.GetDynamicTests(out testsList);
            else
                status = Core.GetDynamicTests(beginDate, endDate, out testsList);

            switch (status)
            {
                case StatusCode.Ok:
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"Произошла ошибка при попытке загрузить данные тестирования.\n\nКод ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                    return;
            }

            List<int> modulesFailsStats = Enumerable.Repeat(0, moduleComboBox.Items.Count).ToList();
            List<int> commandsFailsStats = Enumerable.Repeat(0, commandComboBox.Items.Count).ToList();

            foreach (TestDynamic test in testsList)
            {
                if (test.status == false)
                {
                    TestStatic testStatic = test.GetTestStatic(); // не очень классная функция, надо заменить
                    modulesFailsStats[testStatic.module.id] += 1;

                    int cmdIndex = commandComboBox.Items.IndexOf(testStatic);
                    commandsFailsStats[cmdIndex] += 1;
                }
            }

            // FILL FAIL TESTS DATA WITH RANDOM NUMBERS JUST FOR TESTING PURPOSES
            Random random = new Random();
            for (int i = 0; i < modulesFailsStats.Count; i++)
                modulesFailsStats[i] = random.Next(0, 10);
            for (int i = 0; i < commandsFailsStats.Count; i++)
                commandsFailsStats[i] = random.Next(0, 10);

            // fill first table

            statTable_Product.SuspendLayout();

            statTable_Product.RowCount = 1 + moduleComboBox.Items.Count;
            statTable_Product.RowStyles.Clear();
            for (int i = 0; i < moduleComboBox.Items.Count; i++)
                statTable_Product.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            for (int i = 3; i < statTable_Product.Controls.Count; i++)
                statTable_Product.Controls.RemoveAt(3);
    
            for (int i = 0; i < moduleComboBox.Items.Count; i++)
            {
                Label indexLabel = new Label()
                {
                    Text = (i + 1).ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                Label moduleName = new Label()
                {
                    Text = moduleComboBox.Items[i].ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                Label failsAmountLabel = new Label()
                {
                    Text = modulesFailsStats[i].ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                statTable_Product.Controls.Add(indexLabel, 0, i + 1);
                statTable_Product.Controls.Add(moduleName, 1, i + 1);
                statTable_Product.Controls.Add(failsAmountLabel, 2, i + 1);
            }

            statTable_Product.ResumeLayout();

            // setup product stats chart

            productStatsChart.SuspendLayout();

            productStatsChart.Series.Clear();
            productStatsChart.Titles.Clear();
            productStatsChart.Titles.Add("Статистика отказов во всех модулях");

            Series series = productStatsChart.Series.Add("кол-во отказов");
            series.ChartType = SeriesChartType.Column;

            for (int i = 0; i < moduleComboBox.Items.Count; i++)
            {
                string labelString = moduleComboBox.Items[i].ToString();
                series.Points.AddXY(labelString, modulesFailsStats[i]);
                series.Points[i].AxisLabel = labelString;
            }

            productStatsChart.ResumeLayout();

            // fill second table

            statTable_Module.SuspendLayout();

            statTable_Module.RowCount = 1 + commandComboBox.Items.Count;
            statTable_Module.RowStyles.Clear();
            for (int i = 0; i < commandComboBox.Items.Count; i++)
                statTable_Module.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            while (statTable_Module.Controls.Count > 4)
                statTable_Module.Controls.RemoveAt(4);

            for (int i = 0; i < commandComboBox.Items.Count; i++)
            {
                Label indexLabel = new Label()
                {
                    Text = (i + 1).ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                Label commandName = new Label()
                {
                    Text = commandComboBox.Items[i].ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                Label modeName = new Label()
                {
                    Text = ((TestStatic)commandComboBox.Items[i]).mode,
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                Label failsAmountLabel = new Label()
                {
                    Text = commandsFailsStats[i].ToString(),
                    AutoSize = true,
                    Anchor = AnchorStyles.None
                };

                statTable_Module.Controls.Add(indexLabel, 0, i + 1);
                statTable_Module.Controls.Add(commandName, 1, i + 1);
                statTable_Module.Controls.Add(modeName, 2, i + 1);
                statTable_Module.Controls.Add(failsAmountLabel, 3, i + 1);
            }

            statTable_Module.ResumeLayout();

            // setup product stats chart

            moduleStatsChart.SuspendLayout();

            moduleStatsChart.Series.Clear();
            moduleStatsChart.Titles.Clear();
            moduleStatsChart.Titles.Add("Статистика отказов в указанном модуле");

            List<string> modes = modeComboBox.Items.Cast<string>().ToList();
            string selectedMode = (string)modeComboBox.SelectedItem;
            if (selectedMode == "Все режимы")
            {
                foreach (string mode in modes)
                {
                    if (mode == "Все режимы")
                        continue;

                    series = moduleStatsChart.Series.Add($"кол-во отказов в режиме \"{mode}\"");
                    series.ChartType = SeriesChartType.Column;

                    for (int i = 0; i < commandComboBox.Items.Count; i++)
                    {
                        TestStatic test = (TestStatic)commandComboBox.Items[i];
                        if (test.mode == mode)
                        {
                            string labelString = test.ToString();
                            series.Points.AddXY(labelString, commandsFailsStats[i]);
                            series.Points[series.Points.Count - 1].AxisLabel = labelString;
                        }
                    }
                }
            }
            else
            {
                series = moduleStatsChart.Series.Add($"кол-во отказов в режиме \"{selectedMode}\"");
                series.ChartType = SeriesChartType.Column;

                for (int i = 0; i < commandComboBox.Items.Count; i++)
                {
                    string labelString = commandComboBox.Items[i].ToString();
                    series.Points.AddXY(labelString, commandsFailsStats[i]);
                    series.Points[i].AxisLabel = labelString;
                }
            }


            moduleStatsChart.ResumeLayout();

            // the last table

            statTable_Command.SuspendLayout();

            statTable_Command.RowCount = 1;
            statTable_Command.RowStyles.Clear();

            while (statTable_Command.Controls.Count > 7)
                statTable_Command.Controls.RemoveAt(7);

            int testIndex = 1;
            foreach (TestDynamic test in testsList)
            {
                if (commandComboBox.Items.Contains(test.GetTestStatic()))
                {
                    Label indexLabel = new Label()
                    {
                        Text = testIndex.ToString(),
                        AutoSize = true,
                        Anchor = AnchorStyles.None
                    };

                    Label dateLabel = new Label()
                    {
                        Text = test.beginTime.ToString("dd.MM.yyyy"),
                        AutoSize = true,
                        Anchor = AnchorStyles.None
                    };

                    Label serialNumberLabel = new Label()
                    {
                        Text = test.challenge.controllObject.serialNumber,
                        AutoSize = true,
                        Anchor = AnchorStyles.None
                    };

                    Label nominalLabel = new Label()
                    {
                        Text = test.nominal.HasValue ? test.nominal.ToString() : "-",
                        AutoSize = true,
                        Anchor = AnchorStyles.None
                    };

                    Label boundaryLabel = new Label()
                    {
                        Text = test.boundaryValue.HasValue ? test.boundaryValue.ToString() : "-",
                        AutoSize = true,
                        Anchor = AnchorStyles.None
                    };

                    Label valueLabel = new Label()
                    {
                        Text = test.actualValue.HasValue ? test.actualValue.ToString() : "-",
                        AutoSize = true,
                        Anchor = AnchorStyles.None
                    };

                    Label statusLabel = new Label()
                    {
                        Text = test.status ? "Годен" : "Не годен",
                        AutoSize = true,
                        ForeColor = test.status ? System.Drawing.Color.DarkGreen : System.Drawing.Color.DarkRed,
                        Anchor = AnchorStyles.None
                    };

                    statTable_Command.RowCount++;
                    statTable_Command.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    statTable_Command.Controls.Add(indexLabel, 0, testIndex);
                    statTable_Command.Controls.Add(dateLabel, 1, testIndex);
                    statTable_Command.Controls.Add(serialNumberLabel, 2, testIndex);
                    statTable_Command.Controls.Add(nominalLabel, 3, testIndex);
                    statTable_Command.Controls.Add(boundaryLabel, 4, testIndex);
                    statTable_Command.Controls.Add(valueLabel, 5, testIndex);
                    statTable_Command.Controls.Add(statusLabel, 6, testIndex);

                    testIndex++;
                }
            }

            statTable_Command.ResumeLayout();
        }

        private void allTimeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allTimeCheckBox.CheckState == CheckState.Checked)
            {
                beginDateTimePicker.Enabled = false;
                endDateTimePicker.Enabled = false;
            }
            else
            {
                beginDateTimePicker.Enabled = true;
                endDateTimePicker.Enabled = true;
            }
        }

        private void moduleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // load modes available for selected module

            // first disable "show" button
            showStatisticsButton.Enabled = false;
            commandComboBox.Enabled = false;

            // get selected module
            int moduleId = ((Module)moduleComboBox.SelectedItem).id;

            StatusCode status = Core.GetStaticTests(out List<TestStatic> testsList);

            switch (status)
            {
                case StatusCode.Ok:
                    modeComboBox.Enabled = true;
                    modeComboBox.Items.Clear();
                    modeComboBox.Items.Add("Все режимы");
                    foreach (TestStatic test in testsList)
                        if (test.module.id == moduleId && !modeComboBox.Items.Contains(test.mode))
                            modeComboBox.Items.Add(test.mode);
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"Произошла ошибка при попытке загрузить список доступных режимов.\n\nКод ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                    break;
            }
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // load command names from here

            // first disable "show" button
            showStatisticsButton.Enabled = false;

            // get selected module
            int moduleId = ((Module)moduleComboBox.SelectedItem).id;

            // get selected mode
            string mode = (string)modeComboBox.SelectedItem;

            StatusCode status = Core.GetStaticTests(out List<TestStatic> testsList);

            switch (status)
            {
                case StatusCode.Ok:
                    commandComboBox.Enabled = true;
                    commandComboBox.Items.Clear();
                    foreach (TestStatic test in testsList)
                        if (test.module.id == moduleId && (mode == "Все режимы" || test.mode == mode))
                            commandComboBox.Items.Add(test);
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"Произошла ошибка при попытке загрузить названия команд.\n\nКод ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                    break;
            }
        }

        private void commandComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showStatisticsButton.Enabled = true;
        }
    }
}
