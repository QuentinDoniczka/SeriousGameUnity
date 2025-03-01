// Project/UI/HUD/HUD_SQL_Manager.cs
using System.Collections.Generic;
using Project.Core.Events;
using Project.Core.Service;
using Project.Database;
using UnityEngine;

namespace Project.UI.HUD
{
    public class HudSqlManager : MonoBehaviour
    {
        private static HudSqlManager _instance;
        public static HudSqlManager Instance => _instance ??= CreateInstance();
        
        private SqlQueryData _currentTask;

        private static HudSqlManager CreateInstance()
        {
            var go = new GameObject("HUD_SQL_Manager");
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
        }

        private void OnDestroy()
        {
            EventManager.Instance.Unsubscribe(SqlEventType.LevelSelected, OnLevelSelected);
        }

        private void OnLevelSelected()
        {
            LoadFirstTask();
        }

        private void LoadFirstTask()
        {
            _currentTask = null;
            
            var sqlManager = ServiceManager.Instance.Sql;
            var levelData = sqlManager.GetCurrentLevelData();
            
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
                    task.allowed_commands
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