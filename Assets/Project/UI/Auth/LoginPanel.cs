// UI/Auth/LoginPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core.Events;
using Project.Api;
using Project.Api.Models.Requests;

namespace Project.UI.Auth
{
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;

        private EventManager _eventManager;

        private void Awake() => _eventManager = EventManager.Instance;

        private void Start()
        {
            if (!ValidateComponents()) return;
            
            loginButton.onClick.AddListener(HandleLogin);
            registerButton.onClick.AddListener(HandleRegister);
            ClearError();
        }

        private bool ValidateComponents() =>
            emailInput && passwordInput && errorText && loginButton && registerButton;

        private void ClearError() => 
            errorText.gameObject.SetActive(false);

        private void ShowError(string message)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }

        private bool ValidateInputs() => 
            !string.IsNullOrEmpty(emailInput?.text) && 
            !string.IsNullOrEmpty(passwordInput?.text);

        private async void HandleLogin()
        {
            if (!ValidateInputs())
            {
                ShowError("Email and password are required");
                return;
            }

            loginButton.interactable = false;
            
            try 
            {
                var request = new LoginRequest
                {
                    Email = emailInput.text.Trim(),
                    Password = passwordInput.text
                };

                var response = await ApiServiceManager.Instance.UserService.Login(request);
                if (response != null)
                {
                    ClearError();
                    _eventManager?.TriggerEvent(NavigationEventType.ToMainMenu);
                }
            }
            catch (System.Exception e)
            {
                ShowError(e.Message);
            }
            finally
            {
                loginButton.interactable = true;
            }
        }

        private void HandleRegister()
        {
            ClearError();
            _eventManager.TriggerEvent(NavigationEventType.ToRegister);
        }

        private void OnDestroy()
        {
            if (loginButton) loginButton.onClick.RemoveAllListeners();
            if (registerButton) registerButton.onClick.RemoveAllListeners();
        }
    }
}