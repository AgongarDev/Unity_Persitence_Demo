using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Persistence
{
    public class Enemy : MonoBehaviour, IEntity
    {
        public int EnemyID;
        public string Name;
        public float HP;
        public float MaxHP;
        public PlayerLevel Level;
        public float Shield;
        public float Damage;
        public List<Gun> Guns;
        public int MainGunID;
        public string Prefab;

        public virtual Dictionary<string, object> Save()
        {
            return new Dictionary<string, object>()
            {
                { "ID", EnemyID },
                { "Name", Name },
                { "HP", HP },
                { "MaxHP", MaxHP },
                { "Level", Level },
                { "Shield", Shield },
                { "Damage", Damage },
                { "Guns", Guns },
                { "MainGun", MainGunID },
                { "Prefab", gameObject.name },
                { "XPos", transform.localPosition.x },
                { "YPos", transform.localPosition.y },
                { "ZPos", transform.localPosition.z }
            };
        }

        public virtual void Load(Dictionary<string, object> data)
        {
            EnemyID = Convert.ToInt32(data["ID"]);
            Name = Convert.ToString(data["Name"]);
            HP = Convert.ToSingle(data["HP"]);
            MaxHP = Convert.ToSingle(data["MaxHP"]);
            Level = (PlayerLevel)Convert.ToInt16(data["Level"]);
            Shield = Convert.ToSingle(data["Shield"]);
            Damage = Convert.ToSingle(data["Damage"]);
            Guns = ((List<Gun>)data["Guns"]).ToList();
            MainGunID = Guns.Where(g => g.IsMain == true).First().GunID;
            Prefab = Convert.ToString(data["Prefab"]);
            float x = Convert.ToSingle(data["XPos"]);
            float y = Convert.ToSingle(data["YPos"]);
            float z = Convert.ToSingle(data["ZPos"]);
            transform.localPosition = new Vector3(x, y, z);
        } 
    }
}
