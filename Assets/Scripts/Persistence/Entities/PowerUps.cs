using System;
using System.Collections.Generic;

namespace Persistence
{
    public class PowerUp : IEntity
    {
        public int PowerUpID { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        //Modificar el tipo int de PwUType por una entidad de categorías o un enum
        public int PwUType { get; set; }
        public string Target { get; set; }


        public Dictionary<string, object> Save()
        {
            return new Dictionary<string, object>()
            {
                { "ID", PowerUpID },
                { "Name", Name },
                { "Value", Value },
                { "Type", PwUType },
                { "Target", Target }
            };
        }

        public void Load(Dictionary<string, object> data)
        {
            PowerUpID = Convert.ToInt32(data["ID"]);
            Name = Convert.ToString(data["Name"]);
            Value = Convert.ToSingle(data["Value"]);
            PwUType = Convert.ToInt16(data["Type"]);
            Target = Convert.ToString(data["Prefab"]);
        }

    }
}