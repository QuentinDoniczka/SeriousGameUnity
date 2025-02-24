using UnityEngine;
using UnityEngine.UI;
using Project.Core.Events;
using Project.Api.Models.Validators;
using Project.Api.Models.Requests;
using TMPro;

namespace Project.UI.Auth 
{
    public class RegisterPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;
        private RegisterValidator _registerValidator;
        private EventManager _eventManager;

        private void Start()
        {
            if (!AreAllComponentsAssigned()) 
            {
                Debug.LogError("All components are not assigned");
                return;
            }
            _eventManager = EventManager.Instance;
            _registerValidator = new RegisterValidator();
            
            InitializeButtons();
            ClearError();
        }

        private void InitializeButtons()
        {
            registerButton.onClick.AddListener(ValidateAndRegister);
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }
        private void ClearError() => 
            errorText.gameObject.SetActive(false);

        private void ValidateAndRegister()
        {
            var registerRequest = CreateRegisterRequest();
            var validationResult = _registerValidator.Validate(registerRequest);

            if (validationResult.IsValid)
            {
                Debug.Log("succeded");
                ClearError();
            }
            else
            {
                foreach(var error in validationResult.Errors)
                {
                    Debug.LogError(error);
                }
                errorText.gameObject.SetActive(true);
                errorText.text = validationResult.Errors[0];
            }
        }

        private RegisterRequest CreateRegisterRequest()
        {
            return new RegisterRequest
            {
                Username = usernameInput.text.Trim(),
                Email = emailInput.text.Trim(),
                Password = passwordInput.text
            };
        }

        private void OnLoginButtonClicked()
        {
            _eventManager.TriggerEvent(NavigationEventType.ToLogin);
        }

        private bool AreAllComponentsAssigned() => 
            usernameInput && 
            emailInput && 
            passwordInput && 
            errorText && 
            registerButton && 
            loginButton;

        private void OnDestroy()
        {
            registerButton.onClick.RemoveAllListeners();
            loginButton.onClick.RemoveAllListeners();
        }
    }
}