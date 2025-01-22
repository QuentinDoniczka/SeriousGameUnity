using Project.Core.Events;
using Project.Core.Service;
using Project.Game.Managers;
using Project.Scenes.Managers;
using UnityEngine;

namespace Project.Core
{
    public class CoreManager : MonoBehaviour
    {
        private static CoreManager _instance;
        public static CoreManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystems();
        }

        private void InitializeSystems()
        {
            _ = ServiceManager.Instance;
            _ = EventManager.Instance;
            _ = SceneManager.Instance;
            _ = GameManager.Instance;
            
            EventManager.Instance.TriggerEvent(NavigationEventType.ToLogin);
        }
    }
}