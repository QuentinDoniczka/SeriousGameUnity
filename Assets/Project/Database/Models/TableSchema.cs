// _Project/Database/Models/TableSchema.cs
using System.Collections.Generic;

namespace Project.Database.Models
{
    public class TableSchema
    {
        public string TableName { get; set; }
        public Dictionary<string, string> Columns { get; set; }
        
        public TableSchema(string tableName)
        {
            TableName = tableName;
            Columns = new Dictionary<string, string>();
        }
    }
}