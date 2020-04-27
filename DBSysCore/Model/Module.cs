using System;
using System.Collections.Generic;
using System.Data.SQLite;


namespace DBSysCore.Model
{
    [Serializable]
    public class Module
    {
        public int id;
        public string name;

        public Module() { }

        public Module(string name)
        {
            this.name = name;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public Module(int id)
        {
            string query = $"SELECT [id], [name] FROM [module] WHERE [id] = '{id}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            this.id = id;
            name = (string)reader[1];

            reader.Close();
        }

        public int GenerateId()
        {
            id = Utils.Hash(name);
            return id;
        }

        public void SaveData(SQLiteConnection connection)
        {
            string query = $"INSERT OR IGNORE INTO [module] ([id], [name]) VALUES ({id}, '{name}')";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public static bool Exists(string name)
        {
            string query = $"SELECT * FROM [module] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }

        public static List<Module> GetModules()
        {
            string query = $"SELECT [id], [name] FROM [module]";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            List<Module> result = new List<Module>();

            while (reader.Read())
            {
                Module module = new Module
                {
                    id = (int)reader[0],
                    name = (string)reader[1]
                };
                result.Add(module);
            }

            reader.Close();
            return result;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
