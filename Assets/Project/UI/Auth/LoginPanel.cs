using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core.Events;
using System.Text;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Project.Models;

namespace Project.UI.Auth
{
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;

        private const string API_URL = "http://localhost:5000/api/user/login";
        private EventManager _eventManager;

        [System.Serializable]
        private class LoginRequestData
        {
            public string email;
            public string password;
        }

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
                var response = await SendLoginRequest();
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

        private async Task<LoginResponse> SendLoginRequest()
        {
            var loginData = new LoginRequestData 
            { 
                email = emailInput.text.Trim(),
                password = passwordInput.text
            };

            using var request = new UnityWebRequest(API_URL, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginData));
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.responseCode == 400)
            {
                throw new System.Exception("Invalid credentials");
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new System.Exception("Network error, please try again");
            }

            return JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
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