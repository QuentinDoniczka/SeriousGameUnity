// Project/UI/HUD/SQLQueryData.cs
using System.Collections.Generic;

namespace Project.UI.HUD
{
    [System.Serializable]
    public class SqlQueryData
    {
        public int TaskId { get; set; }
        public string QueryText { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string Difficulty { get; set; }
        public string Hint { get; set; }
        public List<string> AllowedCommands { get; set; }
        
        public SqlQueryData(int taskId, string queryText, string taskName, string taskDescription, 
            string difficulty, string hint, List<string> allowedCommands)
        {
            TaskId = taskId;
            QueryText = queryText;
            TaskName = taskName;
            TaskDescription = taskDescription;
            Difficulty = difficulty;
            Hint = hint;
            AllowedCommands = allowedCommands;
        }
    }
}