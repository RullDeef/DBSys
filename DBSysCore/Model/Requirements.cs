using System;
using System.Data.SQLite;

namespace DBSysCore.Model
{
    [Serializable]
    public class Requirements
    {
        public int id;
        public string name;
        public string docNumber;

        public Requirements() { }

        public Requirements(string name, string docNumber)
        {
            this.name = name;
            this.docNumber = docNumber;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public Requirements(int id)
        {
            string query = $"SELECT [id], [name], [doc_number] FROM [requirements] WHERE [id] = '{id}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            this.id = id;
            name = (string)reader[1];
            docNumber = (string)reader[2];

            reader.Close();
        }

        public int GenerateId()
        {
            id = Utils.Hash(name + docNumber);
            return id;
        }

        public void SaveData(SQLiteConnection connection)
        {
            string query = "REPLACE INTO [requirements] ([id], [name], " +
                $"[doc_number]) VALUES ({id}, '{name}', '{docNumber}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static bool Exists(string name)
        {
            string query = $"SELECT * FROM [requirements] WHERE [name] = '{name}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }
    }
}
