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

        public decimal nominal;
        public decimal actualValue;
        public decimal delta;
        public decimal boundaryValue;

        public bool status;

        public TestDynamic() { }

        public TestDynamic(string tsIndex,
                Challenge challenge,
                DateTime beginTime,
                DateTime endTime,
                decimal nominal,
                decimal actualValue,
                decimal delta,
                decimal boundaryValue,
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
            // dont needed
            // tsIndex.SaveData(connection);
            
            string query = "REPLACE INTO [test_dynamic] ([id], [ts_index], [challenge], "
                + "[begin_time], [end_time], [nominal], [actual_value], [delta], [boundary_value], [status]) VALUES "
                + $"({id}, '{tsIndex}', {challenge.id}, '{beginTime.ToString("yyyy-MM-dd hh:mmm:ss")}', "
                + $"'{endTime.ToString("yyyy-MM-dd hh:mmm:ss")}', '{nominal}', '{actualValue}', " +
                $"'{delta}', '{boundaryValue}', '{status}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static List<TestDynamic> GetTests(Challenge challenge)
        {
            List<TestDynamic> result = new List<TestDynamic>();
            List<int> challengesId = new List<int>();
            
            string query = "SELECT [id], [ts_index], [challenge], " +
                "[begin_time], [end_time], [actual_value], [status] " +
                $"FROM [test_dynamic] WHERE [challenge] = {challenge.id}";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            while (reader.Read())
            {
                TestDynamic test = new TestDynamic();
                test.id = (int)reader[0];
                test.tsIndex = (string)reader[1];
                challengesId.Add((int)reader[2]);
                test.beginTime = DateTime.Parse((string)reader[3]);
                test.endTime = DateTime.Parse((string)reader[4]);
                test.actualValue = (decimal)reader[5];
                test.status = (bool)reader[6];

                result.Add(test);
            }

            reader.Close();

            for (int i = 0; i < result.Count; i++)
                result[i].challenge = new Challenge(challengesId[i]);

            return result;
        }
    }
}
