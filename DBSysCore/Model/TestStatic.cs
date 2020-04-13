using System;
using System.Data.SQLite;

namespace DBSysCore.Model
{
    [Serializable]
    public class TestStatic
    {
        public int id;
        public string tsIndex;
        public string mode;
        public ConnectionInterface connectionInterface;
        public Methodology methodology;
        public Requirements requirements;
        public string unit;
        
        public string description;

        public TestStatic() { }

        public TestStatic(int id,
                    string tsIndex,
                    string mode,
                    ConnectionInterface connectionInterface,
                    Methodology methodology,
                    Requirements requirements,
                    string unit,
                    string description = null)
        {
            this.id = id;
            this.tsIndex = tsIndex;
            this.mode = mode;
            this.connectionInterface = connectionInterface;
            this.methodology = methodology;
            this.requirements = requirements;
            this.unit = unit;

            this.description = description;

            SaveData(Core.dumpConnection);
        }

        public TestStatic(string tsIndex)
        {
            string query = "SELECT [id], [ts_index], [mode], [connection_interface], "
                + "[methodology], [requirements], [unit], "
                + $"[description] FROM [test_static] WHERE [ts_index] = '{tsIndex}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            id = (int)reader[0];
            this.tsIndex = (string)reader[1];
            mode = (string)reader[2];
            int connectionInterfaceId = (int)reader[3];
            int methodologyId = (int)reader[4];
            int requirementsId = (int)reader[5];
            unit = (string)reader[6];
            description = (string)reader[7];

            reader.Close();

            connectionInterface = new ConnectionInterface(connectionInterfaceId);
            methodology = new Methodology(methodologyId);
            requirements = new Requirements(requirementsId);
        }

        public void SaveData(SQLiteConnection connection)
        {
            connectionInterface.SaveData(connection);
            methodology.SaveData(connection);
            requirements.SaveData(connection);

            string query = "INSERT OR REPLACE INTO [test_static] ([id], [ts_index], [mode], [connection_interface], "
                + "[methodology], [requirements], [unit], [description]) VALUES "
                + $"({id}, '{tsIndex}', '{mode}', '{connectionInterface.id}', {methodology.id}, {requirements.id}, "
                + $"'{unit}', '{description}')";
            Utils.ExecuteNonQuery(query, connection);
        }
    }
}
