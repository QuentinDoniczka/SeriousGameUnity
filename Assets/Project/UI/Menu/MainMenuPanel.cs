using UnityEngine;
using UnityEngine.UI;
using Project.Core.Events;

namespace Project.UI.Menu
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private Button disconnectButton;
        private EventManager _eventManager;

        private void Awake() 
        {
            _eventManager = EventManager.Instance;
            disconnectButton.onClick.AddListener(HandleDisconnect);
        }

        private void HandleDisconnect()
        {
            _eventManager.TriggerEvent(NavigationEventType.ToLogin);
        }

        private void OnDestroy()
        {
            if (disconnectButton) disconnectButton.onClick.RemoveAllListeners();
        }
    }
}