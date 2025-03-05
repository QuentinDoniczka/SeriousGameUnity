// _Project/Database/SqlManager.cs
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Project.Database.Services;

namespace Project.Database
{
    public class SqlManager : MonoBehaviour
    {
        private SqliteConnection _connection;
        private JsonSchemaAnalyzer _schemaAnalyzer;
        private SqlTableBuilder _tableBuilder;
        private bool _isInitialized;
        private string _currentLevelJson;

        [System.Serializable]
        public class LevelData
        {
            public Dictionary<string, TableData> tables;
            public List<TaskData> tasks;
        }

        [System.Serializable]
        public class TableData
        {
            public Dictionary<string, string> columns;
            public List<Dictionary<string, object>> rows;
        }

        [System.Serializable]
        public class TaskData
        {
            public int id;
            public string name;
            public string description;
            public string difficulty;
            public string query;
            public string hint;
            public List<string> allowed_commands;
            public List<Dictionary<string, object>> expected;
        }

        private void Awake()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (_isInitialized) return;

            string dbPath = Path.Combine(Application.temporaryCachePath, "game.db");
            string connectionString = $"URI=file:{dbPath}";
            
            try
            {
                _connection = new SqliteConnection(connectionString);
                _schemaAnalyzer = new JsonSchemaAnalyzer();
                _tableBuilder = new SqlTableBuilder(_connection);
                _isInitialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize database: {e.Message}");
            }
        }

        public void LoadLevelData(string levelName)
        {
            string resourcePath = $"LevelData/{levelName}";
            TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

            if (jsonFile == null)
            {
                Debug.LogError($"Failed to load level file: {levelName}");
                return;
            }

            try
            {
                InitializeDatabaseFromJson(jsonFile.text);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize level data: {e.Message}");
            }
        }

        public void InitializeDatabaseFromJson(string jsonContent)
        {
            if (!_isInitialized)
            {
                Debug.LogError("SqlManager not initialized");
                return;
            }

            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("JSON content is empty");
                return;
            }

            _currentLevelJson = jsonContent;

            try
            {
                _connection.Open();
                var schemas = _schemaAnalyzer.AnalyzeJsonStructure(jsonContent);
                _tableBuilder.CreateTables(schemas, jsonContent);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error initializing database from JSON: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public LevelData GetCurrentLevelData()
        {
            if (string.IsNullOrEmpty(_currentLevelJson))
            {
                Debug.LogError("No level data loaded");
                return null;
            }
    
            try
            {
                return JsonUtility.FromJson<LevelData>(_currentLevelJson);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing level data: {e.Message}");
                return null;
            }
        }

        public void ExecuteQuery(string query, System.Action<SqliteDataReader> onResult)
        {
            if (!_isInitialized)
            {
                Debug.LogError("SqlManager not initialized");
                return;
            }

            if (string.IsNullOrEmpty(query))
            {
                Debug.LogError("Query is empty");
                return;
            }

            try
            {
                _connection.Open();
                using var command = new SqliteCommand(query, _connection);
                using var reader = command.ExecuteReader();
                onResult?.Invoke(reader);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error executing query: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        private void OnDestroy()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
            _connection?.Dispose();
        }
    }
}