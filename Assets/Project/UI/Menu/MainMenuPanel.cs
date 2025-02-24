using UnityEngine;
using UnityEngine.UI;
using Project.Core.Events;

namespace Project.UI.Menu
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private Button disconnectButton;
        [SerializeField] private Button sqlButton;
        private EventManager _eventManager;

        private void Awake() 
        {
            _eventManager = EventManager.Instance;
            disconnectButton.onClick.AddListener(HandleDisconnect);
            sqlButton.onClick.AddListener(HandleSql);
        }

        private void HandleDisconnect()
        {
            _eventManager.TriggerEvent(NavigationEventType.ToLogin);
        }
        
        private void HandleSql()
        {
            _eventManager.TriggerEvent(NavigationEventType.ToSqlMenu);
        }

        private void OnDestroy()
        {
            if (disconnectButton) disconnectButton.onClick.RemoveAllListeners();
            if (sqlButton) sqlButton.onClick.RemoveAllListeners();
        }
        
    }
}