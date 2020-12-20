using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Persistence
{
    public class SQLiteManager
    {
        private const string DBName = "space_warrior_db";

        public string connection_string;
        public IDbConnection db_connection;

        public SQLiteManager()
        {
            connection_string = "URI-file:" + Application.persistentDataPath + "/" + DBName;
            Debug.Log("Connecting to: " + connection_string + "...");
            ConnectDatabase();
            Debug.Log("Connected!!");
        }

        ~SQLiteManager()
        {
            CloseConnection();
        }
        
        public void ConnectDatabase()
        {
            db_connection = new SqliteConnection(connection_string);
            db_connection.Open();
        }

        public void CloseConnection()
        {
            if (db_connection != null)
                db_connection.Close();
        }

        public IDbCommand GetDbCommand()
        {
            return db_connection.CreateCommand();
        }

        public IDbTransaction BeginTransaction()
        {
            return db_connection.BeginTransaction();
        }
    }
}