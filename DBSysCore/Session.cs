using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using DBSysCore.Model;


namespace DBSysCore
{
    [Serializable]
    /**
     * Перечисление возможных состояний программы.
     */
    public enum ProgramState
    {
        Idle = 0,
        Working = 1,
        Testing = 2 // TODO: redundant third state
    }

    [Serializable]
    /**
     * Структура данных текущей сессии.
     */
    public struct SessionData
    {
        /**
         * Текущий пользователь.
         */
        public Staff staff;

        /**
         * Текущее состояние программы.
         */
        public ProgramState programState;

        /**
         * Название текущего файла дампа.
         */
        public string filename;

        /**
         * Текущая последовательность тестов.
         */
        public Challenge activeChallenge;

        /**
         * Список произведённых тестов.
         */
        public List<TestDynamic> activeTests;
    }

    /**
     * Перечисление прав пользователей. От неавторизованного
     * до администратора.
     */
    public enum UserGrants
    {
        None,
        Tester,
        Operator,
        Admin
    }

    public static class Session
    {
        /**
         * Данные текущей сессии.
         */
        public static SessionData sessionData; // TODO: combine with current class

        private static bool opened = false; // TODO: redundant variable

        public static Staff virtualAdmin = new Staff() // TODO: move to Staff class as static const one
        {
            id = 461672399,
            surname = "DSPLAB",
            firstName = "Admin",
            patronymicName = "",
            post = "admin",
            department = "",
            login = "admin",
            password = "dsplab"
        };

