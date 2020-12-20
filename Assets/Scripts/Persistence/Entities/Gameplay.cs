using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    class Gameplay : IEntity
    {
        public int GameplayID { get; set; }
        public DateTime Date { get; set; }
        public int PlayerState { get; set; }
        public TimeSpan PlayedTime { get; set; }
        public int Score { get; set; }
        public short CurrentLevelID { get; set; }
        public List<int> EnemiesAlive { get; set; }
    
        public Dictionary<string, object> Save()
        {
            return new Dictionary<string, object>()
            {
                { "ID", GameplayID },
                { "PState", PlayerState },
                { "Date", Date },
                { "Time", PlayedTime },
                { "Level", CurrentLevelID },
                { "Score", Score },
                { "Enemies", EnemiesAlive }
            };
        }

        public void Load(Dictionary<string, object> data)
        {
            GameplayID = Convert.ToInt32(data["ID"]);
            PlayerState = Convert.ToInt32(data["PState"]);
            Date = Convert.ToDateTime(data["Date"]);
            PlayedTime = (TimeSpan)data["Time"];
            CurrentLevelID = Convert.ToInt16(data["Level"]);
            Score = Convert.ToInt32(data["Score"]);
            EnemiesAlive = (List<int>)data["Enemies"];
        }
    }

}
