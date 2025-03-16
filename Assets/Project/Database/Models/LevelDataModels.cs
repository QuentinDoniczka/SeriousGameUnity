using System.Collections.Generic;

namespace Project.Database.Models
{
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
        public List<string> allowedCommands;
        public List<Dictionary<string, object>> expected;
    }
}