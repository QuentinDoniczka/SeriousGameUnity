using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core.Events;
using System.Text;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Project.UI.Auth 
{
    public class RegisterPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;

        private const string API_URL = "http://localhost:5000/api/user/register";
        private EventManager _eventManager;

        [System.Serializable]
        private class RegisterRequest
        {
            public string username;
            public string email;
            public string password;
        }

        [System.Serializable]
        private class RegisterResponse
        {
            public bool success;
            public string[] errors;
        }

        [System.Serializable]
        private class ValidationErrorResponse
        {
            public string title;
            public int status;
            public ValidationErrors errors;
        }

        [System.Serializable]
        private class ValidationErrors
        {
            public string[] Email;
            public string[] Username;
            public string[] Password;
        }

        private void Awake() => _eventManager = EventManager.Instance;

        private void Start()
        {
            if (!ValidateComponents()) return;
            
            registerButton.onClick.AddListener(HandleRegister);
            loginButton.onClick.AddListener(HandleBack);
            HideMessage();
        }

        private void HideMessage() => 
            messageText.gameObject.SetActive(false);

        private void ShowMessage(string message, bool isError = true)
        {
            messageText.text = message;
            messageText.color = isError ? Color.red : Color.green;
            messageText.gameObject.SetActive(true);
        }

        private bool ValidateInputsNotEmpty()
        {
            var emptyFields = new List<string>();
            
            if (string.IsNullOrEmpty(usernameInput?.text?.Trim())) emptyFields.Add("Username");
            if (string.IsNullOrEmpty(emailInput?.text?.Trim())) emptyFields.Add("Email");
            if (string.IsNullOrEmpty(passwordInput?.text)) emptyFields.Add("Password");
            
            if (emptyFields.Count > 0)
            {
                ShowMessage($"Please fill in the following fields: {string.Join(", ", emptyFields)}");
                return false;
            }
            
            return true;
        }

        private async void HandleRegister()
        {
            if (!ValidateInputsNotEmpty())
            {
                return;
            }

            registerButton.interactable = false;
            
            try 
            {
                var response = await SendRegisterRequest();
                
                if (response.success)
                {
                    ShowMessage("Registration successful! Redirecting to login...", false);
                    StartCoroutine(RedirectToLogin());
                }
            }
            catch (System.Exception e)
            {
                ShowMessage(e.Message);
            }
            finally
            {
                registerButton.interactable = true;
            }
        } 
        private async Task<RegisterResponse> SendRegisterRequest()
        {
            var registerData = new RegisterRequest 
            { 
                username = usernameInput.text.Trim(),
                email = emailInput.text.Trim(),
                password = passwordInput.text
            };

            using var request = new UnityWebRequest(API_URL, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(registerData));

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            try 
            {
                await request.SendWebRequest();
            }
            catch (System.Exception)
            {
                throw new System.Exception("Network error. Please check your connection and try again.");
            }

            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new System.Exception("Network error. Please check your connection and try again.");
            }

            if (string.IsNullOrEmpty(request.downloadHandler.text))
            {
                throw new System.Exception("Server error. Please try again later.");
            }

            // Handle validation errors (400 status code)
            if (request.responseCode == 400)
            {
                try
                {
                    var validationError = JsonUtility.FromJson<ValidationErrorResponse>(request.downloadHandler.text);
                    
                    if (validationError?.errors != null)
                    {
                        // Check validation errors in priority order
                        if (validationError.errors.Password?.Length > 0)
                            throw new System.Exception(validationError.errors.Password[0]);
                        
                        if (validationError.errors.Email?.Length > 0)
                            throw new System.Exception("Invalid email format");
                        
                        if (validationError.errors.Username?.Length > 0)
                            throw new System.Exception(validationError.errors.Username[0]);
                    }

                    // If no validation errors found, try parsing as RegisterResponse
                    var response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
                    if (!response.success && response.errors?.Length > 0)
                        throw new System.Exception(response.errors[0]);
                }
                catch (System.Exception e)
                {
                    // If the error is already formatted (from our throws above), rethrow it
                    if (e.Message != "JsonUtility.FromJson failed")
                        throw;
                    
                    throw new System.Exception("Invalid server response. Please try again later.");
                }
            }

            // Handle successful response
            var successResponse = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
            if (successResponse == null || !successResponse.success)
            {
                throw new System.Exception("Server error. Please try again later.");
            }

            return successResponse;
        }

        private RegisterResponse HandleError(string message)
        {
            throw new System.Exception(message);
        }

        private IEnumerator RedirectToLogin()
        {
            yield return new WaitForSeconds(1);
            _eventManager?.TriggerEvent(NavigationEventType.ToLogin);
        }

        private void HandleBack()
        {
            HideMessage();
            _eventManager?.TriggerEvent(NavigationEventType.ToLogin);
        }

        private bool ValidateComponents() =>
            usernameInput && emailInput && passwordInput && messageText && 
            registerButton && loginButton;

        private void OnDestroy()
        {
            if (registerButton) registerButton.onClick.RemoveAllListeners();
            if (loginButton) loginButton.onClick.RemoveAllListeners();
        }
    }
}