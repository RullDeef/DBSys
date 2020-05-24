using System;
using System.Data.SQLite;


namespace DBSysCore.Model
{
    [Serializable]
    public class Methodology
    {
        public int id;
        public string name;
        public string docNumber;

        public Methodology() { }

        public Methodology(int id)
        {
            string query = $"SELECT [id], [name], [doc_number] FROM [methodology] WHERE [id] = '{id}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            this.id = id;
            name = (string)reader[1];
            docNumber = (string)reader[2];

            reader.Close();
        }

        public void SaveData(SQLiteConnection connection)
        {
            string query = $"REPLACE INTO [methodology] ([id], [name], [doc_number]) VALUES ({id}, '{name}', '{docNumber}')";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static bool Exists(string name)
        {
            string query = $"SELECT * FROM [methodology] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }
    }
}
