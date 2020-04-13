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
        Testing = 2
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
         * Название файла сессии.
         */
        private const string filename = ".session";

        /**
         * Данные текущей сессии.
         */
        public static SessionData sessionData;

        private static bool opened = false;

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
        public static void Open()
        {
            // if there is no session file - begin clear session
            if (File.Exists(filename))
            {
                try
                {
                    Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    sessionData = (SessionData)new BinaryFormatter().Deserialize(stream);
                    stream.Dispose();
                    stream.Close();
                    opened = true;
                }
                catch
                {
                    sessionData = GenerateDefaultSessionData();
                }
            }
            else
            {
                sessionData = GenerateDefaultSessionData();
            }


            // initialize new dump file from model.sql definition if there is no such
            if (!File.Exists(sessionData.filename))
                Core.CmdProccess("sqlite3.exe", $"{sessionData.filename} \".read model.sql\"", false);
        }

        private static void EnsureOpen()
        {
            if (!opened)
            {
                Open();
                if (!opened)
                    Console.WriteLine("Session file is still not opened!!");
            }
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
            EnsureOpen();

            if (sessionData.staff == null)
                return "unauthorized";
            else
            {
                Staff person = sessionData.staff;
                return $"{person.surname} {person.firstName.Substring(0, 1)}.";
            }
        }

        /**
         * Возвращает данные сессии по-умолчанию:
         * 
         *      - пользователь не авторизован
         *      - состояние программы - авторизация
         *      - файл дампа - "dump.db"
         */
        public static SessionData GenerateDefaultSessionData()
        {
            return new SessionData
            {
                staff = null,
                programState = ProgramState.Idle,
                filename = "dump.db",
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
            EnsureOpen();

            return sessionData.staff != null;
        }

        /**
         * Возвращает объект UserGrants с правами текущего
         * авторизированного пользователя.
         */
        public static UserGrants GetGrants()
        {
            EnsureOpen();

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
        public static string GrantsString(UserGrants grants)
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
            EnsureOpen();

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
        public static StatusCode Auth(string login, string password)
        {
            Staff staff;

            EnsureOpen();

            // WARNING: may be unsafe with some types of commands

            // make virtual admin with login admin and password dsplab
            if (login == "admin" && password == "dsplab")
            {
                Console.WriteLine("Initialized virtual admin!");
                Console.WriteLine("Please, use it only for adding other users!");

                staff = new Staff()
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
            }
            else // find real user by login
            {
                List<Staff> staffList = Staff.ExtractAll(Core.usersConnection);

                if (staffList.Count == 0)
                    return StatusCode.LoginNoRegisteredUsers;

                if (staffList.Where(s => s.login == login).Count() == 0)
                    return StatusCode.LoginInvalidLogin;

                staff = staffList.Where(s => s.login == login).First();

                if (!Utils.VerifyHash(password, staff.password))
                    return StatusCode.LoginInvalidPass;
            }

            // ok.
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
        public static void Logout()
        {
            sessionData.staff = null;
            sessionData.programState = ProgramState.Idle;
            sessionData.activeChallenge = null;
            sessionData.activeTests.Clear();
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
        public static StatusCode SaveCurrentChallenge(SQLiteConnection connection)
        {
            EnsureOpen();

            StatusCode result = StatusCode.Ok;
            try
            {
                // end current challenge and write to data dump
                DateTime endTime = DateTime.Now;

                Challenge challenge = sessionData.activeChallenge;
                challenge.endTime = endTime;

                for (int i = 0; i < sessionData.activeTests.Count; i++)
                {
                    Console.WriteLine(i);

                    TestDynamic test = sessionData.activeTests[i];

                    if (i + 1 == sessionData.activeTests.Count)
                        test.endTime = endTime;
                    else
                        test.endTime = sessionData.activeTests[i + 1].beginTime;

                    test.SaveData(connection);
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
         * 
         * Дополнительно закрывает Logger.
         */
        public static void Close()
        {
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            new BinaryFormatter().Serialize(stream, sessionData);
            stream.Dispose();
            stream.Close();

            opened = false;

            Logger.Close();
        }
    }
}
