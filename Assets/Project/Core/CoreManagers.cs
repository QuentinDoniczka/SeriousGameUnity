using Project.Core.Events;
using Project.Core.Service;
using Project.Game.Managers;
using Project.Scenes.Managers;
using UnityEngine;

namespace Project.Core
{
    /// <summary>
    /// Singleton manager responsible for initializing and maintaining core systems of the application.
    /// </summary>
    /// <remarks>
    /// This class handles:
    /// <list type="bullet">
    /// <item>Singleton pattern implementation with instance protection</item>
    /// <item>Core systems initialization in the proper sequence</item>
    /// <item>Persistence between scene transitions</item>
    /// <item>Initial navigation event triggering</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
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