using Project.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Menu
{
    /// <summary>
    /// Handles the SQL menu panel user interface and navigation
    /// </summary>
    /// <remarks>
    /// This class manages the SQL menu panel, providing functionality to:
    /// <list type="bullet">
    /// <item>Navigate back to the login screen</item>
    /// <item>Launch the SQL test level</item>
    /// <item>Properly clean up event listeners when destroyed</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
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
            NavigationParameters.SelectedLevelName = "level_test";
            
            _eventManager.TriggerEvent(NavigationEventType.ToSqlLevel);
        }
        private void OnDestroy()
        {
            if (returnButton) returnButton.onClick.RemoveAllListeners();
            if (lvlTestButton) lvlTestButton.onClick.RemoveAllListeners();
        }
    }
}