using System;
using System.Data.SQLite;


namespace DBSysCore.Model
{
    [Serializable]
    public class Location
    {
        public int id;
        public string name;

        public Location() { }

        public Location(string name)
        {
            this.name = name;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public Location(int id)
        {
            string query = $"SELECT [id], [name] FROM [location] WHERE [id] = {id}";
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
            string query = $"REPLACE INTO [location] ([id], [name]) VALUES ({id}, '{name}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static bool Exists(string name)
        {
            string query = $"SELECT * FROM [location] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }
    }
}
