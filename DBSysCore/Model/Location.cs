using System;
using System.Data.SQLite;
using System.Diagnostics;

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
#if DEBUG
            Debug.Assert(Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

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
#if DEBUG
            Debug.Assert(connection.State == System.Data.ConnectionState.Open,
                "connection must be opened");
#endif

            string query = $"REPLACE INTO [location] ([id], [name]) VALUES ({id}, '{name}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static bool Exists(string name)
        {
#if DEBUG
            Debug.Assert(Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

            string query = $"SELECT * FROM [location] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }
    }
}
