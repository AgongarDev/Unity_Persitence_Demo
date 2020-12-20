using System;
using System.Data;

namespace Persistence
{
    class InitDataBase : SQLiteManager
    {
        #region SQL constants
        #region Common strings
        private const string KEY_NAME = "Name";
        private const string KEY_PREFAB = "Prefab";
        private const string KEY_DAMAGE = "Damage";
        private const string KEY_SHIELD = "Shield";
        private const string KEY_MAXHP = "Max_HP";
        private const string KEY_LEVEL = "Level";
        private const string KEY_XPOS = "X_pos";
        private const string KEY_YPOS = "Y_pos";
        private const string KEY_ZPOS = "Z_pos";
        private const string KEY_HP = "HP";
        #endregion

        #region Gameplay strings
        private const string TABLE_GAMES = "Gameplays";
        private const string KEY_GAME_ID = "Gameplay_ID";
        private const string KEY_GAME_DATE = "Game_Date";
        private const string KEY_PLAYED_TIME = "Played_Time";
        private const string KEY_SCORE = "Score";
        private const string KEY_GAME_LEVEL = "GameLevel";
        #endregion

        #region PlayerState strings
        private const string TABLE_PLAYER = "Player_State";
        private const string KEY_PLAYER_ID = "Player_ID";
        #endregion

        #region Enemy strings
        private const string TABLE_ENEMIES = "PowerUps";
        private const string KEY_ENEMY_ID = "Enemy_ID";
        #endregion

        #region Gun strings
        private const string TABLE_GUNS = "Guns";
        private const string KEY_GUN_ID = "Gun_ID";
        #endregion

        #region PowerUp strings
        private const string TABLE_POWERUPS = "PowerUps";
        private const string KEY_POWERUP_ID = "PowerUp_ID";
        private const string KEY_POWERUP_VALUE = "Value";
        private const string KEY_POWERUP_TARGET = "Target";
        #endregion

        #region Relations
        private const string KEY_ISMAIN = "Main";

        private const string TABLE_PLAYER_GUNS = "Player_Guns";
        private const string TABLE_PLAYER_PUPS = "Player_PowerUps";
        private const string TABLE_ENEMY_GUNS = "Enemy_Guns";

        private const string TABLE_GAME_ENEMIES = "Game_Enemies";
        #endregion
        #endregion

        public InitDataBase() : base()
        {
            IDbCommand dbcmd = GetDbCommand();

            CreateTable_PlayerState(dbcmd);

            CreateTable_Games(dbcmd);

            CreateTable_GamesEnemies(dbcmd);

            CreteTable_Enemies(dbcmd);

            CreateTable_Guns(dbcmd);

            CreateTable_PowerUps(dbcmd);

            CreateTable_PlayerGuns(dbcmd);

            CreateTable_PlayerPowerUps(dbcmd);

            CreateTable_EnemiesGuns(dbcmd);
        }

        private void CreateTable_Games(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_GAMES + " ("
                + KEY_GAME_ID + " INTEGER AUTOINCREMENT, "
                + KEY_PLAYER_ID + "INTEGER NOT NULL, "
                + KEY_GAME_DATE + " DATE NOT NULL, "
                + KEY_PLAYED_TIME + "BIGINT, "
                + KEY_GAME_LEVEL + "SMALLINT NOT NULL DEFAULT 1, "
                + KEY_SCORE + "MEDIUMINT,"
                + $" PRIMARY KEY ({KEY_GAME_ID}, {KEY_PLAYER_ID}, {KEY_GAME_DATE})) ";
            dbcmd.ExecuteNonQuery();
        }

        private static void CreateTable_GamesEnemies(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_GAME_ENEMIES + " ( "
                + KEY_GAME_ID + " INTEGER NOT NULL, "
                + KEY_ENEMY_ID + " INTEGER NOT NULL, "
                + "PRIMARY KEY (" + KEY_GAME_ID + ", " + KEY_ENEMY_ID + "),"
                + $"FOREIGN KEY ({KEY_PLAYER_ID}) "
                + $"REFERENCES {TABLE_GAMES} ({KEY_GAME_ID}) "
                + "ON DELETE CASCADE"
                + "ON UPDATE CASCADE"
                + $"FOREIGN KEY({ KEY_ENEMY_ID}) "
                + $"REFERENCES {TABLE_ENEMIES} ({KEY_ENEMY_ID}) "
                + "ON DELETE CASCADE"
                + "ON UPDATE CASCADE";
            dbcmd.ExecuteNonQuery();
        }

