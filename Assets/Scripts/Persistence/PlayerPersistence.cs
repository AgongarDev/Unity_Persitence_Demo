using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class PlayerPersistence : SQLiteManager, IPersistent<PlayerState>
    {
        #region DbCons
        private const string TABLE_PLAYER = "Player_State";
        private const string KEY_PLAYER_ID = "Player_ID";
        private const string KEY_PLAYER_NAME = "Name";
        private const string KEY_PLAYER_LEVEL = "Level";
        private const string KEY_HP = "HP";
        private const string KEY_MAXHP = "Max_HP";
        private const string KEY_SHIELD = "Shield";
        private const string KEY_DAMAGE = "Damage";
        private const string KEY_XPOS = "X_pos";
        private const string KEY_YPOS = "Y_pos";
        private const string KEY_ZPOS = "Z_pos";

        //Relations 
        //Many to Many
        private const string KEY_ISMAIN = "Main";
        //Player_Guns
        private const string TABLE_PLAYER_GUNS = "Player_Guns";
        private const string KEY_GUN_ID = "Gun_ID";
        //Player_PowerUps
        private const string TABLE_PLAYER_PUPS = "Player_PowerUps";
        private const string KEY_POWERUP_ID = "PowerUp_ID";
        #endregion

        private int _PlayerId;
        GunPersistence _GunPersistence;
        PowerUpPersistence _PowerUpPersistence;

        public PlayerPersistence() : base()
        {
            _GunPersistence = new GunPersistence();
            _PowerUpPersistence = new PowerUpPersistence();
        }

        public object InsertOrUpdate(PlayerState p)
        {
            if (GetID(p.Name) != 0)
            {
                return Update(p);
            }
            return Insert(p);
        }
        /// <summary>
        /// Insert Player state and his relations with guns and powerups
        /// </summary>
        /// <param name="playerState"></param>
        public object Insert(PlayerState playerState)
        {
            Dictionary<string, object> data = playerState.Save();
            IDbTransaction transaction = BeginTransaction();
            try
            {
                _PlayerId = (int)InsertPlayerState(data);

                InsertOrUpdate_PlayerGuns((List<Gun>)data["Guns"]);
                InsertOrUpdate_PlayerPowerUps((List<PowerUp>)data["PowerUps"]);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return _PlayerId;
        }
        /// <summary>
        /// Specific insert to get the result id from sqlite autoincrement and use it at Guns and PowerUps persistences
        /// </summary>
        /// <param name="playerState"></param>
        /// <returns></returns>
        private object InsertPlayerState(Dictionary<string, object> data)
        {
            IDbCommand dbcmd = GetDbCommand();
            try
            {
                #region Insert player query
                dbcmd.CommandText =
                    "INSERT INTO "
                    + TABLE_PLAYER + " ("
                    + KEY_PLAYER_NAME + ", "
                    + KEY_PLAYER_LEVEL + ", "
                    + KEY_HP + ", "
                    + KEY_MAXHP + ", "
                    + KEY_SHIELD + ", "
                    + KEY_DAMAGE + ", "
                    + KEY_XPOS + ", "
                    + KEY_YPOS + ", "
                    + KEY_ZPOS + ") "

                    + "VALUES ("
                    + Convert.ToString(data["Name"]) + ", "
                    + Convert.ToInt32(data["PLevel"]) + ", "
                    + Convert.ToSingle(data["HP"]) + ", "
                    + Convert.ToSingle(data["MaxHP"]) + ", "
                    + Convert.ToSingle(data["Shield"]) + ", "
                    + Convert.ToSingle(data["Damage"]) + ", "
                    + Convert.ToSingle(data["XPos"]) + ", "
                    + Convert.ToSingle(data["YPos"]) + ", "
                    + Convert.ToSingle(data["ZPos"]) + ")";
                #endregion

                return dbcmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Update Player state and his relations with guns and powerups
        /// </summary>
        /// <param name="playerState"></param>
        /// <returns></returns>
        public object Update(PlayerState playerState)
        {
            object res = null;

            Dictionary<string, object> data = playerState.Save();
            IDbTransaction transaction = BeginTransaction();
            try
            {
                IDbCommand dbcmd = GetDbCommand();

                #region Update player query
                dbcmd.CommandText =
                    "UPDATE TABLE "
                    + TABLE_PLAYER + " SET "
                    + KEY_PLAYER_NAME + $" = {data["Name"]}"
                    + KEY_PLAYER_LEVEL + $" = {data["PLevel"]}, "
                    + KEY_HP + $" = {data["HP"]}, "
                    + KEY_MAXHP + $" = {data["MaxHP"]}, "
                    + KEY_SHIELD + $" = {data["Shield"]}, "
                    + KEY_DAMAGE + $" = {data["Damage"]}, "
                    + KEY_XPOS + $" = {data["XPos"]}, "
                    + KEY_YPOS + $" = {data["YPos"]}, "
                    + KEY_ZPOS + $" = {data["ZPos"]}, "
                    + $"WHERE {KEY_PLAYER_ID} = {data["ID"]}";
                #endregion
                res = dbcmd.ExecuteScalar();

                InsertOrUpdate_PlayerGuns((List<Gun>)data["Guns"]);
                InsertOrUpdate_PlayerPowerUps((List<PowerUp>)data["PowerUps"]);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return res;
        }
        public int GetID(object pName)
        {
            IDbCommand dbcmd = GetDbCommand();

            #region Select player query
            dbcmd.CommandText =
                $"SELECT {KEY_PLAYER_ID} " +
                $"FROM {TABLE_PLAYER} " +
                $"WHERE {KEY_PLAYER_NAME} = {(string)pName}";
            #endregion

            return (int)dbcmd.ExecuteScalar();
        }
        public Dictionary<string, object> Get(int id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<Gun> PlayerGuns = new List<Gun>();
            List<PowerUp> PlayerPowers = new List<PowerUp>();
            
            IDbCommand dbcmd = GetDbCommand();

            dbcmd.CommandText = "SELECT *"
                + $" FROM {TABLE_PLAYER}"
                + $" WHERE {KEY_PLAYER_ID} = {id}";

            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                data = new Dictionary<string, object>()
                {
                    { "ID", reader[0] },
                    { "Name", reader[1] },
                    { "HP", reader[2] },
                    { "MaxHP", reader[3] },
                    { "Level", reader[4] },
                    { "Shield", reader[5] },
                    { "Damage", reader[6] },
                    { "XPos", reader[7] },
                    { "YPos", reader[8] },
                    { "ZPos", reader[9] }
                };
            }

            #region create Guns list
            var guns = GetPlayerGuns_FromSQL();

            foreach (object[] gunInfo in guns)
            {
                var gun = _GunPersistence.Get((int)gunInfo[0]);
                if (gun != null)
                {
                    PlayerGuns.Add(gun);

                    if ((bool)gunInfo[1])
                        data.Add("MainGun", gun.GunID);
                }
            }
            #endregion
            data.Add("Guns", PlayerGuns);

            #region Create PowerUps list
            var powers = GetPlayerPowerUps_FromSQL();
            foreach (int p in powers)
            {
                var power = _PowerUpPersistence.Get(p);
                if (power != null) 
                    PlayerPowers.Add(power);
            }
            #endregion

            data.Add("PowerUps", PlayerPowers);
            
            return data;
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns>Empty list of PlayerState</returns>
        public List<PlayerState> GetAll()
        {
            Debug.WriteLine("Function not implemented");
            return new List<PlayerState>();
        }
        public void Delete()
        {
            IDbCommand dbcmd = GetDbCommand();

            dbcmd.CommandText =
                "DELETE " +
                $"FROM {TABLE_PLAYER} " +
                $"WHERE {KEY_PLAYER_ID} = {_PlayerId} ";
            dbcmd.ExecuteNonQuery();
        }
        public void Delete(int id)
        {
            IDbCommand dbcmd = GetDbCommand();

            dbcmd.CommandText =
                "DELETE " +
                $"FROM {TABLE_PLAYER} " +
                $"WHERE {KEY_PLAYER_ID} = {id} ";
            dbcmd.ExecuteNonQuery();
        }

        #region playerGuns relation
        /// <summary>
        /// We will make sure there exists all the player guns at database. 
        /// If not we will insert them so we can provide more guns with no 
        /// worry about persistence at game start.
        /// After this, insert into a relational table between player and guns all the guns that player has earned at the current game
        /// </summary>
        /// <param name="playerGuns"></param>
        /// <returns></returns>
        public void InsertOrUpdate_PlayerGuns(List<Gun> playerGuns)
        {

            IDbCommand dbcmd = GetDbCommand();
            var gunsFromDb = GetPlayerGuns_FromSQL();

            foreach (Gun g in playerGuns)
            {
                try
                {
                    if (_GunPersistence.GetID(g.Name) == 0)
                        _GunPersistence.Insert(g);

                    if (!gunsFromDb.Any(id => id.Equals(g.GunID)))
                    {
                        InsertPlayerGun(dbcmd, g);
                        continue;
                    }
                    UpdateEnemyGun(dbcmd, g);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            DeleteNonExistingRelation(dbcmd, playerGuns);
        }


        private void UpdateEnemyGun(IDbCommand dbcmd, Gun g)
        {
            #region Update enemy_guns query
            dbcmd.CommandText =
                "UPDATE TABLE "
                + TABLE_PLAYER_GUNS + " SET "
                + KEY_ISMAIN + $" = {g.IsMain} "
                + "WHERE " + KEY_PLAYER_ID + " = " + _PlayerId
                + "AND " + KEY_GUN_ID + "= " + g.GunID;
            #endregion
            dbcmd.ExecuteNonQuery();
        }
        private void InsertPlayerGun(IDbCommand dbcmd, Gun g)
        {
            #region Insert player_guns query
            dbcmd.CommandText =
                "INSERT INTO "
                + TABLE_PLAYER_GUNS + " ("
                + KEY_PLAYER_ID + ", "
                + KEY_GUN_ID + ", "
                + KEY_ISMAIN + ") "

                + "VALUES ("
                + _PlayerId + ", "
                + g.GunID + ", "
                + g.IsMain + ")";
            #endregion

            dbcmd.ExecuteNonQuery();
        }
        private List<Array> GetPlayerGuns_FromSQL()
        {
            IDbCommand dbcmd = GetDbCommand();
            List<Array> gunsInfo = new List<Array>();

            #region Select Enemies_Guns query
            dbcmd.CommandText =
                $"SELECT {KEY_GUN_ID}," +
                $" {KEY_ISMAIN} " +
                $"FROM {TABLE_PLAYER_GUNS} " +
                $"WHERE {KEY_PLAYER_ID} = {_PlayerId} ";
            #endregion
            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                gunsInfo.Add(new object[]
                        { reader[0],
                          reader[1] });
            }
            return gunsInfo;
        }
        private void DeleteNonExistingRelation(IDbCommand dbcmd, List<Gun> enemyGuns)
        {
            var gunsIds = enemyGuns.Select(g => g.GunID).ToList();

            #region Delete not exisiting enemy_guns query
            dbcmd.CommandText =
                "DELETE FROM "
                + TABLE_PLAYER_GUNS
                + "WHERE " + KEY_PLAYER_ID + " = " + _PlayerId
                + $"AND NOT IN {gunsIds}";
            #endregion
            dbcmd.ExecuteNonQuery();
        }
        #endregion

        #region playerPowerUps relation
        public void InsertOrUpdate_PlayerPowerUps(List<PowerUp> playerPowerUps)
        {
            IDbCommand dbcmd = GetDbCommand();
            var powerUpsFromDb = GetPlayerPowerUps_FromSQL();

            foreach (PowerUp p in playerPowerUps)
            {
                try
                {
                    if (_PowerUpPersistence.GetID(p.Name) == 0)
                        _PowerUpPersistence.Insert(p);

                    if (powerUpsFromDb.Any(id => id.Equals(p.PowerUpID)))
                        continue;

                    InsertPlayerPowerUP(dbcmd, p);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            DeleteNonExistingRelation(dbcmd, playerPowerUps);
        }
        private void InsertPlayerPowerUP(IDbCommand dbcmd, PowerUp p)
        {
            #region Insert Player_powerUps query
            dbcmd.CommandText =
                "INSERT INTO "
                + TABLE_PLAYER_PUPS + " ("
                + KEY_PLAYER_ID + ", "
                + KEY_POWERUP_ID + ") "

                + "VALUES ("
                + _PlayerId + ", "
                + p.PowerUpID + ")";
            #endregion

            dbcmd.ExecuteNonQuery();
        }
        private List<int> GetPlayerPowerUps_FromSQL()
        {
            IDbCommand dbcmd = GetDbCommand();
            List<int> powerUpsid = new List<int>();

            #region Select Enemies_Guns query
            dbcmd.CommandText =
                $"SELECT {KEY_POWERUP_ID} " +
                $"FROM {TABLE_PLAYER_PUPS} " +
                $"WHERE {KEY_PLAYER_ID} = {_PlayerId} ";
            #endregion
            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                powerUpsid.Add((int)reader[0]);
            }
            return powerUpsid;
        }
        private void DeleteNonExistingRelation(IDbCommand dbcmd, List<PowerUp> playerPowerUps)
        {
            var powerUpsIds = playerPowerUps.Select(p => p.PowerUpID).ToList();

            #region Delete not exisiting enemy_guns query
            dbcmd.CommandText =
                "DELETE FROM "
                + TABLE_PLAYER_PUPS
                + "WHERE " + KEY_PLAYER_ID + " = " + _PlayerId
                + $"AND NOT IN {powerUpsIds}";
            #endregion
            dbcmd.ExecuteNonQuery();
        }
        #endregion

    }
}
