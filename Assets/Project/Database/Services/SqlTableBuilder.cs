// _Project/Database/Services/SqlTableBuilder.cs
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using Project.Database.Models;

namespace Project.Database.Services
{
    public class SqlTableBuilder
    {
        private readonly SqliteConnection _connection;

        public SqlTableBuilder(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void CreateTables(Dictionary<string, TableSchema> schemas, string jsonContent)
        {
            var jsonData = JObject.Parse(jsonContent);
            
            foreach (var schema in schemas.Values)
            {
                CreateTableFromSchema(schema);
                var tableData = FindTableDataInJson(jsonData, schema.TableName);
                if (tableData != null)
                {
                    PopulateTable(schema, tableData["units"]);
                }
            }
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

        private void PopulateTable(TableSchema schema, JToken units)
        {
            foreach (var unit in units)
            {
                var insertSql = GenerateInsertStatement(schema, unit);
                ExecuteNonQuery(insertSql);
            }
        }

        private string GenerateInsertStatement(TableSchema schema, JToken unit)
        {
            var columns = new List<string>();
            var values = new List<string>();

            foreach (JProperty prop in unit)
            {
                if (prop.Name == "position")
                {
                    columns.Add("position_x");
                    columns.Add("position_y");
                    values.Add(unit["position"]["x"].ToString());
                    values.Add(unit["position"]["y"].ToString());
                }
                else
                {
                    columns.Add(prop.Name);
                    values.Add(FormatSqlValue(prop.Value));
                }
            }

            return $"INSERT INTO {schema.TableName} ({string.Join(", ", columns)}) " +
                   $"VALUES ({string.Join(", ", values)})";
        }

        private JToken FindTableDataInJson(JObject jsonData, string tableName)
        {
            foreach (var prop in jsonData.Properties())
            {
                if (prop.Value["tableName"]?.ToString() == tableName)
                {
                    return prop.Value;
                }
            }
            return null;
        }

        private string FormatSqlValue(JToken value)
        {
            return value.Type switch
            {
                JTokenType.String => $"'{value}'",
                JTokenType.Boolean => (bool)value ? "1" : "0",
                _ => value.ToString()
            };
        }

        private void ExecuteNonQuery(string sql)
        {
            using var cmd = new SqliteCommand(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error executing SQL: {sql}\nError: {e.Message}");
            }
        }
    }
}