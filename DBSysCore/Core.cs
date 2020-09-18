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
            
            Logger.Func("Core.InitializeConnection");

            StatusCode result = StatusCode.Ok;
            try
            {
                // Build Connection String
                string fname = Session.sessionData.filename;
                string queryString = $"URI=file:{fname};datetimekind=Utc;";

                bool fileExists = File.Exists(fname);

                if (dumpConnection == null)
                {
                    dumpConnection = new SQLiteConnection(queryString);
                    if (dumpConnection == null)
                        throw new Exception("Could not create connection.");
                }
                
                if (dumpConnection.State != ConnectionState.Open)
                {
                    dumpConnection.Open();
#if DEBUG
                    Console.Write("Core.InitializeConnection: Trying to open... ");
                    if (dumpConnection.State == ConnectionState.Open)
                        Console.WriteLine("Success!");
                    else
                        Console.WriteLine("Fail!");
#endif
                }

                if (!fileExists) // load model.sql
                {
                    string modelString = File.ReadAllText(Paths.dumpModelFilename);
                    Utils.ExecuteNonQuery(modelString, dumpConnection);
                }
            }
            catch (FileNotFoundException e)
            {
                Logger.Error("Core.InitializeConnection", e.ToString());
                result = StatusCode.ConnectionDumpModelNotFound;
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
        private static StatusCode InitializeUserDataConnection()
        {
            
            Logger.Func("Core.InitializeUserDataConnection");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (usersConnection != null && usersConnection.State == ConnectionState.Open)
                    return result;

                // Build Connection String
                string queryString = $"Data Source={Paths.usersFilename}";

                bool fileExists = File.Exists(Paths.usersFilename);

                usersConnection = new SQLiteConnection(queryString);
                usersConnection.Open();

                // if there was no such file initialize it with tables schemas
                if (!fileExists)
                {
                    string modelString = File.ReadAllText(Paths.usersModelFilename);
                    Utils.ExecuteNonQuery(modelString, usersConnection);
                }

                RetrieveAllUsers();
            }
            catch (FileNotFoundException e)
            {
                Logger.Error("Core.InitializeUserDataConnection", e.ToString());
                result = StatusCode.ConnectionUsersModelNotFound;
            }
            catch (Exception e)
            {
                Logger.Error("Core.InitializeUserDataConnection", e.ToString());
                result = StatusCode.Error;
            }

            return result;
        }

        /**
         * Метод для сбора данных обо всех пользователях
         * из всех файлов дампов в [папке с программой?].
         */
        private static void RetrieveAllUsers()
        {
            
            Logger.Func("Core.RetrieveAllUsers");

            string[] dumps = Directory.GetFiles(Paths.dumpsDirectory, "*.db");

#if DEBUG
            Console.WriteLine("Core.RetrieveAllUsers: dump file = " + Session.sessionData.filename);
            Console.WriteLine("Core.RetrieveAllUsers: dumps founded = " + dumps.Length);
#endif

            foreach (string filename in dumps)
            {
                // ERROR: если имя файла совпадает с открытым дампом - могут быть проблемы
                List<Staff> staffList;

#if DEBUG
                Console.WriteLine("Core.RetrieveAllUsers: loading file: " + filename);
#endif

                if (filename == Session.sessionData.filename &&
                    dumpConnection != null &&
                    dumpConnection.State == ConnectionState.Open)
                {
                    // extract all employers data from file
                    staffList = Staff.ExtractAll(dumpConnection);
                }
                else
                {
                    string connstr = $"Data Source={filename}";
                    SQLiteConnection tempConnection = new SQLiteConnection(connstr);

                    // extract all employers data from file
                    tempConnection.Open();
                    staffList = Staff.ExtractAll(tempConnection);
                    tempConnection.Close();
                }

                // and push them into .users database...
                SaveAllStaff(staffList);
            }
        }

        /**
         * Выделяет всех сохранённых пользователей
         * из [пользовательской?] базы данных.
         */
        public static StatusCode GetAllUsers(out List<Staff> staffList)
        {
            
            Logger.Func("Core.GetAllUsers");

            StatusCode result = StatusCode.Ok;
            staffList = new List<Staff>();

            try
            {
                // if (!Session.IsLoggedIn())
                //     result = StatusCode.GrantsUnathorized;
                // else if (!Session.RequireGrants(UserGrants.Admin))
                //     result = StatusCode.GrantsInproper;
                // else if (Session.sessionData.programState != ProgramState.Working)
                //     result = StatusCode.ProgramStateInvalid;
                // else
                if ((result = InitializeConnection()) == StatusCode.Ok &&
                    (result = InitializeUserDataConnection()) == StatusCode.Ok)
                {
                    staffList = Staff.ExtractAll(usersConnection);
                }
            }
            catch(Exception e)
            {
                Logger.Error("Core.GetAllUsers", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        public static StatusCode GetAllChallenges(out List<Challenge> challengeList)
        {
            
            Logger.Func("Core.GetAllChallenges");

            StatusCode result = StatusCode.Ok;
            challengeList = null;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok &&
                    (result = InitializeUserDataConnection()) == StatusCode.Ok)
                {
                    challengeList = Challenge.GetChallenges();
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
            
            Logger.Func("Core.GetModules");

            StatusCode result = StatusCode.Ok;
            modulesList = null;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
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
            
            Logger.Func("Core.GetStaticTests");

            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
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
            
            Logger.Func("Core.GetDynamicTests");

            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
                else if ((result = InitializeConnection()) == StatusCode.Ok && (result = InitializeUserDataConnection()) == StatusCode.Ok)
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
            
            Logger.Func("Core.GetDynamicTests");

            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
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
            
            Logger.Func("Core.GetDynamicTests");

#if DEBUG
            Debug.Assert(beginDate <= endDate, "beginDate must be earlier than endDate");
#endif
            StatusCode result = StatusCode.Ok;
            testsList = null;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
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

        public static StatusCode MapStaticTests(List<TestDynamic> testsDynamicList, out List<TestStatic> testsStaticList)
        {
            
            Logger.Func("Core.MapStaticTests");

            StatusCode result = StatusCode.Ok;
            testsStaticList = new List<TestStatic>();
            try
            {
                if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    foreach (TestDynamic testDynamic in testsDynamicList)
                        testsStaticList.Add(testDynamic.GetTestStatic());
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.MapStaticTests", e.ToString());
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
            
            Logger.Func("Core.SaveAllStaff");

#if DEBUG
            Debug.Assert(usersConnection.State == ConnectionState.Open,
                "users connection must be opened");
#endif
            // bool wasOpened = usersConnection != null &&
            //    usersConnection.State == ConnectionState.Open;
            // if (!wasOpened)
            // {
            //    InitializeUserDataConnection();
            //    //usersConnection.Open();
            // }

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

            // if (!wasOpened)
            // {
            //    usersConnection.Close();
            //    usersConnection = null;
            // }
        }

        /**
         * Закрывает все открытые соединения.
         */
        private static void CloseConnections()
        {
            
            Logger.Func("Core.CloseConnections");

            if (usersConnection != null)
            {
                usersConnection.Close();
                usersConnection = null;
            }

            if (dumpConnection != null)
            {
                dumpConnection.Close();
                dumpConnection = null;
            }
        }

        /**
         * Передает данные состояния программы в строку status.
         */
        public static StatusCode Status(out string status)
        {
            
            Logger.Func("Core.Status");

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

                status += $"  Current working database file: \"{Session.GetCurrentWorkingDumpFileName()}\"\n";

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
            
            Logger.Func("Core.Login");

#if DEBUG
            if (File.Exists(Paths.usersFilename))
            {
                try
                {
                    File.OpenRead(Paths.usersFilename).Close();
                    Console.WriteLine("Core.Login: .users available");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Core.Login: .users FILE IS LOCKED");
                    Console.WriteLine(e.ToString());
                }
            }
            else
                Console.WriteLine("Core.Login: .users not found");
#endif

            StatusCode result = StatusCode.Ok;
            try
            {
                if ((result = InitializeUserDataConnection()) == StatusCode.Ok)
                {
                    if (Session.IsLoggedIn())
                    {
#if DEBUG
                        Console.WriteLine("Core.Login: Already authorized!");
#endif
                        result = StatusCode.LoginAlreadyAuthorized;
                    }
                    else
                        result = Session.Auth(login, password);
                }
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

#if DEBUG
            if (File.Exists(Paths.usersFilename))
            {
                try
                {
                    File.OpenRead(Paths.usersFilename).Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Core.Login: .users FILE IS LOCKED");
                    Console.WriteLine(e.ToString());
                }
            }
            else
                Console.WriteLine("Core.Login: .users not found");

            Console.WriteLine($"Core.Login: usersConnection = {(usersConnection == null ? "null" : usersConnection.State.ToString())}");
            Console.WriteLine($"Error code: {result}");
#endif

            return result;
        }

        /**
         * Производит сброс текущего авторизорованного пользователя.
         */
        public static StatusCode Logout()
        {
            
            Logger.Func("Core.Logout");

            StatusCode result = StatusCode.Ok;
            try
            {
                Session.Open();
                Session.Logout();
            }
            catch(Exception e)
            {
                Logger.Error("Core.Logout", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                Session.Close();
            }

            return result;
        }

        private static StatusCode HasOTKStaff(int id, out bool flag)
        {
            
            Logger.Func("Core.HasOTKStaff");

            StatusCode result = StatusCode.Ok;
            flag = false;

            try
            {
                if ((result = GetAllUsers(out List<Staff> staffList)) == StatusCode.Ok)
                    flag = staffList.Exists(staff => staff.id == id);
            }
            catch (Exception e)
            {
                Logger.Error("Core.HasOTKStaff", e.ToString());
                result = StatusCode.Error;
            }

            return result;
        }

        /**
         * Иниализирует новое испытание.
         */
        public static StatusCode BeginChallenge(string productName, string coName,
            string coSerialNumber, string coDecNumber, int staffOTKId, string coParent,
            string challengeTypeName, string locationName, string description = "")
        {
            Logger.Func("Core.BeginChallenge");

            StatusCode result = StatusCode.Ok;

            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.IsLoggedInAsVirtualAdmin())
                    result = StatusCode.GrantsVirtualAdminNotAllowed;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
                else if ((result = HasOTKStaff(staffOTKId, out bool flag)) == StatusCode.Ok)
                {
                    if (!flag)
                        result = StatusCode.BeginChallengeInvalidOTKStaffId;
                    else if ((result = InitializeUserDataConnection()) == StatusCode.Ok)
                    {
                        // open new challenge
                        ControllObject controllObject = new ControllObject()
                        {
                            name = coName,
                            serialNumber = coSerialNumber,
                            decimalNumber = coDecNumber,
                            version = "null",
                            parent = coParent,
                            product = productName
                        };
                        controllObject.GenerateId();

                        ChallengeType challengeType = new ChallengeType() { name = challengeTypeName };
                        challengeType.GenerateId();

                        Staff staff = Session.sessionData.staff;
                        Staff staffOTK = new Staff(staffOTKId);

                        Location location = new Location() { name = locationName };
                        location.GenerateId();

                        Session.Open();
                        Session.sessionData.programState = ProgramState.Testing;
                        Session.sessionData.activeChallenge = new Challenge()
                        {
                            controllObject = controllObject,
                            challengeType = challengeType,
                            staff = staff,
                            staffOTK = staffOTK,
                            location = location,
                            beginTime = DateTime.Now,
                            description = description
                        };

                        Session.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.BeginChallenge", e.ToString());
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
         * Завершает и сохраняет текущее испытание.
         */
        public static StatusCode EndChallenge()
        {
            
            Logger.Func("Core.EndChallenge");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Testing)
                    result = StatusCode.ProgramStateInvalid;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    // save active challenge
                    Session.sessionData.programState = ProgramState.Working;
                    Debug.Assert(Session.sessionData.activeChallenge != null);
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
            
            Logger.Func("Core.Test");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Testing)
                {
#if DEBUG
                    Console.WriteLine($"Core.Test: Program state = {Session.sessionData.programState}");
#endif
                    result = StatusCode.ProgramStateInvalid;
                }
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
#if DEBUG
                    Console.WriteLine("Core.Test: Nominal test added!");
#endif
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.BeginTest", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

#if DEBUG
            Session.Close();
#endif

            return result;
        }

        public static StatusCode Test(string tsIndex, bool status)
        {
            
            Logger.Func("Core.Test");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Tester))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Testing)
                {
#if DEBUG
                    Console.WriteLine($"Core.Test: state = {Session.sessionData.programState}");
#endif
                    result = StatusCode.ProgramStateInvalid;
                }
                else
                {
                    TestDynamic test = new TestDynamic()
                    {
                        tsIndex = tsIndex,
                        challenge = Session.sessionData.activeChallenge,
                        beginTime = DateTime.Now,
                        nominal = 0, // null,
                        actualValue = 0, // null,
                        delta = 0, // null,
                        boundaryValue = 0, // null,
                        status = status
                    };
                    test.GenerateId();

                    // add constructed test to test list
                    Session.sessionData.activeTests.Add(test);
#if DEBUG
                    Console.WriteLine("Core.Test: Status test added!");
#endif
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.BeginTest", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                CloseConnections();
                Session.Close();
            }

#if DEBUG
            Session.Close();
#endif

            return result;
        }

        /**
         * Загружает статические данные в текущий файл дампа.
         */
        public static StatusCode LoadStaticTests()
        {
            
            Logger.Func("Core.LoadStaticTests");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
                else if (!File.Exists(Paths.staticDataTable))
                    result = StatusCode.LoadStaticTestsStaticTableNotFound;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    FileStream stream = File.Open(Paths.staticDataTable, FileMode.Open, FileAccess.Read);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

                    DataTableCollection tables = reader.AsDataSet().Tables;

                    DataRowCollection rows;

                    // Console.WriteLine($"Core.LoadStaticTests: conn = {dumpConnection}");

                    // clear all data first
                    _ExecSQL("DELETE FROM [methodology] WHERE 1");
                    _ExecSQL("DELETE FROM [requirements] WHERE 1");
                    _ExecSQL("DELETE FROM [module] WHERE 1");
                    _ExecSQL("DELETE FROM [test_static] WHERE 1");

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

#if DEBUG
                    // show modules
                    foreach (var m in modules)
                        Console.WriteLine($"Core.LoadStaticTests: module loaded: {m.id} - {m.name}");
#endif

                    // load static test here
                    rows = tables["Protocol"].Rows;

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

                    // WARNING: may be it is important to dispose manually, who knows...
                    // reader.Dispose();
                    reader.Close();
                    // stream.Dispose();
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
                CloseConnections();
                Session.Close();
            }

            return result;
        }

        /**
         * Метод для смены рабочего файла дампа.
         */
        public static StatusCode DumpUse(string filepath)
        {
            Logger.Func("Core.DumpUse");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                // else if (filepath.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                //    result = StatusCode.DumpUseInvalidFilename;
                else
                {
                    /*
                    if (!File.Exists($"{filename}"))
                        // create new one from scratch...
                        CmdProccess("sqlite3.exe", $"{filename} \".read model.sql\"");
                    */

                    CloseConnections();
                    Session.SwitchDumpFile(filepath);
                    result = InitializeConnection();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.DumpUse", e.ToString());
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
         * Метод для объединения нескольких файлов дампов в один.
         */
        public static StatusCode DumpMerge(string dest, string[] src)
        {
            
            Logger.Func("Core.DumpMerge");

            StatusCode result = StatusCode.Ok;
            try
            {
                // filter out invalid filenames
                src = src.Where(filename => File.Exists(filename)).ToArray();

                if (src.Length == 0)
                    result = StatusCode.DumpMergeNoFiles;
                else if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                // WARNING: are we really need to open dump connection here ?
                else // if ((result = InitializeConnection()) == StatusCode.Ok)
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
                CloseConnections();
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
            
            Logger.Func("Core.GenerateReport");

            StatusCode result = StatusCode.Ok;
            resultFileName = "";
            try
            {
                // WARNING: dont really need to close them here...
                // CloseConnections();
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Operator))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load($"{Paths.reportTemplateDirectory}\\universal_template.html");

                    // change encoding here!
                    doc.LoadHtml(Utils.StdEncToUTF8(doc.DocumentNode.OuterHtml));

                    // doc.DocumentNode.SelectNodes("//*[@name='pnum']").First().InnerHtml = "1000"; // reportNumber;

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//*[@id='module']"))
                        node.InnerHtml = challenge.controllObject.name;

                    foreach(HtmlNode node in doc.DocumentNode.SelectNodes("//*[@id='serial']"))
                        node.InnerHtml = challenge.controllObject.serialNumber;

                    doc.DocumentNode.SelectNodes("//*[@id='product']").First().InnerHtml = challenge.controllObject.product;
                    doc.DocumentNode.SelectNodes("//*[@id='operator']").First().InnerHtml =
                        $"{challenge.staff.surname} {challenge.staff.firstName[0]}. {challenge.staff.patronymicName[0]}.";
                    doc.DocumentNode.SelectNodes("//*[@id='OTK']").First().InnerHtml =
                        $"{challenge.staffOTK.surname} {challenge.staffOTK.firstName[0]}. {challenge.staffOTK.patronymicName[0]}.";

                    HtmlNode table = doc.DocumentNode.SelectNodes("//table")[0];

                    bool challengeFailed = false;

                    foreach (TestDynamic testDynamic in TestDynamic.GetTests(challenge))
                    {
                        TestStatic testStatic = testDynamic.GetTestStatic();

                        challengeFailed = challengeFailed || !testDynamic.status;

                        HtmlNode tr = HtmlNode.CreateNode("<tr></tr>");
                        HtmlNode td;

                        td = HtmlNode.CreateNode($"<td>{testStatic.description}</td>");
                        tr.AppendChild(td);

                        if (testStatic.unit == "null")
                            td = HtmlNode.CreateNode($"<td></td>");
                        else
                            td = HtmlNode.CreateNode($"<td>{testStatic.unit}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testStatic.requirements.docNumber}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testStatic.methodology.docNumber}</td>");
                        tr.AppendChild(td);

                        if (testStatic.unit == "null")
                            td = HtmlNode.CreateNode("<td></td>");
                        else
                            td = HtmlNode.CreateNode($"<td>{testDynamic.nominal:0.##}</td>");
                        tr.AppendChild(td);

                        if (testStatic.unit == "null")
                            td = HtmlNode.CreateNode("<td></td>");
                        else
                            td = HtmlNode.CreateNode($"<td>{testDynamic.delta:0.##}</td>");
                        tr.AppendChild(td);

                        if (testStatic.unit == "null")
                            td = HtmlNode.CreateNode("<td></td>");
                        else
                            td = HtmlNode.CreateNode($"<td>{testDynamic.boundaryValue:0.##}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{testDynamic.actualValue:0.##}</td>");
                        tr.AppendChild(td);

                        td = HtmlNode.CreateNode($"<td>{challenge.beginTime:dd/MM/yyyy}</td>");
                        tr.AppendChild(td);

                        tr.AppendChild(HtmlNode.CreateNode("<td></td>"));

                        if (testDynamic.status == true)
                            tr.AppendChild(HtmlNode.CreateNode("<td style=\"color:green\">Годен</td>"));
                        else
                            tr.AppendChild(HtmlNode.CreateNode("<td style=\"color:red\">Не годен</td>"));

                        table.AppendChild(tr);
                    }

                    if (challengeFailed)
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//*[@id='failed']"))
                            node.InnerHtml = "не";

                    string htmlString = doc.DocumentNode.OuterHtml;

                    string reportFileName = GenerateReportFileName(challenge);
                    resultFileName = $"{Paths.reportDirectory}\\{reportFileName}";

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
            
            Logger.Func("Core.AddStaff");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    // raplce password with corresponding hash
                    password = Utils.GetHash(password);

                    // check if staff exists
                    Staff staff = Staff.Get(surname, firstName, patronymicName);
                    if (staff != null)
                    {
                        // update if exists
                        staff.post = postName;
                        staff.department = departmentName;
                        staff.login = login;
                        staff.password = password;
                        staff.SaveData(dumpConnection);
#if DEBUG
                        Console.WriteLine($"Core.AddStaff: staff updated! {dumpConnection.DataSource}");
#endif
                    }
                    else
                    {
                        // create new staff
                        staff = new Staff(surname, firstName, patronymicName,
                            postName, departmentName, login, password);
#if DEBUG
                        Console.WriteLine("Core.AddStaff: staff created!");
#endif
                    }
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
            
            Logger.Func("Core.UpdateStaff");

            StatusCode result = StatusCode.Ok;
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                else if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else if (Session.sessionData.programState != ProgramState.Working)
                    result = StatusCode.ProgramStateInvalid;
                else if ((result = InitializeUserDataConnection()) == StatusCode.Ok)
                {
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

        private static void _ExecSQL(string query)
        {
            
            Logger.Func("Core._ExecSQL");

#if DEBUG
            Debug.Assert(dumpConnection.State == ConnectionState.Open,
                "dump connection must be opened");
#endif
            SQLiteCommand cmd = new SQLiteCommand(query, dumpConnection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static StatusCode ExecSQL(string query, out string output)
        {
            Logger.Func("Core.ExecSQL");

            StatusCode result = StatusCode.Ok;
            SQLiteDataReader reader = null;
            output = "";
            try
            {
                if (!Session.IsLoggedIn())
                    result = StatusCode.GrantsUnathorized;
                if (!Session.RequireGrants(UserGrants.Admin))
                    result = StatusCode.GrantsInproper;
                else if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    reader = Utils.ExecuteReader(query, dumpConnection);

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            object data = reader.GetValue(i);
                            output += data.ToString() + "|";
                        }

                        output += "\n";
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.ExecSQL", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
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
            Logger.Func("Core.MergeFiles");

#if DEBUG
            Debug.Assert(connection.State == ConnectionState.Open,
                "connection must be opened");
#endif

            string[] tables = new string[]
            {
                "requirements",
                "methodology",
                "module",
                "test_static",
                "location",
                "staff",
                "challenge_type",
                "controll_object",
                "challenge",
                "test_dynamic"
            };

            string query = $"ATTACH \"{source}\" AS [temp_db]";
            Utils.ExecuteNonQuery(query, connection);

            foreach (string table in tables)
            {
                query = $"INSERT OR IGNORE INTO [{table}]" +
                    $"SELECT * FROM [temp_db].[{table}]";
                Utils.ExecuteNonQuery(query, connection);
            }

            query = $"DETACH [temp_db]";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static StatusCode GetFPGAVersion(Challenge challenge, out string version)
        {
            StatusCode result = StatusCode.Ok;
            Logger.Func("Core.GetFPGAVersion");
            SQLiteDataReader reader = null;
            version = "";

            try
            {
                if ((result = InitializeConnection()) == StatusCode.Ok)
                {
                    string query = $"SELECT [actual_value] FROM [test_dynamic] WHERE [ts_index] = " +
                        $"(SELECT [ts_index] FROM [test_static] WHERE instr([description], 'ПЛИС') > 0 " +
                        $"AND instr([description], 'верси') > 0) AND [challenge] = {challenge.id}";
                    reader = Utils.ExecuteReader(query, dumpConnection);

                    if (reader.Read())
                        version = Convert.ToString((decimal)reader[0]);
                    else
                    {
                        version = "-";
                        result = StatusCode.NoFPGAVersionFound;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Core.GetFPGAVersion", e.ToString());
                result = StatusCode.Error;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                CloseConnections();
                Session.Close();
            }

            return result;
        }
    }
}
