using System;
using System.Data.SQLite;


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
            string query = $"REPLACE INTO [challenge_type] ([id], [name]) VALUES ({id}, '{name}')";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public static bool Exists(string name)
        {
            string query = $"SELECT * FROM [challenge_type] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }
    }
}
