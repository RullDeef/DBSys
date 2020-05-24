// #define DEBUG_STAFF

using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DBSysCore.Model
{
    [Serializable]
    public class Staff
    {
        public int id;
        public string surname;
        public string firstName;
        public string patronymicName;
        public string post;
        public string department;
        public string login;
        public string password;

        public Staff() { }

        public Staff(string surname, string firstName, string patronymicName, string post, string department, string login, string password)
        {
            this.surname = surname;
            this.firstName = firstName;
            this.patronymicName = patronymicName;
            this.post = post;
            this.department = department;
            this.login = login;
            this.password = password;

            if (this.password.Length != 64)
                this.password = Utils.GetHash(this.password);

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public Staff(int id)
        {
            string query = "SELECT [id], [surname], [first_name], [patronymic_name], "
                + $"[post], [department], [login], [password] FROM [staff] WHERE [id] = {id}";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            if (!reader.Read())
            {
                reader.Close();
                reader = Utils.ExecuteReader(query, Core.usersConnection);
                reader.Read();
            }

            this.id = id;
            surname = (string)reader[1];
            firstName = (string)reader[2];
            patronymicName = (string)reader[3];
            post = (string)reader[4];
            department = (string)reader[5];
            login = (string)reader[6];
            password = (string)reader[7];

            reader.Close();
        }

        public int GenerateId()
        {
            id = Utils.Hash(surname + firstName + patronymicName);
            return id;
        }

        public void SaveData(SQLiteConnection connection)
        {
            string query = "REPLACE INTO [staff] ([id], [surname], [first_name], [patronymic_name], [post], [department], [login], [password]) "
                + $"VALUES ({id}, '{surname}', '{firstName}', '{patronymicName}', '{post}', '{department}', '{login}', '{password}')";
            Utils.ExecuteNonQuery(query, connection);
#if DEBUG_STAFF
            Console.WriteLine($"Staff.SaveData: {surname} saved into {connection.DataSource}");
#endif
        }

        public static bool Exists(string surname, string firstName, string patronymicName)
        {
            string query = $"SELECT * FROM [staff] WHERE [surname] = '{surname}' AND [first_name] = '{firstName}' "
                + $"AND [patronymic_name] = '{patronymicName}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }

        public static List<Staff> ExtractAll(SQLiteConnection connection)
        {
            List<Staff> result = new List<Staff>();

#if DEBUG_STAFF
            Console.WriteLine("Staff.ExtractAll: Preparing for extraction");
            Console.WriteLine($"Staff.ExtractAll: connection: {connection.DataSource}");
#endif

            string query = $"SELECT [id], [surname], [first_name], [patronymic_name], [post], [department], [login], [password] FROM [staff]";
            SQLiteDataReader reader = Utils.ExecuteReader(query, connection);

            while (reader.Read())
            {
                result.Add(new Staff
                {
                    id = (int)reader[0],
                    surname = (string)reader[1],
                    firstName = (string)reader[2],
                    patronymicName = (string)reader[3],
                    post = (string)reader[4],
                    department = (string)reader[5],
                    login = (string)reader[6],
                    password = (string)reader[7]
                });
            }

            reader.Close();
            return result;
        }

        public override string ToString()
        {
            return $"{surname} {firstName[0]}.";
        }
    }
}
