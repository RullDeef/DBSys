using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace DBSysCore.Model
{
    [Serializable]
    public class ChallengeType
    {
        public int id;
        public string name;

        public ChallengeType() { }

        public ChallengeType(string name)
        {
            this.name = name;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public ChallengeType(int id)
        {
#if DEBUG
            Debug.Assert(Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

            string query = $"SELECT [id], [name] FROM [challenge_type] WHERE [id] = '{id}'";
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

            string query = $"REPLACE INTO [challenge_type] ([id], [name]) VALUES ({id}, '{name}')";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static bool Exists(string name)
        {
#if DEBUG
            Debug.Assert(Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

            string query = $"SELECT * FROM [challenge_type] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }
    }
}
