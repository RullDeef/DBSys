using System;
using System.Collections.Generic;
using System.Data.SQLite;


namespace DBSysCore.Model
{
    [Serializable]
    public class Challenge
    {
        public int id;
        public ControllObject controllObject;
        public ChallengeType challengeType;
        public Staff staff;
        public Location location;

        public DateTime beginTime;
        public DateTime endTime;
        public string description;

        public Challenge() { }

        public Challenge(ControllObject controllObject,
                ChallengeType challengeType,
                Staff staff,
                Location location,
                DateTime beginTime,
                DateTime endTime,
                string description = null)
        {
            this.controllObject = controllObject;
            this.challengeType = challengeType;
            this.staff = staff;
            this.location = location;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.description = description;

            GenerateId();
            SaveData(Core.dumpConnection);
        }

        public Challenge(int id)
        {
            string query = "SELECT [id], [controll_object], "
                + "[challenge_type], [staff], [location], [begin_time], "
                + $"[end_time], [description] FROM [challenge] WHERE [id] = {id}";
            SQLiteDataReader reader = Utils.ExecuteReader(query, Core.dumpConnection);

            reader.Read();

            this.id = id;
            int controllObjectId = (int)reader[1];
            int challengeTypeId = (int)reader[2];
            int staffId = (int)reader[3];
            int locationId = (int)reader[4];
            // beginTime = DateTime.Parse((string)reader[5]);
            // endTime = DateTime.Parse((string)reader[6]);
            beginTime = (DateTime)reader[5];
            endTime = (DateTime)reader[6];
            description = (string)reader[7];

            reader.Close();

            controllObject = new ControllObject(controllObjectId);
            challengeType = new ChallengeType(challengeTypeId);
            staff = new Staff(staffId);
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
            controllObject.SaveData(connection);
            challengeType.SaveData(connection);
            location.SaveData(connection);

            string query = "REPLACE INTO [challenge] ([id], [controll_object], "
                + "[challenge_type], [staff], [location], [begin_time], [end_time], "
                + $"[description]) VALUES ({id}, {controllObject.id}, {challengeType.id}, "
                + $"{staff.id}, {location.id}, '{beginTime.ToString("yyyy-MM-dd hh:mmm:ss")}', " +
                $"'{endTime.ToString("yyyy-MM-dd hh:mmm:ss")}', '{description}')";
            Utils.ExecuteNonQuery(query, connection);
        }

        public override string ToString()
        {
            return $"{beginTime.ToString("yyyy-MM-dd hh:mm")}";
        }
    }
}