        private void CreateTable_PlayerState(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_PLAYER + " ( "
                + KEY_PLAYER_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, "
                + KEY_NAME + " TEXT, "
                + KEY_LEVEL + " INTEGER DEFAULT 1,"
                + KEY_HP + " REAL, "
                + KEY_MAXHP + " REAL, "
                + KEY_SHIELD + " REAL, "
                + KEY_DAMAGE + " REAL, "
                + KEY_XPOS + " REAL, "
                + KEY_YPOS + " REAL, "
                + KEY_ZPOS + " REAL )";
            dbcmd.ExecuteNonQuery();
        }

        private static void CreateTable_PlayerGuns(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_PLAYER_GUNS + " ( "
                + KEY_PLAYER_ID + " INTEGER NOT NULL, "
                + KEY_GUN_ID + " INTEGER NOT NULL, "
                + KEY_ISMAIN + " BOOLEAN, "
                + "PRIMARY KEY (" + KEY_PLAYER_ID + ", " + KEY_GUN_ID + ")"
                + $"FOREIGN KEY({ KEY_PLAYER_ID}) "
                + $"REFERENCES {TABLE_PLAYER} ({KEY_PLAYER_ID}) "
                + "ON DELETE CASCADE"
                + "ON UPDATE CASCADE"
                + $"FOREIGN KEY({ KEY_GUN_ID}) "
                + $"REFERENCES {TABLE_GUNS} ({KEY_GUN_ID}) "
                + "ON DELETE CASCADE"
                + "ON UPDATE CASCADE";
            dbcmd.ExecuteNonQuery();
        }

        private static void CreateTable_PlayerPowerUps(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_PLAYER_PUPS + " ( "
                + KEY_PLAYER_ID + " INTEGER NOT NULL, "
                + KEY_POWERUP_ID + " INTEGER NOT NULL, "
                + "PRIMARY KEY (" + KEY_PLAYER_ID + ", " + KEY_POWERUP_ID + "),"
                + $"FOREIGN KEY ({KEY_PLAYER_ID}) "
                + $"REFERENCES {TABLE_PLAYER} ({KEY_PLAYER_ID}) "
                + "ON DELETE CASCADE"
                + "ON UPDATE CASCADE"
                + $"FOREIGN KEY({ KEY_POWERUP_ID}) "
                + $"REFERENCES {TABLE_POWERUPS} ({KEY_POWERUP_ID}) "
                + "ON DELETE CASCADE"
                + "ON UPDATE CASCADE";
            dbcmd.ExecuteNonQuery();
        }

        private static void CreateTable_Guns(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_GUNS + " ( "
                + KEY_GUN_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, "
                + KEY_NAME + "TEXT, "
                + KEY_DAMAGE + "REAL, "
                + KEY_PREFAB + "TEXT ) ";
            dbcmd.ExecuteNonQuery();
        }

        private void CreateTable_PowerUps(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_POWERUPS + " ( "
                + KEY_POWERUP_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, "
                + KEY_NAME + "TEXT, "
                + KEY_POWERUP_VALUE + "REAL, "
                + KEY_POWERUP_TARGET + "TEXT, "
                + KEY_PREFAB + "TEXT )";
            dbcmd.ExecuteNonQuery();
        }

        private void CreteTable_Enemies(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
                + TABLE_ENEMIES + " ( "
                + KEY_ENEMY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT, "
                + KEY_NAME + " TEXT, "
                + KEY_LEVEL + " INTEGER DEFAULT 1,"
                + KEY_HP + " REAL, "
                + KEY_MAXHP + " REAL, "
                + KEY_SHIELD + " REAL, "
                + KEY_DAMAGE + " REAL, "
                + KEY_XPOS + " REAL, "
                + KEY_YPOS + " REAL, "
                + KEY_ZPOS + " REAL ) ";
            dbcmd.ExecuteNonQuery();
        }

        private void CreateTable_EnemiesGuns(IDbCommand dbcmd)
        {
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS "
               + TABLE_ENEMY_GUNS + " ( "
               + KEY_ENEMY_ID + " INTEGER NOT NULL, "
               + KEY_GUN_ID + " INTEGER NOT NULL, "
               + KEY_ISMAIN + " BOOLEAN, "
               + "PRIMARY KEY (" + KEY_ENEMY_ID + ", " + KEY_GUN_ID + "),"
               + $"FOREIGN KEY ({KEY_ENEMY_ID}) "
               + $"REFERENCES {TABLE_ENEMIES} ({KEY_ENEMY_ID}) "
               + "ON DELETE CASCADE"
               + "ON UPDATE CASCADE"
               + $"FOREIGN KEY ({ KEY_GUN_ID}) "
               + $"REFERENCES {TABLE_GUNS} ({KEY_GUN_ID}) "
               + "ON DELETE CASCADE"
               + "ON UPDATE CASCADE )";
            dbcmd.ExecuteNonQuery();
        }
    }
}
