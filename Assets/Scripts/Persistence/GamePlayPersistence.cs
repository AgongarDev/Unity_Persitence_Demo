using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    class GamePlayPersistence : SQLiteManager, IPersistent<Gameplay>
    {
        private const string TABLE_GAMES = "Gameplays";
        private const string KEY_GAME_ID = "Gameplay_ID";
        private const string KEY_GAME_DATE = "Game_Date";
        private const string KEY_PLAYED_TIME = "Played_Time";
        private const string KEY_SCORE = "Score";
        private const string KEY_GAME_LEVEL = "GameLevel";

        private const string TABLE_GAME_ENEMIES = "Game_Enemies";
        private const string KEY_GAME_ENEMY_ID = "Game_Enemy_ID";

        private const string KEY_PLAYER_ID = "Player_ID";
        private const string KEY_ENEMY_ID = "Enemy_ID";

        private EnemiesPersistence _EnemiesPersistence;
        private int _GameId;

        public GamePlayPersistence() : base() { }
        
        public object Insert(Gameplay g)
        {
            Dictionary<string, object> data = g.Save();
            IDbTransaction transaction = BeginTransaction();
            try
            {
                _GameId = (int)InsertGamePlayData(data);

                InsertOrUpdate_GameEnemies((List<Enemy>)data["Enemies"]);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return _GameId;
        }

        private object InsertGamePlayData(Dictionary<string, object> data)
        {
            IDbCommand dbcmd = GetDbCommand();
            try
            {
                #region Insert player query
                dbcmd.CommandText =
                    "INSERT INTO "
                    + TABLE_GAMES + " ("
                    + KEY_PLAYER_ID + ", "
                    + KEY_GAME_DATE + ", "
                    + KEY_PLAYED_TIME + ", "
                    + KEY_GAME_LEVEL + ", "
                    + KEY_SCORE + ") "

                    + "VALUES ("
                    + Convert.ToInt32(data["PState"]) + ", "
                    + Convert.ToDateTime(data["Date"]) + ", "
                    + (TimeSpan)data["Time"] + ", "
                    + Convert.ToInt16(data["Level"]) + ", "
                    + Convert.ToInt32(data["Score"]) + ") ";
                #endregion
                return dbcmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertOrUpdate_GameEnemies(List<Enemy> gameEnemies)
        {
            EnemiesPersistence _EnemyPersistence = new EnemiesPersistence();
            IDbCommand dbcmd = GetDbCommand();
            var enemiesFromDb = GetGameEnemies_FromSQL();

            foreach (Enemy e in gameEnemies)
            {
                try
                {
                    if (_EnemyPersistence.GetID(e.Name) == 0)
                        _EnemyPersistence.Insert(e);

                    if (enemiesFromDb.Any(id => id.Equals(e.EnemyID)))
                    {
                        continue;
                    }
                    InsertGameEnemy(dbcmd, e);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            DeleteNonExistingRelation(dbcmd, gameEnemies);
        }

        private void DeleteNonExistingRelation(IDbCommand dbcmd, List<Enemy> gameEnemies)
        {
            var enemiesIds = gameEnemies.Select(e => e.EnemyID).ToList();

            #region Delete not exisiting game_enemies query
            dbcmd.CommandText =
                "DELETE FROM "
                + TABLE_GAME_ENEMIES
                + "WHERE " + KEY_GAME_ID + " = " + _GameId
                + $"AND NOT IN {gameEnemies}";
            #endregion
            dbcmd.ExecuteNonQuery();
        }

        private void InsertGameEnemy(IDbCommand dbcmd, Enemy e)
        {
            #region Insert game_enemies query
            dbcmd.CommandText =
                "INSERT INTO "
                + TABLE_GAME_ENEMIES + " ("
                + KEY_GAME_ID + ", "
                + KEY_ENEMY_ID + ") "

                + "VALUES ("
                + _GameId + ", "
                + e.EnemyID + ") ";
            #endregion
            dbcmd.ExecuteNonQuery();
        }

        private List<int> GetGameEnemies_FromSQL()
        {
            IDbCommand dbcmd = GetDbCommand();
            List<int> enemiesid = new List<int>();

            #region Select Enemies_Guns query
            dbcmd.CommandText =
                $"SELECT {KEY_ENEMY_ID} " +
                $"FROM {TABLE_GAME_ENEMIES} " +
                $"WHERE {KEY_GAME_ID} = {_GameId} ";
            #endregion
            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                enemiesid.Add((int)reader[0]);
            }
            return enemiesid;
        }

        public object Update(Gameplay game)
        {
            object res = null;

            Dictionary<string, object> data = game.Save();
            IDbTransaction transaction = BeginTransaction();
            try
            {
                IDbCommand dbcmd = GetDbCommand();

                #region Update player query
                dbcmd.CommandText =
                    "UPDATE TABLE "
                    + TABLE_GAMES + " SET "
                    + KEY_PLAYER_ID + $" = {Convert.ToInt32(data["PState"])} "
                    + KEY_GAME_DATE + $" = {Convert.ToDateTime(data["Date"])}, "
                    + KEY_PLAYED_TIME + $" = {Convert.ToInt64(data["Time"])}, "
                    + KEY_GAME_LEVEL + $" = {Convert.ToInt16(data["Level"])}, "
                    + KEY_SCORE + $" = {Convert.ToInt32(data["Score"])}) "
                    + $"WHERE {KEY_GAME_ID} = {data["ID"]}";
                #endregion
                res = dbcmd.ExecuteScalar();

                InsertOrUpdate_GameEnemies((List<Enemy>)data["Enemies"]);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return res;
        }

        public object InsertOrUpdate(Gameplay g)
        {
            if (GetID(g.PlayerState) != 0)
            {
                return Update(g);
            }
            return Insert(g);
        }

        public int GetID(object playerStateId)
        {
            IDbCommand dbcmd = GetDbCommand();

            #region Select EnemyId query
            dbcmd.CommandText =
                $"SELECT {KEY_GAME_ID} " +
                $"FROM {TABLE_GAMES} " +
                $"WHERE {KEY_PLAYER_ID} = {(int)playerStateId}";
            #endregion
            return (int)dbcmd.ExecuteScalar();
        }

        public Gameplay Get(int id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<Enemy> gameEnemies = new List<Enemy>();
            
            IDbCommand dbcmd = GetDbCommand();

            #region Select player query
            dbcmd.CommandText =
                $"SELECT * " +
                $"FROM {TABLE_GAMES} " +
                $"WHERE {KEY_GAME_ID} = {id}";
            #endregion
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                data = new Dictionary<string, object>()
                {
                    { "ID", reader[0] },
                    { "PState", reader[1] },
                    { "Date", reader[2] },
                    { "Time", reader[3] },
                    { "Level", reader[4] },
                    { "Score", reader[5] },
                };
            }

            data.Add("Enemies", GetGameEnemies_FromSQL());
            Gameplay game = new Gameplay();
            game.Load(data);
            return game;
        }

        public List<Gameplay> GetAll()
        {
            IDbCommand dbcmd = GetDbCommand();

            List<Gameplay> games = new List<Gameplay>();
            dbcmd.CommandText =
                " SELECT * " +
                $"FROM {TABLE_GAMES}";
            var reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                games.Add((Gameplay)reader);
            }
            return games;
        }

        public void Delete(int id)
        {
            IDbCommand dbcmd = GetDbCommand();

            dbcmd.CommandText =
                "DELETE "
                + $"FROM {TABLE_GAMES} "
                + $"WHERE {KEY_GAME_ID} = {id}";
            dbcmd.ExecuteNonQuery();
        }
    }
}
