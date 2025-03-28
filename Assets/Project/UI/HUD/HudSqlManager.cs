using System.Collections.Generic;
using Project.Core.Events;
using Project.Core.Service;
using UnityEngine;

namespace Project.UI.HUD
{
    /// <summary>
    /// Manages SQL tasks and queries for the HUD interface
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Implements singleton pattern for global access</item>
    /// <item>Handles SQL task management and validation</item>
    /// <item>Interfaces with event system for scene transitions</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class HudSqlManager : MonoBehaviour
    {
        private static HudSqlManager _instance;
        public static HudSqlManager Instance => _instance ??= CreateInstance();
        
        private SqlQueryData _currentTask;

        private static HudSqlManager CreateInstance()
        {
            var go = new GameObject("HudSqlManager");
            _instance = go.AddComponent<HudSqlManager>();
            DontDestroyOnLoad(go);
            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeHUDManager();
        }

        private void InitializeHUDManager()
        {
            EventManager.Instance.Subscribe(SqlEventType.LevelSelected, OnLevelSelected);
            EventManager.Instance.Subscribe(GameEventType.SceneLoaded, OnSceneLoaded);
        }

        private void OnDestroy()
        {
            EventManager.Instance.Unsubscribe(SqlEventType.LevelSelected, OnLevelSelected);
            EventManager.Instance.Unsubscribe(GameEventType.SceneLoaded, OnSceneLoaded);
        }
        
        private void OnSceneLoaded()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SqlLevel")
            {
                EventManager.Instance.TriggerEvent(SqlEventType.LevelSelected);
            }
        }

        private void OnLevelSelected()
        {
            LoadFirstTask();
        }

        private void LoadFirstTask()
        {
            _currentTask = null;
            
            var levelData = ServiceManager.Instance.Sql.GetCurrentLevelData();
            if (levelData != null && levelData.tasks != null && levelData.tasks.Count > 0)
            {
                var task = levelData.tasks[0]; // On prend uniquement la première tâche
                
                _currentTask = new SqlQueryData(
                    task.id,
                    task.query,
                    task.name,
                    task.description,
                    task.difficulty,
                    task.hint,
                    task.allowedCommands
                );
                
                EventManager.Instance.TriggerEvent(SqlEventType.QueryValidated);
            }
        }

        public SqlQueryData GetCurrentTask()
        {
            return _currentTask;
        }
        
        public void SubmitQuery(string query)
        {
            if (_currentTask == null) return;
            
            ServiceManager.Instance.Sql.ExecuteQuery(query, reader => {
                // Traiter le résultat ici
                // Pour l'instant, on déclenchera juste l'événement de validation
                EventManager.Instance.TriggerEvent(SqlEventType.QueryValidated);
            });
        }
    }
}