        /**
         * Считывает данные последней сессии из файла сессии.
         * 
         * Если файл сессии не существует, то будут использованы данные
         * сессии по-умолчанию:
         * 
         *      - пользователь не авторизован
         *      - состояние программы - авторизация
         *      - файл дампа - "dump.db"
         * 
         */
        public static void Open() // TODO: make private and put in constructor. (with Close() of course)
        {
            if (opened)
            {
#if DEBUG
                Console.WriteLine("Session.Open: Session is already opened");
#endif
                return;
            }

#if DEBUG
            Console.WriteLine("Session.Open: Openning sesison...");
#endif

            // if there is no session file - begin clear session
            if (File.Exists(Paths.sessionFilename))
            {
                Stream stream = null;
                try
                {
#if DEBUG
                    Console.WriteLine("Session.Open: Trying to read from existing file...");
# endif
                    stream = new FileStream(Paths.sessionFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    sessionData = (SessionData)new BinaryFormatter().Deserialize(stream);
                }
                catch
                {
#if DEBUG
                    Console.WriteLine("Session.Open: Failed! Using Default data");
# endif
                    sessionData = GenerateDefaultSessionData();
                }
                finally
                {
#if DEBUG
                    Console.WriteLine("Session.Open: Succeed!");
# endif
                    if (stream != null)
                    {
                        // WARNING: dispose commented
                        // stream.Dispose();
                        stream.Close();
                    }
                }
            }
            else
            {
                // FileStream stream = File.Create(filename);
                // stream.Dispose();
                // stream.Close();
#if DEBUG
                Console.WriteLine("Session.Open: No session file found. Use default data");
# endif
                sessionData = GenerateDefaultSessionData();
            }

            opened = true;

            // initialize new dump file from model.sql definition if there is no such
            // if (!File.Exists(sessionData.filename))
            //    Core.CmdProccess("sqlite3.exe", $"{sessionData.filename} \".read model.sql\"", false);
        }

        public static string GetCurrentWorkingDumpFileName() // TODO: delegate to sessionData struct
        {
            Open();

            string fname = sessionData.filename;
            int i = fname.LastIndexOf('\\');
            fname = fname.Substring(i + 1, fname.Length - i - 4);

            return fname;
        }

        public static string GetCurrentWorkingDumpFileNameWithPath()
        {
            Open();

            return sessionData.filename;
        }

        /**
         * Возвращает форматированное имя текущего пользователя, или
         * "unatorized", если пользователь не авторизован.
         * 
         * Формат вывода:
         *      Фамилия И.
         */
        public static string GetUserName()
        {
            string userName = "unauthorized";

            Open();

            if (sessionData.staff != null)
            {
                Staff person = sessionData.staff;
                userName = $"{person.surname} {person.firstName.Substring(0, 1)}."; // TODO: move to ToString method of class Staff
            }

            return userName;
        }

        /**
         * Возвращает данные сессии по-умолчанию:
         * 
         *      - пользователь не авторизован
         *      - состояние программы - авторизация
         *      - файл дампа - "dump.db"
         */
        public static SessionData GenerateDefaultSessionData() // TODO: make private. Use whenever session file could not be found
        {
            return new SessionData
            {
                staff = null,
                programState = ProgramState.Idle,
                filename = $"{Paths.dumpsDirectory}\\dump.db",
                activeChallenge = null,
                activeTests = new List<TestDynamic>()
            };
        }

        /**
         * Возвращает true, если данные последней сессии
         * содержат информацию о пользователе.
         */
        public static bool IsLoggedIn()
        {
            Open();

            return sessionData.staff != null;
        }

        public static bool IsLoggedInAsVirtualAdmin()
        {
            Open();

            return sessionData.staff.id == virtualAdmin.id;
        }

        /**
         * Возвращает объект UserGrants с правами текущего
         * авторизированного пользователя.
         */
        public static UserGrants GetGrants()
        {
            Open();

            if (sessionData.staff == null)
                return UserGrants.None;

            string post = sessionData.staff.post;

            if (post == "tester")
                return UserGrants.Tester;
            if (post == "operator")
                return UserGrants.Operator;
            if (post == "admin")
                return UserGrants.Admin;

            return UserGrants.None;
        }

        /**
         * Возвращает строку-представление переданного объекта прав.
         * 
         *      "admin" | "operator" | "tester" | "unauthorized"
         */
        public static string GrantsString(UserGrants grants) // TODO: move in UserGrants class as ToString override
        {
            string result = "unauthorized";
            switch (grants)
            {
                case UserGrants.None:
                    result = "unauthorized";
                    break;

                case UserGrants.Tester:
                    result = "tester";
                    break;

                case UserGrants.Operator:
                    result = "operator";
                    break;

                case UserGrants.Admin:
                    result = "admin";
                    break;
            };

            return result;
        }

        /**
         * Возвращает true, если права текущего пользователя
         * включают в себя переданные права.
         */
        public static bool RequireGrants(UserGrants grants)
        {
            Open();

            if (!IsLoggedIn())
            {
                Console.WriteLine("You need to authorize first to run this command.");
                return false;
            }
            
            if (GetGrants() < grants)
            {
                Console.WriteLine($"You must have {GrantsString(grants)} grants to be able to run this command!");
                return false;
            }

            return true;
        }

        /**
         * Авторизирует пользователя по переданным логину и паролю.
         * Возвращает 0, если авторизация прошла успешно.
         * 
         * Использует данные для входа из файла пользователей.
         * 
         * Гарантируется, что файл пользовательских данных будет создан
         * до вызова данного метода.
         * 
         * Сохраняет инофрмацию о текущем аутентифицированном пользователе
         * в данных текущей сессии.
         * 
         * Переводит текущее состояние программы в рабочее.
         */
        public static StatusCode Auth(string login, string password) // TODO: use params in constructor
        {
            Logger Logger = new Logger();
            Logger.Func("Session.Auth");

            Staff staff;

            Open();

#if DEBUG
            Console.WriteLine($"Session.Auth: passed \"{login}\" \"{password}\"");
# endif

            // WARNING: may be unsafe with some types of commands

            // make virtual admin with login admin and password dsplab
            if (login == "admin" && password == "dsplab")
            {
                Console.WriteLine("Initialized virtual admin!");
                Console.WriteLine("Please, use it only for adding other users!");

                staff = virtualAdmin;
                /*
                new Staff()
                {
                    surname = "DSPLAB",
                    firstName = "Admin",
                    patronymicName = "",
                    post = "admin",
                    department = "",
                    login = login,
                    password = password
                };
                staff.GenerateId();
                */
            }
            else // find real user by login
            {
#if DEBUG
                Console.WriteLine("Session.Auth: Searching for real user...");
# endif
                List<Staff> staffList = Staff.ExtractAll(Core.usersConnection);

                if (staffList.Count == 0)
                {
#if DEBUG
                    Console.WriteLine("Session.Auth: no registered users!");
# endif
                    return StatusCode.LoginNoRegisteredUsers;
                }

                if (staffList.Where(s => s.login == login).Count() == 0)
                {
#if DEBUG
                    Console.WriteLine("Session.Auth: invalid login!");
# endif
                    return StatusCode.LoginInvalidLogin;
                }

                staff = staffList.Where(s => s.login == login).First();

                if (!Utils.VerifyHash(password, staff.password))
                {
#if DEBUG
                    Console.WriteLine("Session.Auth: Invalid password!");
# endif
                    return StatusCode.LoginInvalidPass;
                }
            }

            // ok.
#if DEBUG
            Console.WriteLine("Session.Auth: Success!");
# endif
            sessionData.staff = staff;
            sessionData.programState = ProgramState.Working;
            return StatusCode.Ok;
        }

        /**
         * Сбрасывает данные текущей сессии. В том числе информацию о текущем
         * пользователе, активной последовательности тестов и о самих тестах.
         * 
         * Переводит текущее состояние программы в ожидание аутентификации.
         */
        public static void Logout() // use every time on exit. Move in destructor ("unauthorized" is useless state anymore)
        {
            Logger Logger = new Logger();
            Logger.Func("Session.Logout");

            sessionData.staff = null;
            sessionData.programState = ProgramState.Idle;
            sessionData.activeChallenge = null;
            sessionData.activeTests.Clear();
#if DEBUG
            Console.WriteLine("Session.Logout: state cleared!");
# endif
        }

        public static void SwitchDumpFile(string filename)
        {
            Logger.Func("Session.SwitchDumpFile");

            sessionData.filename = filename;
        }

        /**
         * Сохраняет текущую последовательность тестов по переданному соединению.
         * При этом данные текущей последовательности тестов сбрасываются в данных
         * текущей сессии.
         * 
         * Переданное соединение должно быть открыто.
         * 
         * Возвращает false, если запись прошла успешно.
         */
        public static StatusCode SaveCurrentChallenge(SQLiteConnection connection) // TODO: move all "connection" variables inside session class
        {
            Logger Logger = new Logger(); // TODO: initialize logger on session creation
            Logger.Func("Session.SaveCurrentChallege");

            Open();

            StatusCode result = StatusCode.Ok;
            try
            {
                // end current challenge and write to data dump
                DateTime endTime = DateTime.Now;

                Challenge challenge = sessionData.activeChallenge;
                challenge.endTime = endTime;

                for (int i = 0; i < sessionData.activeTests.Count; i++) // TODO: refactor as funtion inside Challange class
                {
                    TestDynamic test = sessionData.activeTests[i];

                    if (i + 1 == sessionData.activeTests.Count)
                        test.endTime = endTime;
                    else
                        test.endTime = sessionData.activeTests[i + 1].beginTime;

                    test.SaveData(connection);
#if DEBUG
                    Console.WriteLine($"Session.SaveCurrentChallenge: saved test #{i} with id = {test.id}");
# endif
                }

                challenge.staff.SaveData(connection);
                challenge.SaveData(connection);

                // revert active tests and active challenge
                sessionData.activeTests.Clear();
                sessionData.activeChallenge = null;
            }
            catch (Exception e)
            {
                Logger.Error("Session.SaveCurrentChallenge", e.ToString());
                result = StatusCode.Error;
            }
            
            return result;
        }

        /**
         * Сохраняет данные текущей сессии в файл данных последней сессии.
         */
        public static void Close() // TODO: make private. Use only in destructor
        {
            if (opened)
            {
                Stream stream = new FileStream(Paths.sessionFilename, FileMode.Create, FileAccess.Write, FileShare.None);
                new BinaryFormatter().Serialize(stream, sessionData);
                stream.Dispose();
                stream.Close();

                opened = false;
            }
        }
    }
}
