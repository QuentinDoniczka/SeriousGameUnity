using UnityEngine;
using Project.Core.Service;
using Project.Core.Events;
using Project.Scenes.Managers;
using Project.Game.Managers;

namespace Project.Core
{
    public class CoreManager : MonoBehaviour
    {
        public static CoreManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCore();
        }

        private void InitializeCore()
        {
            _ = ServiceManager.Instance;
            _ = EventManager.Instance;
            _ = SceneManager.Instance;
            _ = GameManager.Instance;
            
            EventManager.Instance.TriggerEvent(NavigationEventType.ToLogin);
        }
    }
}