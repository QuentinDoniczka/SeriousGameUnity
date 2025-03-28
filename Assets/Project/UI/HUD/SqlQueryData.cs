// Project/UI/HUD/SQLQueryData.cs
using System.Collections.Generic;

namespace Project.UI.HUD
{
    /// <summary>
    /// Represents SQL query data for educational tasks in the application.
    /// </summary>
    /// <remarks>
    /// This class provides a data structure for SQL query challenges:
    /// <list type="bullet">
    /// <item>Stores task information including ID, name, description, and difficulty</item>
    /// <item>Contains the SQL query text to be executed</item>
    /// <item>Manages hints and allowed SQL commands for guided learning</item>
    /// <item>Supports serialization for data persistence</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
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