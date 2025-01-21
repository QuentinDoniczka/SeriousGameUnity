using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Core.Events;

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
            if (_instance != null) 
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SubscribeToEvents();
        }

        private void SubscribeToEvents() 
        {
            EventManager.Instance.Subscribe<NavigationEventType>(
                NavigationEventType.ToGame,
                HandleNavigation
            );
        }

        private void HandleNavigation(NavigationEventType navEvent) 
        {
            var sceneName = navEvent.ToString()[2..]; // Using range operator instead of Substring
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}