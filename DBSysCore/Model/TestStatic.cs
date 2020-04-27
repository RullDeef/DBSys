using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DBSysCore.Model
{
    [Serializable]
    public class TestStatic
    {
        public int id;
        public string tsIndex;
        public string mode;
        public Module module;
        public Methodology methodology;
        public Requirements requirements;
        public string unit;
        
        public string description;

        public TestStatic() { }

        public TestStatic(int id,
                    string tsIndex,
                    string mode,
                    Module module,
                    Methodology methodology,
                    Requirements requirements,
                    string unit,
                    string description = null)
        {
            this.id = id;
            this.tsIndex = tsIndex;
            this.mode = mode;
            this.module = module;
            this.methodology = methodology;
            this.requirements = requirements;
            this.unit = unit;

            this.description = description;

            SaveData(Core.dumpConnection);
        }

        public TestStatic(string tsIndex)
        {
            string query = "SELECT [id], [ts_index], [mode], [module], "
                + "[methodology], [requirements], [unit], "
                + $"[description] FROM [test_static] WHERE [ts_index] = '{tsIndex}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            id = (int)reader[0];
            this.tsIndex = (string)reader[1];
            mode = (string)reader[2];
            int moduleId = (int)reader[3];
            int methodologyId = (int)reader[4];
            int requirementsId = (int)reader[5];
            unit = (string)reader[6];
            description = (string)reader[7];

            reader.Close();

            module = new Module(moduleId);
            methodology = new Methodology(methodologyId);
            requirements = new Requirements(requirementsId);
        }

        /**
         * Returns all static test data from dump connection.
         * Connection must be opened!
         */
        public static List<TestStatic> GetTests()
        {
            List<int> moduleIds = new List<int>();
            List<int> methodologyIds = new List<int>();
            List<int> requirementsIds = new List<int>();

            List<TestStatic> result = new List<TestStatic>();

            string query = "SELECT [id], [ts_index], [mode], [module], "
                + "[methodology], [requirements], [unit], [description] FROM [test_static]";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            while (reader.Read())
            {
                TestStatic test = new TestStatic();
                test.id = (int)reader[0];
                test.tsIndex = (string)reader[1];
                test.mode = (string)reader[2];
                moduleIds.Add((int)reader[3]);
                methodologyIds.Add((int)reader[4]);
                requirementsIds.Add((int)reader[5]);
                test.unit = (string)reader[6];
                test.description = (string)reader[7];

                result.Add(test);
            }

            reader.Close();

            for (int i = 0; i < result.Count; i++)
            {
                TestStatic test = result[i];
                test.module = new Module(moduleIds[i]);
                test.methodology = new Methodology(methodologyIds[i]);
                test.requirements = new Requirements(requirementsIds[i]);
            }

            return result;
        }

        public void SaveData(SQLiteConnection connection)
        {
            module.SaveData(connection);
            methodology.SaveData(connection);
            requirements.SaveData(connection);

            string query = "INSERT OR REPLACE INTO [test_static] ([id], [ts_index], [mode], [module], "
                + "[methodology], [requirements], [unit], [description]) VALUES "
                + $"({id}, '{tsIndex}', '{mode}', '{module.id}', {methodology.id}, {requirements.id}, "
                + $"'{unit}', '{description}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public override string ToString()
        {
            return description;
        }
    }
}
