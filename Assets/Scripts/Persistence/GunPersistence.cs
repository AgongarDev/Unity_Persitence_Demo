using System;
using System.Collections.Generic;
using System.Data;

namespace Persistence
{
    class GunPersistence : SQLiteManager, IPersistent<Gun>
    {
        private const string TABLE_GUNS = "Guns";
        private const string KEY_GUN_ID = "gun_id";
        private const string KEY_GUN_NAME = "name";
        private const string KEY_GUN_DAMAGE = "damage";
        private const string KEY_GUN_PREFAB = "prefab_name";

        public GunPersistence() : base() { }

        public object Insert(Gun g)
        {
            Dictionary<string, object> data = g.Save();
            IDbCommand dbcmd = GetDbCommand();

            #region Insert Gun query
            dbcmd.CommandText =
                "INSERT INTO "
                + TABLE_GUNS + " ("
                + KEY_GUN_NAME + ", "
                + KEY_GUN_DAMAGE + ", "
                + KEY_GUN_PREFAB + ") "

                + "VALUES ("
                + Convert.ToString(data["Name"]) + ", "
                + Convert.ToSingle(data["Damage"]) + ", "
                + Convert.ToString(data["Prefab"]) + ")";
            #endregion

            var rs = dbcmd.ExecuteScalar();
            g.GunID = (int)rs;
            return rs;
        }
       
        public object Update(Gun g)
        {
            Dictionary<string, object> data = g.Save();
            IDbCommand dbcmd = GetDbCommand();

            #region Update player query
            dbcmd.CommandText =
                "UPDATE TABLE "
                + TABLE_GUNS + " SET "
                + KEY_GUN_NAME + $" = {data["Name"]}, "
                + KEY_GUN_DAMAGE + $" = {data["Damage"]}, "
                + KEY_GUN_PREFAB + $" = {data["Prefab"]} "
                + $"WHERE {KEY_GUN_ID} = {data["ID"]}";
            #endregion
            return dbcmd.ExecuteScalar();
        }
        
        public object InsertOrUpdate(Gun g)
        {
            if (GetID(g.Name) != 0)
            {
                return Update(g);
            }
            return Insert(g);
        }

        public int GetID(object name)
        {
            IDbCommand dbcmd = GetDbCommand();

            #region Select player query
            dbcmd.CommandText =
                $"SELECT {KEY_GUN_ID} " +
                $"FROM {TABLE_GUNS} " +
                $"WHERE {KEY_GUN_NAME} = {(string)name}";
            #endregion

            return (int)dbcmd.ExecuteScalar();
        }
        
        public Gun Get(int id)
        {
            IDbCommand dbcmd = GetDbCommand();
            Gun gun = new Gun();
            Dictionary<string, object> data = new Dictionary<string, object>();

            #region Select player query
            dbcmd.CommandText =
                $"SELECT * " +
                $"FROM {TABLE_GUNS} " +
                $"WHERE {KEY_GUN_ID} = {id}";
            #endregion
            var reader = dbcmd.ExecuteReader();
            
            while (reader.Read())
            {
                data.Add("ID", reader[0]);
                data.Add("Name", reader[1]);
                data.Add("Damage", reader[2]);
                data.Add("Prefab", reader[3]);
            }
            gun.Load(data);
            return gun;
        }
        
        public List<Gun> GetAll()
        {
            IDbCommand dbcmd = GetDbCommand();

            List<Gun> guns = new List<Gun>();
            dbcmd.CommandText =
                " SELECT * " +
                $"FROM {TABLE_GUNS}";
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                guns.Add((Gun)reader);
            }
            return guns;
        }

        public void Delete(int id)
        {
            IDbCommand dbcmd = GetDbCommand();

            #region Delete Gun query
            dbcmd.CommandText =
                $"DELETE " +
                $"FROM {TABLE_GUNS} " +
                $"WHERE {KEY_GUN_ID} = {id} ";
            #endregion
            dbcmd.ExecuteNonQuery();
        }
    }
}
