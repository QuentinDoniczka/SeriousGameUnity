// UI/Auth/LoginPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core.Events;
using Project.Api.User;
using Project.Api.Models.Requests;

namespace Project.UI.Auth
{
    /// <summary>
    /// Handles the login panel UI and authentication process for users
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Manages login form UI elements and input validation</item>
    /// <item>Processes login credentials through the UserApiService</item>
    /// <item>Handles error states and provides user feedback</item>
    /// <item>Manages navigation to registration or main menu screens</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;

        private EventManager _eventManager;
        private IUserApiService _userApiService;

        private void Awake() => _eventManager = EventManager.Instance;

        private void Start()
        {
            if (!ValidateComponents()) return;
            
            _userApiService = new UserApiService();
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
            ClearError();
            if (!ValidateInputs())
            {
                ShowError("Email and password are required");
                return;
            }

            try
            {
                loginButton.interactable = false;
        
                var request = new LoginRequest
                {
                    email = emailInput.text.Trim(),
                    password = passwordInput.text
                };
        
                var response = await _userApiService.Login(request);
        
                // Vérifier si la réponse de l'API est valide
                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Log the successful response
                    Debug.Log($"Login successful for: {response.Email}");
                    Debug.Log($"Username: {response.Username}");
                    Debug.Log($"Token: {response.Token}");
                    Debug.Log($"Expiration: {response.Expiration}");
            
                    // You might want to store the token for future API calls
                    // PlayerPrefs.SetString("AuthToken", response.Token);
            
                    // Navigate to the main menu ONLY after successful login
                    _eventManager.TriggerEvent(NavigationEventType.ToMainMenu);
                }
                else
                {
                    // Si la réponse est nulle ou le token est vide, c'est une erreur
                    Debug.LogError("Login failed: Invalid response from server");
                    ShowError("Login failed. Please check your credentials and try again.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Login error: {e.Message}");
                ShowError("Login failed. Please check your credentials and try again.");
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