using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExcelDataReader;
using OpenHtmlToPdf;
using HtmlAgilityPack;

using DBSysCore.Model;

namespace DBSysCore
{
    /**
     * Класс реализующий все базовые операции над БД, в том числе администрирование пользователей.
     */
    public static class Core
    {
        /**
         * Название файла, используемого для хранения данных
         * о пользователях независимо от обычных файлов дампов.
         * 
         * В дальнейшем - пользовательский файл.
         */
        private const string usersfilename = ".users";

        private const string staticDataTable = "static.xls";

        private const string reportDirectory = "report";

        private const string reportTemplateDirectory = "report\\templates";

        /**
         * Соединение для пользовательского файла.
         */
        public static SQLiteConnection usersConnection;

        /**
         * Соединение для текущего файла дампов.
         */
        public static SQLiteConnection dumpConnection;

        /**
         * Инициализирует соединение с текущим файлом дампа.
         * 
         * Возвращает false, если соединение прошло успешно.
         */
        private static StatusCode InitializeConnection()
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                // Build Connection String
                string fname = Session.sessionData.filename;
                string queryString = $"URI=file:{fname}";

                dumpConnection = new SQLiteConnection(queryString);
                if (dumpConnection == null)
                    throw new Exception("Could not create connection.");

                dumpConnection.Open();
            }
            catch (InvalidOperationException e)
            {
                Logger.Error("Core.InitializeConnection", e.ToString());
                result = StatusCode.ConnectionInvalidOperation;
            }
            catch (SQLiteException e)
            {
                Logger.Error("Core.InitializeConnection", e.ToString());
                result = StatusCode.ConnectionSQLError;
            }
            catch (Exception e)
            {
                Logger.Error("Core.InitializeConnection", e.ToString());
                result = StatusCode.ConnectionError;
            }

