using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SQLite;

namespace DBSysCore.Model
{
    [Serializable]
    public class TestDynamic
    {
        public int id;
        public string tsIndex;
        public Challenge challenge;

        public DateTime beginTime;
        public DateTime endTime;

        public decimal? nominal;
        public decimal? actualValue;
        public decimal? delta;
        public decimal? boundaryValue;

        public bool status;

        public TestDynamic() { }

        public TestDynamic(string tsIndex,
                Challenge challenge,
                DateTime beginTime,
                DateTime endTime,
                decimal? nominal,
                decimal? actualValue,
                decimal? delta,
                decimal? boundaryValue,
                bool status)
        {
            this.tsIndex = tsIndex;
            this.challenge = challenge;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.nominal = nominal;
            this.actualValue = actualValue;
            this.delta = delta;
            this.boundaryValue = boundaryValue;
            this.status = status;

            GenerateId();
        }

        public TestStatic GetTestStatic()
        {
            return new TestStatic(tsIndex);
        }

        /*
        public TestDynamic(int id)
        {
            string query = "SELECT ([id], [ts_index], [challenge], " +
                "[begin_time], [end_time], [actual_value], [status]) " +
                $"FROM [test_dynamic] WHERE [id] = '{id}'";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            this.id = id;
            tsIndex = (string)reader[1];
            challenge = new Challenge((int)reader[2]);
            beginTime = reader.GetDateTime[3];
            endTime = reader.GetDateTime[4];
            actualValue = (decimal)reader[5];
            status = (bool)reader[6];

            reader.Close();
        }
        */

        public int GenerateId()
        {
            id = Utils.Hash(beginTime.ToString());
            return id;
        }

        public void SaveData(SQLiteConnection connection)
        {
            string query = "REPLACE INTO [test_dynamic] ([id], [ts_index], [challenge], "
                + "[begin_time], [end_time], [nominal], [actual_value], [delta], [boundary_value], [status]) VALUES "
                + $"({id}, '{tsIndex}', {challenge.id}, '{beginTime:yyyy-MM-dd hh:mmm:ss}', "
                + $"'{endTime:yyyy-MM-dd hh:mmm:ss}', '{nominal}', '{actualValue}', " +
                $"'{delta}', '{boundaryValue}', '{status}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static List<TestDynamic> GetTests()
        {
            string query = "SELECT [id], [ts_index], [challenge], " +
                "[begin_time], [end_time], [actual_value], [delta], " +
                "[boundary_value], [status] FROM [test_dynamic]";

            return GetTestsByQuery(query);
        }

        public static List<TestDynamic> GetTests(Challenge challenge)
        {
            string query = "SELECT [id], [ts_index], [challenge], " +
                "[begin_time], [end_time], [actual_value], [delta], [boundary_value], [status] " +
                $"FROM [test_dynamic] WHERE [challenge] = {challenge.id}";

            return GetTestsByQuery(query);
        }

        public static List<TestDynamic> GetTests(DateTime beginDate, DateTime endDate)
        {
            string query = "SELECT [id], [ts_index], [challenge], " +
                "[begin_time], [end_time], [actual_value], [delta], [boundary_value], [status] " +
                $"FROM [test_dynamic] WHERE [begin_time] >= '{beginDate:yyyy-MM-dd hh:mmm:ss}' " +
                $"AND [begin_time] <= '{endDate:yyyy-MM-dd hh:mmm:ss}'";

            return GetTestsByQuery(query);
        }

        private static List<TestDynamic> GetTestsByQuery(string query)
        {
            List<TestDynamic> result = new List<TestDynamic>();
            List<int> challengesId = new List<int>();

            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            while (reader.Read())
            {
                TestDynamic test = new TestDynamic
                {
                    id = (int)reader[0],
                    tsIndex = (string)reader[1],
                    // challenge = new Challenge((int)reader[2]),
                    beginTime = DateTime.Parse((string)reader[3]),
                    endTime = DateTime.Parse((string)reader[4]),
                    actualValue = reader.IsDBNull(5) ? null : (decimal?)reader[5],
                    delta = reader.IsDBNull(6) ? null : (decimal?)reader[6],
                    boundaryValue = reader.IsDBNull(7) ? null : (decimal?)reader[7],
                    status = (bool)reader[8]
                };
                challengesId.Add((int)reader[2]);

                result.Add(test);
            }

            reader.Close();

            for (int i = 0; i < result.Count; i++)
                result[i].challenge = new Challenge(challengesId[i]);

            return result;
        }
    }
}
