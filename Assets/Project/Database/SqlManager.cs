// _Project/Database/SqlManager.cs
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using Project.Database.Services;

namespace Project.Database
{
    public class SqlManager : MonoBehaviour
    {
        private SqliteConnection _connection;
        private JsonSchemaAnalyzer _schemaAnalyzer;
        private SqlTableBuilder _tableBuilder;
        private bool _isInitialized;

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
        public void DebugAllTables()
        {
            string[] tableNames = { "blue_army", "red_army" };
    
            foreach (string tableName in tableNames)
            {
                Debug.Log($"\nContenu de la table {tableName}:");
                ExecuteQuery(
                    $"SELECT * FROM {tableName}",
                    reader =>
                    {
                        // Afficher les noms des colonnes
                        string columns = "";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns += reader.GetName(i) + " | ";
                        }
                        Debug.Log($"Colonnes: {columns}");

                        // Afficher les données
                        while (reader.Read())
                        {
                            string rowData = "";
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rowData += $"{reader.GetName(i)}: {reader[i]} | ";
                            }
                            Debug.Log(rowData);
                        }
                    }
                );
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