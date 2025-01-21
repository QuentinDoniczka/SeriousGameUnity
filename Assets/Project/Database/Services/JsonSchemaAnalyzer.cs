// _Project/Database/Services/JsonSchemaAnalyzer.cs
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Project.Database.Models;

namespace Project.Database.Services
{
    public class JsonSchemaAnalyzer
    {
        private readonly Dictionary<string, TableSchema> _schemas;

        public JsonSchemaAnalyzer()
        {
            _schemas = new Dictionary<string, TableSchema>();
        }

        public Dictionary<string, TableSchema> AnalyzeJsonStructure(string jsonContent)
        {
            var jsonData = JObject.Parse(jsonContent);
            
            foreach (var property in jsonData.Properties())
            {
                AnalyzeTable(property.Name, property.Value);
            }

            return _schemas;
        }

        private void AnalyzeTable(string propertyName, JToken tableData)
        {
            if (!tableData.HasValues || tableData["tableName"] == null || tableData["units"] == null)
            {
                return;
            }

            string tableName = tableData["tableName"].ToString();
            var schema = new TableSchema(tableName);
            
            var firstUnit = tableData["units"].First;
            foreach (JProperty column in firstUnit)
            {
                if (column.Name == "position")
                {
                    schema.Columns.Add("position_x", "REAL");
                    schema.Columns.Add("position_y", "REAL");
                    continue;
                }

                schema.Columns.Add(column.Name, DetermineColumnType(column.Value.Type));
            }

            _schemas.Add(propertyName, schema);
        }

        private string DetermineColumnType(JTokenType jsonType)
        {
            return jsonType switch
            {
                JTokenType.Integer => "INTEGER",
                JTokenType.Float => "REAL",
                JTokenType.Boolean => "INTEGER",
                _ => "TEXT"
            };
        }
    }
}