            return result;
        }

        /**
         * Инициализирует соединение с файлом пользовательких данных.
         * 
         * Если файл пользовательских данных не существует - создаёт новый,
         * при этом собирает данные обо всех пользователях из всех возможных
         * источников - файлов дампов, находящихся в папке с программой.
         */
        private static void InitializeUserDataConnection()
        {
            // Build Connection String
            string queryString = $"Data Source={usersfilename}";

            // if there is no such file - create new one
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + usersfilename))
            {
                FileStream fs = File.Create($"{usersfilename}");
                fs.Close();

                // initialize created file with tables schemas
                CmdProccess("sqlite3.exe", $"{usersfilename} \".read users.sql\"", false);

                usersConnection = new SQLiteConnection(queryString);
                RetrieveAllUsers();
            }
            else
            {
                usersConnection = new SQLiteConnection(queryString);
            }

            usersConnection.Open();
        }

        /**
         * Метод для сбора данных обо всех пользователях
         * из всех файлов дампов в папке с программой.
         */
        private static void RetrieveAllUsers()
        {
            string[] dumps = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.db");

            foreach (string filename in dumps)
            {
                string fname = filename.Substring(filename.LastIndexOf("\\") + 1);
                string connstr = $"Data Source={fname}";
                SQLiteConnection tempConnection = new SQLiteConnection(connstr);

                // extract all employers data from file
                tempConnection.Open();
                List<Staff> staffList = Staff.ExtractAll(tempConnection);
                tempConnection.Close();

                // and push them into .users database...
                SaveAllStaff(staffList);
            }
        }

        /**
         * Выделяет всех сохранённых пользователей
         * из пользовательской базы данных.
         */
        public static List<Staff> GetAllUsers()
        {
            InitializeUserDataConnection();
            List<Staff> result = Staff.ExtractAll(usersConnection);
            CloseConnections();
            return result;
        }

        public static StatusCode GetAllChallenges(out List<Challenge> challengeList)
        {
            StatusCode result = StatusCode.Ok;
            challengeList = new List<Challenge>();
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    InitializeUserDataConnection();

                    string query = "SELECT [id] FROM [challenge]";
                    SQLiteDataReader reader = Utils.ExecuteReader(query, dumpConnection);

                    List<int> challengeIdList = new List<int>();

                    while (reader.Read())
                    {
                        int id = (int)reader[0];
                        challengeIdList.Add(id);
                    }

                    reader.Close();

                    foreach (int id in challengeIdList)
                    {
                        Challenge challenge = new Challenge(id);
                        challengeList.Add(challenge);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error("Core.GetAllChallenges", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode GetModules(out List<Module> modulesList)
        {
            StatusCode result = StatusCode.Ok;
            modulesList = null;
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    modulesList = Module.GetModules();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GetModules", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode GetStaticTests(out List<TestStatic> testsList)
        {
            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    testsList = TestStatic.GetTests();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GetStaticTests", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode GetDynamicTests(out List<TestDynamic> testsList)
        {
            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    testsList = TestDynamic.GetTests();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GetDynamicTests", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode GetDynamicTests(Challenge challenge, out List<TestDynamic> testsList)
        {
            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    testsList = TestDynamic.GetTests(challenge);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GetDynamicTests", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode GetDynamicTests(DateTime beginDate, DateTime endDate, out List<TestDynamic> testsList)
        {
            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    testsList = TestDynamic.GetTests(beginDate, endDate);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GetDynamicTests", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        /**
         * Производит запись информации обо всех предоставленных
         * пользователях в пользовательский файл.
         */
        private static void SaveAllStaff(List<Staff> staffList)
        {
            usersConnection.Open();

            foreach (Staff staff in staffList)
            {
                staff.SaveData(usersConnection);
                /*
                string query = "INSERT OR IGNORE INTO [staff] ([id], [surname], [first_name], "
                    + "[patronymic_name], [post], [department], [login], [password]) VALUES "
                    + $"({staff.id}, '{staff.surname}', '{staff.firstName}', '{staff.patronymicName}', "
                    + $"'{staff.post}', '{staff.department}', '{staff.login}', '{staff.password}')";
                Utils.ExecuteNonQuery(query, usersConnection);
                */
            }

            usersConnection.Close();
        }

        /**
         * Закрывает все открытые соединения.
         */
        private static void CloseConnections()
        {
            if (usersConnection != null)
                usersConnection.Close();

            if (dumpConnection != null)
                dumpConnection.Close();
        }

        /**
         * Передает данные состояния программы в строку status.
         */
        public static StatusCode Status(out string status)
        {
            StatusCode result = StatusCode.Ok;
            status = "";
            try
            {
                if (Session.IsLoggedIn())
                {
                    string grants = Session.GrantsString(Session.GetGrants());
                    status += $"  Logged in as {Session.GetUserName()} ({grants})\n";
                }
                else
                    status += "  Unauthorized.\n";

                status += $"  Current working database file: \"{Session.sessionData.filename}\"\n";

                switch (Session.sessionData.programState)
                {
                    case ProgramState.Idle:
                        status += $"  Program state: idle\n";
                        break;

                    case ProgramState.Working:
                        status += $"  Program state: working\n";
                        break;

                    case ProgramState.Testing:
                        status += $"  Program state: testing\n";
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.Status", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        /**
         * Производит авторизацию пользователя по введённому логину и паролю.
         */
        public static StatusCode Login(string login, string password)
        {
            StatusCode result;
            try
            {
                InitializeUserDataConnection();

                if (Session.IsLoggedIn())
                    result = StatusCode.LoginAlreadyAuthorized;
                else
                    result = Session.Auth(login, password);
            }
            catch (Exception e)
            {
                Logger.Error("Core.Login", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        /**
         * Производит сброс текущего авторизорованного пользователя.
         */
        public static StatusCode Logout()
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                Session.Open();
                Session.Logout();
            }
            catch
            {
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        /**
         * Иниализирует новое испытание.
         */
        public static StatusCode BeginChallenge(string productName, string coName,
            string coSerialNumber, string coDecNumber, string coVersion, string coParent,
            string challengeTypeName, string locationName, string description = "")
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
                else
                {
                    // open new challenge
                    Session.sessionData.programState = ProgramState.Testing;

                    ControllObject controllObject = new ControllObject()
                    {
                        name = coName,
                        serialNumber = coSerialNumber,
                        decimalNumber = coDecNumber,
                        version = coVersion,
                        parent = coParent,
                        product = productName
                    };
                    controllObject.GenerateId();

                    ChallengeType challengeType = new ChallengeType() { name = challengeTypeName };
                    challengeType.GenerateId();

                    Staff staff = Session.sessionData.staff;

                    Location location = new Location() { name = locationName };
                    location.GenerateId();

                    Session.sessionData.activeChallenge = new Challenge()
                    {
                        controllObject = controllObject,
                        challengeType = challengeType,
                        staff = staff,
                        location = location,
                        beginTime = DateTime.Now,
                        description = description
                    };
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.BeginChallenge", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        /**
         * Завершает и сохраняет текущее испытание.
         */
        public static StatusCode EndChallenge()
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Testing)
                    result = StatusCode.ProgramStateInvalid;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    // save active challenge
                    Session.sessionData.programState = ProgramState.Working;
                    Session.sessionData.activeChallenge.GenerateId();

                    result = Session.SaveCurrentChallenge(dumpConnection);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.EndChallenge", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        /**
         * Иниализирует новый тест. (test_dynamic)
         */
        public static StatusCode Test(string tsIndex, bool status, decimal nominal,
            decimal actualValue, decimal delta, decimal boundaryValue)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Testing)
                    result = StatusCode.ProgramStateInvalid;
                else
                {
                    TestDynamic test = new TestDynamic()
                    {
                        tsIndex = tsIndex,
                        challenge = Session.sessionData.activeChallenge,
                        beginTime = DateTime.Now,
                        nominal = nominal,
                        actualValue = actualValue,
                        delta = delta,
                        boundaryValue = boundaryValue,
                        status = status
                    };
                    test.GenerateId();

                    // add constructed test to test list
                    Session.sessionData.activeTests.Add(test);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.BeginTest", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        public static StatusCode Test(string tsIndex, bool status)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Testing)
                    result = StatusCode.ProgramStateInvalid;
                else
                {
                    TestDynamic test = new TestDynamic()
                    {
                        tsIndex = tsIndex,
                        challenge = Session.sessionData.activeChallenge,
                        beginTime = DateTime.Now,
                        nominal = null,
                        actualValue = null,
                        delta = null,
                        boundaryValue = null,
                        status = status
                    };
                    test.GenerateId();

                    // add constructed test to test list
                    Session.sessionData.activeTests.Add(test);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.BeginTest", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        /**
         * Загружает статические данные в текущий файл дампа.
         */
        public static StatusCode LoadStaticTests()
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    FileStream stream = File.Open(staticDataTable, FileMode.Open, FileAccess.Read);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

                    DataTableCollection tables = reader.AsDataSet().Tables;

                    DataRowCollection rows;

                    // clear all data first
                    CmdProccess("sqlite3.exe", $"{Session.sessionData.filename} \"DELETE FROM [methodology] WHERE 1\"");
                    CmdProccess("sqlite3.exe", $"{Session.sessionData.filename} \"DELETE FROM [requirements] WHERE 1\"");
                    CmdProccess("sqlite3.exe", $"{Session.sessionData.filename} \"DELETE FROM [module] WHERE 1\"");
                    CmdProccess("sqlite3.exe", $"{Session.sessionData.filename} \"DELETE FROM [test_static] WHERE 1\"");

                    List<Methodology> methodologies = new List<Methodology>();
                    List<Requirements> requirements = new List<Requirements>();
                    List<Module> modules = new List<Module>();

                    // load methodology and requirements

                    rows = tables["Requirements"].Rows;

                    for (int i = 3; i < rows.Count; i++)
                    {
                        try
                        {
                            object[] data = rows[i].ItemArray;
                            Methodology methodology = new Methodology()
                            {
                                id = Convert.ToInt32((double)data[0]),
                                name = (string)data[1],
                                docNumber = (string)data[2],
                            };
                            methodologies.Add(methodology);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            break;
                        }
                    }

                    for (int i = 3; i < rows.Count; i++)
                    {
                        try
                        {
                            object[] data = rows[i].ItemArray;
                            Requirements requirements_ = new Requirements()
                            {
                                id = Convert.ToInt32((double)data[5]),
                                name = (string)data[6],
                                docNumber = (string)data[7]
                            };
                            requirements.Add(requirements_);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            break;
                        }
                    }

                    for (int i = 3; i < rows.Count; i++)
                    {
                        try
                        {
                            object[] data = rows[i].ItemArray;
                            Module module = new Module()
                            {
                                id = Convert.ToInt32((double)data[10]),
                                name = (string)data[11]
                            };
                            modules.Add(module);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            break;
                        }
                    }

                    // load static test here
                    rows = tables["Protocol"].Rows;

                    // show modules
                    foreach (var m in modules)
                        Console.WriteLine(m.id + "-" + m.name);

                    for (int i = 4; i < rows.Count; i++)
                    {
                        try
                        {
                            object[] data = rows[i].ItemArray;

                            int id = Convert.ToInt32((double)data[0]);
                            string tsIndex = (string)data[1];
                            string description = (string)data[2];
                            string mode = (string)data[3];
                            int moduleId = Convert.ToInt32((double)data[4]);
                            string unit = (string)data[5];
                            int req_id = Convert.ToInt32((string)data[6]);
                            int met_id = Convert.ToInt32((string)data[7]);

                            Module module = modules.Where(c => c.id == moduleId).First();
                            Methodology methodology = methodologies.Where(m => m.id == met_id).First();
                            Requirements requirements_ = requirements.Where(r => r.id == req_id).First();

                            TestStatic test = new TestStatic()
                            {
                                id = id,
                                mode = mode,
                                tsIndex = tsIndex,
                                module = module,
                                methodology = methodology,
                                requirements = requirements_,
                                unit = unit,
                                description = description
                            };

                            test.SaveData(dumpConnection);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            break;
                        }
                    }

                    reader.Dispose();
                    reader.Close();
                    stream.Dispose();
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.LoadStaticData", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        /**
         * Метод для смены рабочего файла дампа.
         */
        public static StatusCode DumpUse(string filename)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                // TODO: add Unathorized error for same code blocks!
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else
                {
                    if (!File.Exists($"{filename}"))
                        // create new one from scratch...
                        CmdProccess("sqlite3.exe", $"{filename} \".read model.sql\"");

                    Session.sessionData.filename = filename;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.DumpUse", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        /**
         * Метод для объединения нескольких файлов дампов в один.
         */
        public static StatusCode DumpMerge(string dest, string[] src)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                src = src.Where(filename => File.Exists(filename)).ToArray();

                if (src.Length == 0)
                    result = StatusCode.DumpMergeNoFiles;
                // WARNING: are we really need to open dump connection here ?
                else if ((result = InitializeConnection()) != StatusCode.Ok)
                {
                    // pass
                }
                else
                {
                    string connectionString = $"Data Source=\"{dest}\"";
                    SQLiteConnection connection = new SQLiteConnection(connectionString);
                    connection.Open();

                    foreach (string filename in src)
                    {
                        MergeFiles(connection, filename);
                        File.Delete(filename);
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.DumpMerge", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        private static string GenerateReportFileName(Challenge challenge)
        {
            string result = $"{challenge.beginTime:yyyy_MM_dd}.pdf";
            return result;
        }

        public static StatusCode GenerateReport(Challenge challenge, out string resultFileName)
        {
            StatusCode result = StatusCode.Ok;
            resultFileName = "";
            try
            {
                CloseConnections();
                if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load($"{reportTemplateDirectory}\\universal_template.html");

                    // change encoding here!
                    doc.LoadHtml(Utils.StdEncToUTF8(doc.DocumentNode.OuterHtml));

                    HtmlNode table = doc.DocumentNode.SelectNodes("//table")[0];

                    foreach (TestDynamic testDynamic in TestDynamic.GetTests(challenge))
                    {
                        TestStatic testStatic = testDynamic.GetTestStatic();

                        HtmlNode tr = HtmlNode.CreateNode("<tr></tr>");
                        HtmlNode td;

                        td = HtmlNode.CreateNode($"<td>{testStatic.description}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testStatic.unit}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testStatic.requirements.docNumber}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testStatic.methodology.docNumber}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testDynamic.nominal}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testDynamic.delta}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testDynamic.boundaryValue}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testDynamic.actualValue}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{challenge.beginTime:dd/MM/yyyy}</td>");
                        tr.AppendChild(td);

                        for (int i = 0; i < 2; i++)
                            tr.AppendChild(HtmlNode.CreateNode("<td></td>"));

                        table.AppendChild(tr);
                    }

                    string htmlString = doc.DocumentNode.OuterHtml;

                    string reportFileName = GenerateReportFileName(challenge);
                    resultFileName = $"{Directory.GetCurrentDirectory()}\\{reportDirectory}\\{reportFileName}";

                    IPdfDocument pdf = Pdf.From(htmlString).EncodedWith("UTF-8");
                    File.WriteAllBytes(resultFileName, pdf.Content());
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GenerateReport", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        /**
         * Метод для добавления учётной записи для пользователя.
         * 
         * При попытке создать пользователя с существующими фамилией,
         * именем и отчеством то происходит смена логина и пароля.
         */
        public static StatusCode AddStaff(string surname, string firstName, string patronymicName,
            string postName, string departmentName, string login, string password)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) != StatusCode.Ok)
                {
                    // do nothing
                }
                else
                {
                    // raplce password with corresponding hash
                    password = Utils.GetHash(password);

                    // check if staff exists
                    string query = $"SELECT * FROM [staff] WHERE [surname] = \"{surname}\" AND " +
                        $"[first_name] = \"{firstName}\" AND [patronymic_name] = \"{patronymicName}\"";
                    SQLiteDataReader reader = Utils.ExecuteReader(query, dumpConnection);

                    if (reader.HasRows)
                    {
                        // update if exists
                        query = $"UPDATE [staff] SET [post] = \"{postName}\", [department] = " +
                            $"\"{departmentName}\", [login] = \"{login}\", [password] = \"{password}\"";
                        Utils.ExecuteNonQuery(query, dumpConnection);
                    }
                    else
                    {
                        // create new staff
                        query = $"INSERT INTO [staff] ([surname], [first_name], [patronymic_name], [post], " +
                            $"[department], [login], [password]) VALUES (\"{surname}\", \"{firstName}\", " +
                            $"\"{patronymicName}\", \"{postName}\", \"{departmentName}\", \"{login}\", \"{password}\")";
                        Utils.ExecuteNonQuery(query, dumpConnection);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.AddStaff", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode UpdateStaff(Staff staff)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                Session.Open();

                if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else
                {
                    InitializeUserDataConnection();
                    staff.SaveData(usersConnection);
                    CloseConnections();

                    if ((result = InitializeConnection()) == StatusCode.Ok)
                        staff.SaveData(dumpConnection);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.UpdateStaff", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode ExecSQL(string query)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                    CmdProccess("sqlite3.exe", $"{Session.sessionData.filename} \"{query}\"", true);
            }
            catch (Exception e)
            {
                Logger.Error("Core.ExecSQL", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode ExecSQLout(string query, out string output)
        {
            StatusCode result = StatusCode.Ok;
            try
            {
                output = "";
                
                if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
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
                        output += line;
                    }

                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.ExecSQL", e.ToString());
                result = StatusCode.Error;
                output = "[Internal error]\n";
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        /**
         * Метод для объединения данных из файла с названием source по переданному соединению.
         */
        private static void MergeFiles(SQLiteConnection connection, string source)
        {
            string[] tables = new string[]
            {
                "challenge",
                "challenge_type",
                "controll_object",
                "location",
                "methodology",
                "product",
                "requirements",
                "staff",
                "test_dynamic",
                "test_static"
            };

            string query = $"ATTACH \"{source}\" AS [temp_db]";
            Utils.ExecuteNonQuery(query, connection);

            foreach (string table in tables)
            {
                query = $"INSERT OR IGNORE INTO [{table}] SELECT * FROM [temp_db].[{table}]";
                Utils.ExecuteNonQuery(query, connection);
            }

            query = $"DETACH [temp_db]";
            Utils.ExecuteNonQuery(query, connection);
        }

        /**
         * Метод для выполнения сторонних процессов с переданными аргументами.
         */
        public static void CmdProccess(string app, string args, bool logConsole = false, bool logLogger = false)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = app,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();

                if (logConsole)
                    Console.WriteLine(line);

                if (logLogger)
                    Logger.Log($"[PROC:{app}] {line}");
            }

            process.WaitForExit();
        }
    }
}
