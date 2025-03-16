using UnityEngine;
using Mono.Data.Sqlite;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace Project.Database.Services
{
    public class SqlDebugger
    {
        private readonly SqliteConnection _connection;

        public SqlDebugger(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void DebugAllTables()
        {
            try
            {
                // Ouvrir la connexion si nécessaire
                bool wasConnectionClosed = false;
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    wasConnectionClosed = true;
                }
                
                List<string> tables = new List<string>();
                string query = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                
                using (var command = new SqliteCommand(query, _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
                
                Debug.Log($"Tables in database: {tables.Count}");
                foreach (string table in tables)
                {
                    Debug.Log($"Debugging table: {table}");
                    DebugTable(table);
                }
                
                // Fermer la connexion si nous l'avons ouverte
                if (wasConnectionClosed)
                {
                    _connection.Close();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error getting table names: {e.Message}");
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public void DebugTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                Debug.LogError("Table name is empty");
                return;
            }

            bool wasConnectionClosed = false;
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    wasConnectionClosed = true;
                }
                
                string query = $"SELECT * FROM {tableName}";
                using var command = new SqliteCommand(query, _connection);
                using var reader = command.ExecuteReader();
                
                StringBuilder header = new StringBuilder();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    header.Append(reader.GetName(i));
                    if (i < reader.FieldCount - 1)
                        header.Append(" | ");
                }
                Debug.Log($"Table: {tableName}");
                Debug.Log(header.ToString());
                
                while (reader.Read())
                {
                    StringBuilder row = new StringBuilder();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Append(reader.GetValue(i)?.ToString() ?? "NULL");
                        if (i < reader.FieldCount - 1)
                            row.Append(" | ");
                    }
                    Debug.Log(row.ToString());
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error debugging table {tableName}: {e.Message}");
            }
            finally
            {
                // Fermer la connexion seulement si nous l'avons ouverte
                if (wasConnectionClosed && _connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
    }
}