using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                post = Session.GrantsString(UserGrants.Operator),
                department = "",
                login = "",
                password = ""
            };
        }

        // setup post choises
        private void InitPostsItems()
        {
            foreach (UserGrants grants in Enum.GetValues(typeof(UserGrants)))
                postInput.Items.Add(Session.GrantsString(grants));
        }

        // show existing users in user list
        private void UpdateUsersList()
        {
#if DEBUG
            Debug.Assert(Session.RequireGrants(UserGrants.Admin),
                "admin must be authorized to continue");
#endif

            StatusCode status = Core.GetAllUsers(out allStaff);
            switch (status) // select all users from db
            {
                case StatusCode.Ok:
                    usersList.BeginUpdate();
                    usersList.Items.Clear();
                    
                    GenerateNewStaff(); // option for creating new user
                    usersList.Items.Add(newStaff);

                    foreach (Staff staff in allStaff)
                        usersList.Items.Add(staff);

                    usersList.EndUpdate();
                    break;

                default:
                case StatusCode.Error:
                    MessageBox.Show($"При попытке инициализации списка пользователей " +
                        $"произошла ошибка.\n\nКод ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                    break;
            }

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
            currentDumpFileInput.Text = Session.GetCurrentWorkingDumpFileNameWithPath();

            // setup report panel
            InitReportPanel();

            // load available test types in statistics tab
            InitModuleNames();

            groupBox2.Parent.Controls.Remove(groupBox2);
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
                Core.Logout();
                Close();
            }
        }

        private void usersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // fill out data in form
            Staff selectedStaff = (Staff)usersList.SelectedItem;

            // if new staff selected - fill special
            if (selectedStaff == newStaff)
            {
                surnameInput.Text = "Фамилия";
                nameInput.Text = "Имя";
                patronymicNameInput.Text = "Отчество";

                postInput.SelectedItem = Session.GrantsString(UserGrants.Operator);
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
            Staff selectedStaff = (Staff)usersList.SelectedItem;

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
                InitialDirectory = Paths.dumpsDirectory,
                Filter = "dump files (*.db)|*.db"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                currentDumpFileInput.Text = filePath;

                Core.DumpUse(filePath);
            }
        }

        private void LoadStaticTests(object sender, EventArgs e)
        {
            if (MessageBox.Show("Загрузка данных может занять некоротое время (около 1 минуты). " +
                "В это время приложение не будет реагировать на Ваши действия. Продолжить?",
                "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                StatusCode status = Core.LoadStaticTests();
                if (status == StatusCode.Ok)
                {
                    MessageBox.Show("Загрузка данных прошла успешно!",
                        "Завершение загрузки", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить тестовые данные!\n" +
                        "Проверьте наличие файла статических тестов и его корректность.\n\n" +
                        $"Код ошибки: {status}",
                        "Ошибка загрузки", MessageBoxButtons.OK);
                }
            }
        }

        private void InputSQLCommand(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string query = sqlInput.Text;
                query = query.Replace('"', '\'');
                StatusCode status = Core.ExecSQL(query, out string result);

                switch (status)
                {
                    case StatusCode.Ok:
                        sqlOutput.Text += $"{result}\n";
                        break;

                    default:
                        MessageBox.Show("Не удалось выполнить SQL запрос!\n" +
                            $"Код ошибки: {status}",
                            "Ошибка выполнения SQL запроса", MessageBoxButtons.OK);
                        break;
                }
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

            // show challenge data here
            Challenge challenge = (Challenge)challengeDates.SelectedItem;
            Core.GetFPGAVersion(challenge, out string FPGAVersion);
            ChallengeDataLabel.Text = $"- Серийный номер изделия:\n" +
                $"{challenge.controllObject.product}\n\n" +
                $"- Наименование объекта контроля:\n" +
                $"{challenge.controllObject.name}\n\n" +
                $"- Серийный номер объекта контроля:\n" +
                $"{challenge.controllObject.decimalNumber}\n\n" +
                $"- Заводской номер объекта контроля:\n" +
                $"{challenge.controllObject.serialNumber}\n\n" +
                $"- В составе изделия:\n" +
                $"{challenge.controllObject.parent}\n\n" +
                $"- Вид испытания:\n" +
                $"{challenge.challengeType.name}\n\n" +
                $"- Место проведения испытания:\n" +
                $"{challenge.location.name}\n\n" +
                $"- Оператор:{challenge.staff}\n" +
                $"- Представитель ОТК:{challenge.staffOTK}\n\n" +
                $"- Описание:\n" +
                $"{challenge.description}\n\n" +
                $"- Версия ПЛИС: {FPGAVersion}";

            ChallengeDataLabel.Visible = true;
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

        private bool GrabDataForStatistics(out List<TestDynamic> testsDynamicList, out List<TestStatic> testsStaticList, out List<int> modulesFailsStats, out List<int> commandsFailsStats)
        {
            StatusCode status;
            
            modulesFailsStats = Enumerable.Repeat(0, moduleComboBox.Items.Count).ToList();
            commandsFailsStats = Enumerable.Repeat(0, commandComboBox.Items.Count).ToList();
            testsStaticList = new List<TestStatic>();

            if (allTimeCheckBox.Checked)
                status = Core.GetDynamicTests(out testsDynamicList);
            else
            {
                DateTime beginDate = beginDateTimePicker.Value;
                DateTime endDate = endDateTimePicker.Value;
                status = Core.GetDynamicTests(beginDate, endDate, out testsDynamicList);
            }

            if (status != StatusCode.Ok)
            {
                MessageBox.Show($"Произошла ошибка при попытке загрузить данные тестирования.\n\n" +
                    $"Код ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                return false;
            }

            status = Core.MapStaticTests(testsDynamicList, out testsStaticList);
            if (status != StatusCode.Ok)
            {
                MessageBox.Show($"Произошла ошибка при попытке загрузить статические тесты.\n\n" +
                    $"Код ошибки: {status}", "Ошибка", MessageBoxButtons.OK);
                return false;
            }

            for (int i = 0; i < testsDynamicList.Count; i++)
            {
                TestDynamic testDynamic = testsDynamicList[i];
                TestStatic testStatic = testsStaticList[i];

                if (testDynamic.status == false)
                {
                    modulesFailsStats[testStatic.module.id] += 1;

                    // int cmdIndex = commandComboBox.Items.IndexOf(testStatic);
                    int cmdIndex = commandComboBox.Items.Cast<TestStatic>().ToList().
                        FindIndex(test => test.tsIndex == testStatic.tsIndex);
                    commandsFailsStats[cmdIndex] += 1;
                }
            }

            return true;
        }

        private void UpdateStatisticsDataLables()
        {
            if (allTimeCheckBox.Checked)
            {
                periodLabel1.Text = $"За весь период";
                periodLabel2.Text = $"За весь период\n" +
                    $"В модуле {moduleComboBox.SelectedItem}";
                periodLabel3.Text = $"За весь период\n" +
                    $"Команды \"{commandComboBox.SelectedItem}\"\n" +
                    $"В модуле {moduleComboBox.SelectedItem}";
            }
            else
            {
                DateTime beginDate = beginDateTimePicker.Value;
                DateTime endDate = endDateTimePicker.Value;
                periodLabel1.Text = $"В период с {beginDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                periodLabel2.Text = $"В период с {beginDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}\n" +
                    $"В модуле {moduleComboBox.SelectedItem}";
                periodLabel3.Text = $"В период с {beginDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}\n" +
                    $"Команды \"{commandComboBox.SelectedItem}\"\n" +
                    $"В модуле {moduleComboBox.SelectedItem}";
            }
        }

        private Label CreateTableLabel(string text)
        {
            return new Label()
            {
                Text = text,
                AutoSize = true,
                Anchor = AnchorStyles.None
            };
        }

        private void ClearTable(TableLayoutPanel table, int columnsCount, int itemsCount)
        {
            table.RowCount = 1; // + moduleComboBox.Items.Count;
            table.RowStyles.Clear();
            for (int i = 0; i < itemsCount; i++)
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            while (table.Controls.Count > columnsCount)
                table.Controls.RemoveAt(columnsCount);
        }

        private void ShowStatistics(object sender, EventArgs e)
        {
            // grab all nessesary data for tables and graphs
            if (!GrabDataForStatistics(out List<TestDynamic> testsDynamicList, out List<TestStatic> testsStaticList,
                    out List<int> modulesFailsStats, out List<int> commandsFailsStats))
                return;

            // setup labels
            UpdateStatisticsDataLables();

            // suspend all layouts for update

            statTable_Product.SuspendLayout();
            productStatsChart.SuspendLayout();
            statTable_Module.SuspendLayout();
            moduleStatsChart.SuspendLayout();
            statTable_Command.SuspendLayout();

            // fill first table

            ClearTable(statTable_Product, 3, moduleComboBox.Items.Count);
    
            for (int i = 0; i < moduleComboBox.Items.Count; i++)
            {
                Label indexLabel = CreateTableLabel((i + 1).ToString());
                Label moduleName = CreateTableLabel(moduleComboBox.Items[i].ToString());
                Label failsAmountLabel = CreateTableLabel(modulesFailsStats[i].ToString());

                statTable_Product.Controls.Add(indexLabel, 0, i + 1);
                statTable_Product.Controls.Add(moduleName, 1, i + 1);
                statTable_Product.Controls.Add(failsAmountLabel, 2, i + 1);
            }

            // setup product stats chart

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

            // fill second table

            ClearTable(statTable_Module, 4, commandComboBox.Items.Count);

            for (int i = 0; i < commandComboBox.Items.Count; i++)
            {
                Label indexLabel = CreateTableLabel((i + 1).ToString());
                Label commandName = CreateTableLabel(commandComboBox.Items[i].ToString());
                Label modeName = CreateTableLabel(((TestStatic)commandComboBox.Items[i]).mode);
                Label failsAmountLabel = CreateTableLabel(commandsFailsStats[i].ToString());

                statTable_Module.Controls.Add(indexLabel, 0, i + 1);
                statTable_Module.Controls.Add(commandName, 1, i + 1);
                statTable_Module.Controls.Add(modeName, 2, i + 1);
                statTable_Module.Controls.Add(failsAmountLabel, 3, i + 1);
            }

            // setup product stats chart

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

            // the last table

            ClearTable(statTable_Command, 7, 0);

            int testIndex = 1;
            for (int i = 0; i < testsDynamicList.Count; i++)
            {
                TestDynamic testDynamic = testsDynamicList[i];
                TestStatic testStatic = testsStaticList[i];

                // if (commandComboBox.Items.Cast<TestStatic>().ToList().Exists(test => test.tsIndex == testStatic.tsIndex))
                if (((TestStatic)commandComboBox.SelectedItem).tsIndex == testStatic.tsIndex)
                {
                    Label indexLabel = CreateTableLabel(testIndex.ToString());
                    Label dateLabel = CreateTableLabel(testDynamic.beginTime.ToString("dd.MM.yyyy"));
                    Label serialNumberLabel = CreateTableLabel(testDynamic.challenge.controllObject.serialNumber);
                    Label nominalLabel = CreateTableLabel(testStatic.unit == "null" ? testDynamic.nominal.ToString() : "-");
                    Label boundaryLabel = CreateTableLabel(testStatic.unit == "null" ? testDynamic.boundaryValue.ToString() : "-");
                    Label valueLabel = CreateTableLabel(testStatic.unit == "null" ? testDynamic.actualValue.ToString() : "-");
                    Label statusLabel = CreateTableLabel(testDynamic.status ? "Годен" : "Не годен");

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

            statTable_Product.ResumeLayout();
            productStatsChart.ResumeLayout();
            statTable_Module.ResumeLayout();
            moduleStatsChart.ResumeLayout();
            statTable_Command.ResumeLayout();

            statisticTableProduct.Visible = true;
            statisticTableModule.Visible = true;
            statisticTableCommand.Visible = true;
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

        private void ShowDatabasePanel(object sender, EventArgs e)
        {
            mainSplitContainer.Panel1Collapsed = false;
            mainSplitContainer.Panel2Collapsed = true;
        }

        private void ShowReportPanel(object sender, EventArgs e)
        {
            mainSplitContainer.Panel1Collapsed = true;
            mainSplitContainer.Panel2Collapsed = false;
        }

        private void ShowBothPanels(object sender, EventArgs e)
        {
            mainSplitContainer.Panel1Collapsed = false;
            mainSplitContainer.Panel2Collapsed = false;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программное обеспечение DBSysReport © DSPLAB, 2020\n\n" +
                "Используйте это ПО для визуализации результатов тестирования.\n\n" +
                "При возникновении критических ошибок сообщите о них разработчику.\n" +
                "Для обратной связи используйте электронный адрес klimenko0037@gmail.com", "О программе", MessageBoxButtons.OK);
        }
    }
}
