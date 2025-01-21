using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core.Events;

namespace Project.UI.Auth
{
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button guestButton;

        private void Start()
        {
            loginButton.onClick.AddListener(HandleLogin);
            registerButton.onClick.AddListener(HandleRegister);
            guestButton.onClick.AddListener(HandleGuest);
        }

        private void HandleLogin()
        {
            var username = usernameInput.text;
            var password = passwordInput.text;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return;

            EventManager.Instance.TriggerEvent(NavigationEventType.ToGame);
        }

        private void HandleRegister()
        {
            EventManager.Instance.TriggerEvent(NavigationEventType.ToRegister);
        }

        private void HandleGuest()
        {
            EventManager.Instance.TriggerEvent(NavigationEventType.ToGame);
        }
    }
}