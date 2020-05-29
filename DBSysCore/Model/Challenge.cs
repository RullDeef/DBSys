using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace DBSysCore.Model
{
    [Serializable]
    public class Challenge
    {
        public int id;
        public ControllObject controllObject;
        public ChallengeType challengeType;
        public Staff staff;
        public Staff staffOTK;
        public Location location;

        public DateTime beginTime;
        public DateTime endTime;
        public string description;

        public Challenge() { }

        public Challenge(ControllObject controllObject,
                ChallengeType challengeType,
                Staff staff,
                Staff staffOTK,
                Location location,
                DateTime beginTime,
                DateTime endTime,
                string description = null)
        {
            this.controllObject = controllObject;
            this.challengeType = challengeType;
            this.staff = staff;
            this.staffOTK = staffOTK;
            this.location = location;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.description = description;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public Challenge(int id)
        {
#if DEBUG
            Debug.Assert(Core.dumpConnection != null && Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

            string query = "SELECT [id], [controll_object], "
                + "[challenge_type], [staff], [staff_otk], [location], [begin_time], "
                + $"[end_time], [description] FROM [challenge] WHERE [id] = {id}";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            this.id = id;
            int controllObjectId = (int)reader[1];
            int challengeTypeId = (int)reader[2];
            int staffId = (int)reader[3];
            int staffOTKId = (int)reader[4];
            int locationId = (int)reader[5];
            beginTime = (DateTime)reader[6];
            endTime = (DateTime)reader[7];
            description = (string)reader[8];

            reader.Close();

            controllObject = new ControllObject(controllObjectId);
            challengeType = new ChallengeType(challengeTypeId);
            staff = new Staff(staffId);
            staffOTK = new Staff(staffOTKId);
            location = new Location(locationId);
        }

        public int GenerateId()
        {
            id = Utils.Hash(challengeType.name + staff.firstName
                + location.name + beginTime.ToString());
            return id;
        }

        public void SaveData(SQLiteConnection connection)
        {
#if DEBUG
            Debug.Assert(connection.State == System.Data.ConnectionState.Open,
                "connection must be opened");
#endif

            controllObject.SaveData(connection);
            challengeType.SaveData(connection);
            location.SaveData(connection);

            string query = "REPLACE INTO [challenge] ([id], [controll_object], "
                + "[challenge_type], [staff], [staff_otk], [location], [begin_time], [end_time], "
                + $"[description]) VALUES ({id}, {controllObject.id}, {challengeType.id}, "
                + $"{staff.id}, {staffOTK.id}, {location.id}, '{beginTime:yyyy-MM-dd HH:mm:ss}', " +
                $"'{endTime:yyyy-MM-dd HH:mm:ss}', '{description}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public static List<Challenge> GetChallenges()
        {
#if DEBUG
            Debug.Assert(Core.dumpConnection.State == System.Data.ConnectionState.Open,
                "dump connection must be opened");
#endif

            List<Challenge> result = new List<Challenge>();

            string query = "SELECT [id] FROM [challenge]";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            List<int> ids = new List<int>();

            while (reader.Read())
                ids.Add((int)reader[0]);

            reader.Close();

            foreach (int id in ids)
                result.Add(new Challenge(id));

            return result;
        }

        public override string ToString()
        {
            return $"{beginTime:yyyy-MM-dd HH:mm}";
        }
    }
}
