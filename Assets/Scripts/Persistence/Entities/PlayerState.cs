using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Persistence
{
    public class PlayerState : MonoBehaviour, IEntity
    {
        public int PlayerStateID;
        public string Name;
        public float HP;
        public float MaxHP;
        public PlayerLevel Level;
        public float Shield;
        public float Damage;
        public int MainGun;
        public List<Gun> Guns;
        public List<PowerUp> PowerUps;

        public virtual Dictionary<string, object> Save()
        {
            return new Dictionary<string, object>()
            {
                { "ID", PlayerStateID },
                { "Name", Name },
                { "HP", HP },
                { "MaxHP", MaxHP },
                { "Level", Level },
                { "Shield", Shield },
                { "Damage", Damage },
                { "Guns", Guns },
                { "MainGun", MainGun },
                { "PowerUps", PowerUps },
                { "XPos", transform.position.x },
                { "YPos", transform.position.y },
                { "ZPos", transform.position.z }
            };
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="data"></param>
        public virtual void Load(Dictionary<string, object> data)
        {
            PlayerStateID = Convert.ToInt32(data["ID"]);
            Name = Convert.ToString(data["Name"]);
            HP = Convert.ToSingle(data["HP"]);
            MaxHP = Convert.ToSingle(data["MaxHP"]);
            Level = (PlayerLevel)Convert.ToInt16(data["Level"]);
            Shield = Convert.ToSingle(data["Shield"]);
            Damage = Convert.ToSingle(data["Damage"]);
            Guns = ((List<Gun>)data["Guns"]).ToList();
            MainGun = Guns.Where(g => g.IsMain == true).First().GunID;
            PowerUps = ((List<PowerUp>)data["PowerUps"]).ToList();
            float x = Convert.ToSingle(data["XPos"]);
            float y = Convert.ToSingle(data["YPos"]);
            float z = Convert.ToSingle(data["ZPos"]);
            transform.localPosition = new Vector3(x, y, z);
        }
    }
}
