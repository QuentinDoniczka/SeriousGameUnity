// _Project/Game/Managers/GameManager.cs
using UnityEngine;
using Project.Core.Service;

namespace Project.Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private string initialLevelName = "level_test";
        [SerializeField] private string levelQueriesPath = "LevelData";

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeGame()
        {
            LoadLevel(initialLevelName);
            DebugCurrentLevelData();
        }

        private void LoadLevel(string levelName)
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
                ServiceManager.Instance.Sql.DebugAllTables();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to debug level data: {e.Message}");
            }
        }
    }
}