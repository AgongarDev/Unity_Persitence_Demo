using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Persistence
{
    public class EnemiesPersistence : SQLiteManager, IPersistent<Enemy>
    {
        private const string TABLE_ENEMIES = "PowerUps";
        private const string KEY_ENEMY_ID = "ID";
        private const string KEY_ENEMY_NAME = "Name";
        private const string KEY_ENEMY_LEVEL = "Level";
        private const string KEY_HP = "HP";
        private const string KEY_MAXHP = "Max_HP";
        private const string KEY_SHIELD = "Shield_HP";
        private const string KEY_DAMAGE = "Damage";
        private const string KEY_XPOS = "X_pos";
        private const string KEY_YPOS = "Y_pos";
        private const string KEY_ZPOS = "Z_pos";
        private const string KEY_PREFAB = "Prefab";

        //Relations
        //Many to Many
        //Enemy_Guns
        private const string TABLE_ENEMY_GUNS = "Enemy_Guns";
        private const string KEY_GUN_ID = "Gun_id";
        private const string KEY_ISMAIN = "Main";

        private GunPersistence _GunPersistence;
        private int _EnemyId;

        public EnemiesPersistence() : base() { }

        public object Insert(Enemy e)
        {
            Dictionary<string, object> data = e.Save();
            IDbTransaction transaction = BeginTransaction();
            try
            {
                _EnemyId = (int)InsertEnemyData(data);

                InsertOrUpdate_EnemyGuns((List<Gun>)data["Guns"]);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return _EnemyId;
        }

        private object InsertEnemyData(Dictionary<string, object> data)
        {
            IDbCommand dbcmd = GetDbCommand();
            try
            {
                #region Insert player query
                dbcmd.CommandText =
                    "INSERT INTO "
                    + TABLE_ENEMIES + " ("
                    + KEY_ENEMY_NAME + ", "
                    + KEY_ENEMY_LEVEL + ", "
                    + KEY_HP + ", "
                    + KEY_MAXHP + ", "
                    + KEY_SHIELD + ", "
                    + KEY_DAMAGE + ", "
                    + KEY_XPOS + ", "
                    + KEY_YPOS + ", "
                    + KEY_ZPOS + ", "
                    + KEY_PREFAB + ") "

                    + "VALUES ("
                    + Convert.ToString(data["Name"]) + ", "
                    + Convert.ToInt32(data["PLevel"]) + ", "
                    + Convert.ToSingle(data["HP"]) + ", "
                    + Convert.ToSingle(data["MaxHP"]) + ", "
                    + Convert.ToSingle(data["Shield"]) + ", "
                    + Convert.ToSingle(data["Damage"]) + ", "
                    + Convert.ToSingle(data["XPos"]) + ", "
                    + Convert.ToSingle(data["YPos"]) + ", "
                    + Convert.ToSingle(data["ZPos"]) + ", "
                    + Convert.ToString(data["Prefab"]) + ")";
                #endregion
                return dbcmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertOrUpdate_EnemyGuns(List<Gun> enemyGuns)
        {
            GunPersistence _GunPersistence = new GunPersistence();
            IDbCommand dbcmd = GetDbCommand();
            var gunsFromDb = (from object[] array in GetEnemyGuns_FromSQL() 
                              select (int)array[0]).ToList();

            foreach (Gun g in enemyGuns)
            {
                try
                {
                    if (_GunPersistence.GetID(g.Name) == 0)
                        _GunPersistence.Insert(g);

                    if (!gunsFromDb.Any(id => id.Equals(g.GunID)))
                    {
                        InsertEnemyGun(dbcmd, g);
                        continue;
                    }
                    UpdateEnemyGun(dbcmd, g);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            DeleteNonExistingRelation(dbcmd, enemyGuns);
        }

        private void DeleteNonExistingRelation(IDbCommand dbcmd, List<Gun> enemyGuns)
        {
            var gunsIds = enemyGuns.Select(g => g.GunID).ToList();

            #region Delete not exisiting enemy_guns query
            dbcmd.CommandText =
                "DELETE FROM "
                + TABLE_ENEMY_GUNS
                + "WHERE " + KEY_ENEMY_ID + " = " + _EnemyId
                + $"AND NOT IN {gunsIds}";
            #endregion
            dbcmd.ExecuteNonQuery();
        }

        private void UpdateEnemyGun(IDbCommand dbcmd, Gun g)
        {
            #region Update enemy_guns query
            dbcmd.CommandText =
                "UPDATE TABLE "
                + TABLE_ENEMY_GUNS + " SET "
                + KEY_ISMAIN + $" = {g.IsMain} "
                + "WHERE " + KEY_ENEMY_ID + " = " + _EnemyId
                + "AND " + KEY_GUN_ID + "= " + g.GunID;
            #endregion
            dbcmd.ExecuteNonQuery();
        }

        private void InsertEnemyGun(IDbCommand dbcmd, Gun g)
        {
            #region Insert player_guns query
            dbcmd.CommandText =
                "INSERT INTO "
                + TABLE_ENEMY_GUNS + " ("
                + KEY_ENEMY_ID + ", "
                + KEY_GUN_ID + ", "
                + KEY_ISMAIN + ") "

                + "VALUES ("
                + _EnemyId + ", "
                + g.GunID + ", "
                + g.IsMain + ")";
            #endregion
            dbcmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Get from database the list of guns IDs and if IS_MAIN value 
        /// </summary>
        /// <returns>List of object array casteable to (int, bool)</returns>
        private List<Array> GetEnemyGuns_FromSQL()
        {
            IDbCommand dbcmd = GetDbCommand();
            List<Array> gunsInfo = new List<Array>();

            #region Select Enemies_Guns query
            dbcmd.CommandText =
                $"SELECT {KEY_GUN_ID}, " +
                $"{KEY_ISMAIN} "+
                $"FROM {TABLE_ENEMY_GUNS} " +
                $"WHERE {KEY_ENEMY_ID} = {_EnemyId} ";
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

        public object Update(Enemy enemy)
        {
            object res = null;

            Dictionary<string, object> data = enemy.Save();
            IDbTransaction transaction = BeginTransaction();
            try
            {
                IDbCommand dbcmd = GetDbCommand();

                #region Update player query
                dbcmd.CommandText =
                    "UPDATE TABLE "
                    + TABLE_ENEMIES + " SET "
                    + KEY_ENEMY_NAME + $" = {Convert.ToString(data["Name"])}, "
                    + KEY_HP + $" = {Convert.ToSingle(data["HP"])}, "
                    + KEY_MAXHP + $" = {Convert.ToSingle(data["MaxHP"])}, "
                    + KEY_SHIELD + $" = {Convert.ToSingle(data["Shield"])}, "
                    + KEY_ENEMY_LEVEL + $" = {Convert.ToInt16(data["Level"])}, "
                    + KEY_DAMAGE + $" = {Convert.ToSingle(data["Damage"])}, "
                    + KEY_XPOS + $" = {Convert.ToSingle(data["XPos"])}, "
                    + KEY_YPOS + $" = {Convert.ToSingle(data["YPos"])}, "
                    + KEY_ZPOS + $" = {Convert.ToSingle(data["ZPos"])}, "
                    + KEY_PREFAB + $" = {Convert.ToString(data["Prefab"])}"
                    + $"WHERE {KEY_ENEMY_ID} = {data["ID"]}";
                #endregion
                res = dbcmd.ExecuteScalar();

                InsertOrUpdate_EnemyGuns((List<Gun>)data["Guns"]);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return res;
        }

        public object InsertOrUpdate(Enemy e)
        {
            if (GetID(e.Name) != 0)
            {
                return Update(e);
            }
            return Insert(e);
        }

        public int GetID(object name)
        {
            IDbCommand dbcmd = GetDbCommand();

            #region Select EnemyId query
            dbcmd.CommandText =
                $"SELECT {KEY_ENEMY_ID} " +
                $"FROM {TABLE_ENEMIES} " +
                $"WHERE {KEY_ENEMY_NAME} = {(string)name}";
            #endregion
            return (int)dbcmd.ExecuteScalar();
        }

        public Dictionary<string, object> Get(int id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<Gun> enemyGuns = new List<Gun>();
            
            IDbCommand dbcmd = GetDbCommand();
            
            #region Select enemy query
            dbcmd.CommandText =
                $"SELECT * " +
                $"FROM {TABLE_ENEMIES} " +
                $"WHERE {KEY_ENEMY_ID} = {id}";
            #endregion
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
            var guns = GetEnemyGuns_FromSQL();

            foreach (object[] g in guns)
            {
                var gun = _GunPersistence.Get((int)g[0]);
                if (gun != null)
                {
                    enemyGuns.Add(gun);

                    if ((bool)g[1])
                        data.Add("MainGun", gun.GunID);
                }
            }
            #endregion
            
            data.Add("Guns", enemyGuns);
            return data;
        }

        public List<Enemy> GetAll()
        {
            IDbCommand dbcmd = GetDbCommand();

            List<Enemy> enemies = new List<Enemy>();
            dbcmd.CommandText =
                " SELECT * " +
                $"FROM {TABLE_ENEMIES}";
            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                enemies.Add((Enemy)reader);
            }
            return enemies;
        }

        public void Delete(int id)
        {
            IDbCommand dbcmd = GetDbCommand();

            dbcmd.CommandText =
                "DELETE "
                + $"FROM {TABLE_ENEMIES} "
                + $"WHERE {KEY_ENEMY_ID} = {id}";
            dbcmd.ExecuteNonQuery();
        }
    }
}