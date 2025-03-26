using UnityEngine;
using Mono.Data.Sqlite;
using Project.Database.Services;
using Project.Database.Models;

namespace Project.Database
{
    public class SqlManager : MonoBehaviour
    {
        private SqlConnectionService _connectionService;
        private SqlTableBuilder _tableBuilder;
        private SqlDebugger _sqlDebugger;
        private SqlQueryService _queryService;
        private LevelDataService _levelDataService;

        private void Awake()
        {
            InitializeServices();
        }

        private void InitializeServices()
        {
            _connectionService = new SqlConnectionService();
            
            if (_connectionService.IsInitialized)
            {
                _tableBuilder = new SqlTableBuilder(_connectionService.Connection);
                _sqlDebugger = new SqlDebugger(_connectionService.Connection);
                _queryService = new SqlQueryService(_connectionService);
                _levelDataService = new LevelDataService();
            }
            else
            {
                Debug.LogError("Failed to initialize database services");
            }
        }

        public void LoadLevelData(string levelName)
        {
            string jsonContent = _levelDataService.LoadLevelJson(levelName);
            
            if (!string.IsNullOrEmpty(jsonContent))
            {
                InitializeDatabaseFromJson(jsonContent);
            }
        }

        public void InitializeDatabaseFromJson(string jsonContent)
        {
            if (!_connectionService.IsInitialized)
            {
                Debug.LogError("SqlManager not initialized");
                return;
            }

            if (!_levelDataService.ValidateLevelData(jsonContent))
            {
                Debug.LogError("Invalid level data");
                return;
            }

            _levelDataService.SetLevelJson(jsonContent);

            try
            {
                _connectionService.OpenConnection();
                
                _queryService.DropAllTables();
                _tableBuilder.CreateTables(jsonContent);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error initializing database from JSON: {e.Message}");
                throw;
            }
            finally
            {
                _connectionService.CloseConnection();
                
                //DebugAllTables();
            }
        }

        public LevelData GetCurrentLevelData()
        {
            return _levelDataService.GetCurrentLevelData();
        }

        public void ExecuteQuery(string query, System.Action<SqliteDataReader> onResult)
        {
            _queryService.ExecuteQuery(query, onResult);
        }
        
        public void DebugAllTables()
        {
            if (!_connectionService.IsInitialized)
            {
                Debug.LogError("SqlManager not initialized");
                return;
            }
            
            _sqlDebugger.DebugAllTables();
        }
        
        public void DebugTable(string tableName)
        {
            if (!_connectionService.IsInitialized)
            {
                Debug.LogError("SqlManager not initialized");
                return;
            }
            
            _sqlDebugger.DebugTable(tableName);
        }

        private void OnDestroy()
        {
            _connectionService?.Dispose();
        }
    }
}