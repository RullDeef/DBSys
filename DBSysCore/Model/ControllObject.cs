using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace DBSysCore.Model
{
    [Serializable]
    public class ControllObject
    {
        public int id;
        public string name;
        public string serialNumber;
        public string decimalNumber;
        public string version;
        public string parent;
        public string product;

        public ControllObject() { }

        public ControllObject(string name,
            string serialNumber,
            string decimalNumber,
            string version,
            string parent,
            string product)
        {
            this.name = name;
            this.serialNumber = serialNumber;
            this.decimalNumber = decimalNumber;
            this.version = version;
            this.parent = parent;
            this.product = product;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public ControllObject(int id)
        {
#if DEBUG
            Debug.Assert(Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

            string query = $"SELECT [id], [name], [serial_number], [decimal_number], [version], [parent], [product] FROM [controll_object] WHERE [id] = '{id}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            bool res = reader.Read();

            this.id = id;
            name = (string)reader[1];
            serialNumber = (string)reader[2];
            decimalNumber = (string)reader[3];
            version = (string)reader[4];
            parent = (string)reader[5];
            product = (string)reader[6];

            reader.Close();
        }

        public int GenerateId()
        {
            id = Utils.Hash(name + serialNumber + version + parent);
            return id;
        }

        public void SaveData(SQLiteConnection connection)
        {
#if DEBUG
            Debug.Assert(connection.State == System.Data.ConnectionState.Open,
                "connection must be opened");
#endif

            string query = "REPLACE INTO [controll_object] ([id], [name], [serial_number], "
                + $"[decimal_number], [version], [parent], [product]) VALUES ({id}, '{name}', "
                + $"'{serialNumber}', '{decimalNumber}', '{version}', '{parent}', '{product}')";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}
