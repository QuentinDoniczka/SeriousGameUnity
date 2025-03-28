using UnityEngine;
using UnityEngine.UI;
using Project.Core.Events;

namespace Project.UI.Menu
{
    /// <summary>
    /// Manages the main menu panel UI elements and handles navigation events.
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Handles button click events for disconnection and SQL menu navigation</item>
    /// <item>Communicates with the EventManager system to trigger navigation events</item>
    /// <item>Properly cleans up event listeners when destroyed</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>

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