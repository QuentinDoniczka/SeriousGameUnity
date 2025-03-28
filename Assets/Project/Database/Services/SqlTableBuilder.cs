using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using Project.Database.Models;

namespace Project.Database.Services
{
    /// <summary>
    /// Handles the creation and population of SQLite tables from JSON configuration
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Creates database tables based on JSON structure</item>
    /// <item>Generates column definitions from JSON specifications</item>
    /// <item>Populates tables with initial data if provided</item>
    /// <item>Manages SQLite connection states automatically</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class SqlTableBuilder
    {
        private readonly SqliteConnection _connection;

        public SqlTableBuilder(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void CreateTables(string jsonContent)
        {
            var jsonData = JObject.Parse(jsonContent);
            var tables = jsonData["tables"];

            if (tables == null)
            {
                Debug.LogError("No 'tables' object found in JSON");
                return;
            }
            
            bool connectionWasOpened = EnsureConnectionOpen();

            try
            {
                foreach (JProperty tableProperty in tables.Children<JProperty>())
                {
                    string tableName = tableProperty.Name;
                    JObject tableData = (JObject)tableProperty.Value;
                    
                    var schema = CreateSchemaFromJson(tableName, tableData["columns"]);
                    
                    CreateTableFromSchema(schema);
                    
                    if (tableData["rows"] != null)
                    {
                        PopulateTable(schema, tableData["rows"]);
                    }
                }
            }
            finally
            {
                if (connectionWasOpened && _connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        private TableSchema CreateSchemaFromJson(string tableName, JToken columnsData)
        {
            var schema = new TableSchema(tableName);
            
            foreach (JProperty column in columnsData.Children<JProperty>())
            {
                schema.Columns.Add(column.Name, column.Value.ToString());
            }
            
            return schema;
        }

        private void CreateTableFromSchema(TableSchema schema)
        {
            var builder = new StringBuilder();
            builder.Append($"CREATE TABLE IF NOT EXISTS {schema.TableName} (");

            var columnDefinitions = new List<string>();
            foreach (var column in schema.Columns)
            {
                columnDefinitions.Add($"{column.Key} {column.Value}");
            }

            builder.Append(string.Join(", ", columnDefinitions));
            builder.Append(")");

            ExecuteNonQuery(builder.ToString());
        }

        private void PopulateTable(TableSchema schema, JToken rows)
        {
            foreach (var row in rows)
            {
                var insertSql = GenerateInsertStatement(schema, row);
                ExecuteNonQuery(insertSql);
            }
        }

        private string GenerateInsertStatement(TableSchema schema, JToken row)
        {
            var columns = new List<string>();
            var values = new List<string>();

            foreach (JProperty prop in row.Children<JProperty>())
            {
                columns.Add(prop.Name);
                values.Add(FormatSqlValue(prop.Value));
            }

            return $"INSERT INTO {schema.TableName} ({string.Join(", ", columns)}) " +
                   $"VALUES ({string.Join(", ", values)})";
        }

        private string FormatSqlValue(JToken value)
        {
            if (value == null || value.Type == JTokenType.Null)
                return "NULL";
                
            return value.Type switch
            {
                JTokenType.String => $"'{value.ToString().Replace("'", "''")}'",
                JTokenType.Boolean => (bool)value ? "1" : "0",
                _ => value.ToString()
            };
        }

        private bool EnsureConnectionOpen()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                return true;
            }
            return false;
        }

        private void ExecuteNonQuery(string sql)
        {
            try
            {
                using var cmd = new SqliteCommand(sql, _connection);
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error executing SQL: {sql}\nError: {e.Message}");
                throw;
            }
        }
    }
}