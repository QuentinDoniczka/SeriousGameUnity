using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;

namespace Project.Database.Services
{
    /// <summary>
    /// Provides methods to execute SQL queries and manage database operations in Unity
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Handles SQL query execution with proper connection management</item>
    /// <item>Provides methods for both data retrieval and modification operations</item>
    /// <item>Includes utilities for database structure management (listing and dropping tables)</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class SqlQueryService
    {
        private readonly SqlConnectionService _connectionService;

        public SqlQueryService(SqlConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public void ExecuteQuery(string query, System.Action<SqliteDataReader> onResult)
        {
            if (!_connectionService.IsInitialized)
            {
                Debug.LogError("Connection not initialized");
                return;
            }

            if (string.IsNullOrEmpty(query))
            {
                Debug.LogError("Query is empty");
                return;
            }

            try
            {
                _connectionService.OpenConnection();
                using var command = new SqliteCommand(query, _connectionService.Connection);
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
                _connectionService.CloseConnection();
            }
        }

        public void ExecuteNonQuery(string query)
        {
            if (!_connectionService.IsInitialized)
            {
                Debug.LogError("Connection not initialized");
                return;
            }

            if (string.IsNullOrEmpty(query))
            {
                Debug.LogError("Query is empty");
                return;
            }

            try
            {
                _connectionService.OpenConnection();
                using var command = new SqliteCommand(query, _connectionService.Connection);
                command.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error executing non-query: {e.Message}");
                throw;
            }
            finally
            {
                _connectionService.CloseConnection();
            }
        }

        public List<string> GetAllTableNames()
        {
            List<string> tables = new List<string>();
            string query = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
            
            ExecuteQuery(query, reader => {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            });
            
            return tables;
        }

        public void DropAllTables()
        {
            try
            {
                List<string> tables = GetAllTableNames();
                
                foreach (string table in tables)
                {
                    ExecuteNonQuery($"DROP TABLE IF EXISTS {table}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error dropping tables: {e.Message}");
            }
        }
    }
}