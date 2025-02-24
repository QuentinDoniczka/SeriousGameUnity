using Project.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Menu
{
    public class SqlMenuPanel : MonoBehaviour
    {
        [SerializeField] private Button returnButton;
        [SerializeField] private Button lvlTestButton;
        private EventManager _eventManager;
        
        private void Awake() 
        {
            _eventManager = EventManager.Instance;
            returnButton.onClick.AddListener(HandleReturn);
            lvlTestButton.onClick.AddListener(HandleLvlTest);
        }

        private void HandleReturn()
        {
            _eventManager.TriggerEvent(NavigationEventType.ToLogin);
        }
        
        private void HandleLvlTest()
        {
            _eventManager.TriggerEvent(NavigationEventType.ToSqlTest);
        }
        
        private void OnDestroy()
        {
            if (returnButton) returnButton.onClick.RemoveAllListeners();
        }
    }
}