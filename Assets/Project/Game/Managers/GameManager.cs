// _Project/Game/Managers/GameManager.cs
using UnityEngine;
using Project.Core.Service;
using Project.Core.Events;

namespace Project.Game.Managers
{
    /// <summary>
    /// Main game manager responsible for level management and initialization of the game environment
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Handles singleton pattern implementation for global access</item>
    /// <item>Manages level loading from JSON resources</item>
    /// <item>Initializes database through SQL service</item>
    /// <item>Manages navigation events between game levels</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private string initialLevelName = "level_test";
        [SerializeField] private string levelQueriesPath = "LevelData/Sql";

        private static GameManager _instance;
        public static GameManager Instance => _instance ??= CreateInstance();

        private static GameManager CreateInstance()
        {
            var go = new GameObject("GameManager");
            _instance = go.AddComponent<GameManager>();
            DontDestroyOnLoad(go);
            return _instance;
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                RegisterEventListeners();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void RegisterEventListeners()
        {
            EventManager.Instance.Subscribe(NavigationEventType.ToSqlLevel, OnNavigateToSqlLevel);
        }
        
        private void OnDestroy()
        {
            EventManager.Instance.Unsubscribe(NavigationEventType.ToSqlLevel, OnNavigateToSqlLevel);
        }
        
        private void OnNavigateToSqlLevel()
        {
            LoadLevel(NavigationParameters.SelectedLevelName ?? initialLevelName);
        }

        public void LoadLevel(string levelName)
        {
            string resourcePath = $"{levelQueriesPath}/{levelName}";
            TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

            if (jsonFile == null)
            {
                Debug.LogError($"Failed to load level file: {levelName}");
                return;
            }

            try
            {
                ServiceManager.Instance.Sql.InitializeDatabaseFromJson(jsonFile.text);
                DebugCurrentLevelData();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize level data: {e.Message}");
            }
        }

        public void DebugCurrentLevelData()
        {
            try
            {
                //ServiceManager.Instance.Sql.DebugAllTables();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to debug level data: {e.Message}");
            }
        }
    }
}