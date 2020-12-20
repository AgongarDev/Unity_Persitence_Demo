using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Persistence
{
    public class Gun :IEntity
    {
        public int GunID { get; internal set; }
        public string Name { get; internal set; }
        public float Damage { get; internal set; }
        public string Prefab { get; internal set; }
        public bool IsMain { get; internal set; }

        public Dictionary<string, object> Save()
        {
            return new Dictionary<string, object>()
            {
                { "ID", GunID },
                { "Name", Name },
                { "Damage", Damage },
                { "Prefab", Prefab }
            };
        }

        public void Load(Dictionary<string, object> data)
        {
            GunID = Convert.ToInt32(data["ID"]);
            Name = Convert.ToString(data["Name"]);
            Damage = Convert.ToSingle(data["Damage"]);
            Prefab = Convert.ToString(data["Prefab"]);
        }
    }
}