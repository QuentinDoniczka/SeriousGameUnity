using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

namespace Project.Database.Services
{
    public class SqlConnectionService
    {
        private SqliteConnection _connection;
        private bool _isInitialized;

        public SqliteConnection Connection => _connection;
        public bool IsInitialized => _isInitialized;

        public SqlConnectionService()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            string dbPath = Path.Combine(Application.temporaryCachePath, "game.db");
            string connectionString = $"URI=file:{dbPath}";
            
            try
            {
                _connection = new SqliteConnection(connectionString);
                _isInitialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize database connection: {e.Message}");
            }
        }

        public bool OpenConnection()
        {
            if (!_isInitialized)
            {
                Debug.LogError("Connection not initialized");
                return false;
            }

            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to open connection: {e.Message}");
                return false;
            }
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public bool EnsureConnectionOpen()
        {
            return _connection.State == ConnectionState.Open || OpenConnection();
        }

        public void Dispose()
        {
            CloseConnection();
            _connection?.Dispose();
            _isInitialized = false;
        }
    }
}