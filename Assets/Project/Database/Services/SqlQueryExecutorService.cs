using UnityEngine;
using Project.Core.Service;
using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using Project.Game.Characters;

namespace Project.Database.Services
{
    /// <summary>
    /// Service for executing SQL queries against the game database and processing character data results.
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Implements a singleton pattern for database access</item>
    /// <item>Provides methods to execute SQL queries and extract character data</item>
    /// <item>Handles the conversion between SQL query results and CharacterData objects</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class SqlQueryExecutorService
    {
        private static SqlQueryExecutorService _instance;
        public static SqlQueryExecutorService Instance => _instance ??= new SqlQueryExecutorService();
        
        private SqlQueryExecutorService() { }
        
        public void ExecuteQuery(string query, Action<List<CharacterData>> onResultsProcessed)
        {
            if (string.IsNullOrEmpty(query))
            {
                onResultsProcessed?.Invoke(new List<CharacterData>());
                return;
            }
            
            ServiceManager.Instance.Sql.ExecuteQuery(query, reader => {
                List<CharacterData> queryResults = ProcessQueryResults(reader);
                onResultsProcessed?.Invoke(queryResults);
            });
        }
        
        private List<CharacterData> ProcessQueryResults(SqliteDataReader reader)
        {
            List<CharacterData> queryResults = new List<CharacterData>();

            try
            {
                while (reader.Read())
                {
                    CharacterData data = new CharacterData();
                    
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i).ToLower();
                        
                        if (columnName == "id" || columnName == "army_id")
                            data.Id = reader.GetInt32(i);
                        else if (columnName == "name")
                            data.Name = reader.GetString(i);
                        else if (columnName == "faction")
                            data.Faction = reader.GetString(i);
                        else if (columnName == "role")
                            data.Role = reader.GetString(i);
                    }
                    
                    if (!string.IsNullOrEmpty(data.Name))
                    {
                        queryResults.Add(data);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Erreur lors de la lecture des résultats: {e.Message}");
            }
            
            return queryResults;
        }
        
        public List<CharacterData> ExtractAllCharactersData()
        {
            List<CharacterData> characterDataList = new List<CharacterData>();
            
            string query = @"
                SELECT a.id, a.name, a.faction, a.role, s.health, s.attack, s.defense, s.mana, s.experience
                FROM army a
                JOIN stats s ON a.id = s.army_id
            ";
            
            ServiceManager.Instance.Sql.ExecuteQuery(query, reader => {
                while (reader.Read())
                {
                    CharacterData characterData = new CharacterData
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Faction = reader.GetString(2),
                        Role = reader.GetString(3),
                        Health = reader.GetInt32(4),
                        Attack = reader.GetInt32(5),
                        Defense = reader.GetInt32(6),
                        Mana = reader.GetInt32(7),
                        Experience = reader.GetInt32(8)
                    };
                    
                    characterDataList.Add(characterData);
                }
            });
            
            return characterDataList;
        }
    }
}