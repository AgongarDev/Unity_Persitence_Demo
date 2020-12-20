using System;
using System.Collections.Generic;
using System.Data;

namespace Persistence
{
    public class PowerUpPersistence : SQLiteManager, IPersistent<PowerUp>
    {
        private const string TABLE_POWERUPS = "PowerUps";
        private const string KEY_POWERUP_ID = "ID";
        private const string KEY_POWERUP_NAME = "Name";
        private const string KEY_POWERUP_VALUE = "Value";
        private const string KEY_POWERUP_TARGET = "Target";
        private const string KEY_POWERUP_PREFAB = "Prefab";

        private const string TABLE_PLAYER_PUPS = "Player_PowerUps";

        public PowerUpPersistence() : base() { }

        public object Insert(PowerUp p)
        {

            Dictionary<string, object> data = p.Save();
            IDbCommand dbcmd = GetDbCommand();

            #region Insert Gun query
            dbcmd.CommandText =
                "INSERT INTO "
                + TABLE_POWERUPS + " ("
                + KEY_POWERUP_NAME + ", "
                + KEY_POWERUP_VALUE + ", "
                + KEY_POWERUP_TARGET + ", "
                + KEY_POWERUP_PREFAB + ") "

                + "VALUES ("
                + Convert.ToString(data["Name"]) + ", "
                + Convert.ToSingle(data["Value"]) + ", "
                + Convert.ToString(data["Target"])
                + Convert.ToString(data["Prefab"]) + ")";
            #endregion
            var rs = dbcmd.ExecuteScalar();
            p.PowerUpID = (int)rs;
            return rs;
        }
        public object Update(PowerUp p)
        {
            Dictionary<string, object> data = p.Save();
            IDbCommand dbcmd = GetDbCommand();

            #region Update powerUp query
            dbcmd.CommandText =
                "UPDATE TABLE "
                + TABLE_POWERUPS + " SET "
                + KEY_POWERUP_NAME + $" = {data["Name"]}, "
                + KEY_POWERUP_VALUE + $" = {data["Value"]}, "
                + KEY_POWERUP_TARGET + $" = {data["Target"]} "
                + KEY_POWERUP_PREFAB + $" = {data["Prefab"]} "
                + $"WHERE {KEY_POWERUP_ID} = {data["ID"]}";
            #endregion
            return dbcmd.ExecuteScalar();
        }
        public object InsertOrUpdate(PowerUp p)
        {
            if (GetID(p.Name) != 0)
            {
                return Update(p);
            }
            return Insert(p);
        }
        
        public int GetID(object name)
        {
            IDbCommand dbcmd = GetDbCommand();

            #region Select player query
            dbcmd.CommandText =
                $"SELECT {KEY_POWERUP_ID} " +
                $"FROM {TABLE_POWERUPS} " +
                $"WHERE {KEY_POWERUP_NAME} = {(string)name}";
            #endregion
            return (int)dbcmd.ExecuteScalar();
        }
        public PowerUp Get(int id)
        {
            IDbCommand dbcmd = GetDbCommand();
            PowerUp power = new PowerUp();
            Dictionary<string, object> data = new Dictionary<string, object>();

            #region Select player query
            dbcmd.CommandText =
                $"SELECT * " +
                $"FROM {TABLE_POWERUPS} " +
                $"WHERE {KEY_POWERUP_ID} = {id}";
            #endregion
            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                data.Add("ID", reader[0]);
                data.Add("Name", reader[1]);
                data.Add("Value", reader[2]);
                data.Add("Type", reader[3]);
                data.Add("Target", reader[4]);
            }
            power.Load(data);
            return power;

        }
        public List<PowerUp> GetAll()
        {
            IDbCommand dbcmd = GetDbCommand();

            List<PowerUp> powerUps = new List<PowerUp>();
            dbcmd.CommandText =
                " SELECT * " +
                $"FROM {TABLE_POWERUPS}";
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                powerUps.Add((PowerUp)reader);
            }
            return powerUps;
        }
        
        public void Delete(int id)
        {
            IDbCommand dbcmd = GetDbCommand();

            dbcmd.CommandText =
                "DELETE "
                + $"FROM {TABLE_POWERUPS} "
                + $"WHERE {KEY_POWERUP_ID} = {id}";
            dbcmd.ExecuteNonQuery();

            DeleteRelations(dbcmd, id);
        }

        private void DeleteRelations(IDbCommand dbcmd, int id)
        {
            dbcmd.CommandText =
                "DELETE "
                + $"FROM {TABLE_PLAYER_PUPS}"
                + $"WHERE {KEY_POWERUP_ID} = {id}";
            dbcmd.ExecuteNonQuery();
        }
    }
}