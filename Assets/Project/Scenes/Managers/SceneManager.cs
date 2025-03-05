using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Core.Events;
using Project.Core.Service;

namespace Project.Scenes.Managers 
{
    public class SceneManager : MonoBehaviour 
    {
        private static SceneManager _instance;
        public static SceneManager Instance => _instance ??= CreateInstance();

        private static SceneManager CreateInstance()
        {
            var go = new GameObject("SceneManager");
            _instance = go.AddComponent<SceneManager>();
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
            InitializeNavigationEvents();
        }

        private void InitializeNavigationEvents() 
        {
            var eventManager = EventManager.Instance;
            eventManager.Subscribe(NavigationEventType.ToMainMenu, () => LoadScene("MainMenu"));
            eventManager.Subscribe(NavigationEventType.ToGame, () => LoadScene("Game"));
            eventManager.Subscribe(NavigationEventType.ToLogin, () => LoadScene("Login"));
            eventManager.Subscribe(NavigationEventType.ToRegister, () => LoadScene("Register"));
            eventManager.Subscribe(NavigationEventType.ToSqlMenu, () => LoadScene("SqlMenu"));
            eventManager.Subscribe(NavigationEventType.ToSqlLevel, () => LoadScene("SqlLevel"));
            eventManager.Subscribe(NavigationEventType.ToSqlLevel, OnNavigateToSqlLevel);
        }
        private void OnNavigateToSqlLevel()
        {
            // Charger la scène SQL
            LoadScene("SqlLevel");
            
            // Enregistrer un callback pour quand la scène est chargée
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSqlSceneLoaded;
        }
        
        private void OnSqlSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SqlLevel")
            {
                string levelName = NavigationParameters.SelectedLevelName;
                
                // Charger le niveau dans SqlManager via ServiceManager
                ServiceManager.Instance.Sql.LoadLevelData(levelName);
                
                // Déclencher l'événement pour le HUD_SQL_Manager
                EventManager.Instance.TriggerEvent(SqlEventType.LevelSelected);
                
                // Se désabonner de l'événement sceneLoaded
                UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSqlSceneLoaded;
            }
        }

        private void LoadScene(string sceneName) 
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            EventManager.Instance.TriggerEvent(GameEventType.SceneLoaded);
        }

        public string GetCurrentScene() => 
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
}