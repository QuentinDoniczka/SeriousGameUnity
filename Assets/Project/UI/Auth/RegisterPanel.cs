using UnityEngine;
using UnityEngine.UI;
using Project.Core.Events;
using Project.Api.Models.Validators;
using Project.Api.Models.Requests;

namespace Project.UI.Auth 
{
    public class RegisterPanel : MonoBehaviour
    {
        [SerializeField] private InputField usernameInput;
        [SerializeField] private InputField emailInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Text messageText;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;

        private RegisterValidator _registerValidator;
        private EventManager _eventManager;

        private void Start()
        {
            if (!AreAllComponentsAssigned()) return;
            
            _eventManager = EventManager.Instance;
            _registerValidator = new RegisterValidator();
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            registerButton.onClick.AddListener(ValidateAndRegister);
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        private void ValidateAndRegister()
        {
            var registerRequest = CreateRegisterRequest();
            var validationResult = _registerValidator.Validate(registerRequest);

            if (validationResult.IsValid)
            {
                Debug.Log("Validation réussie");
            }
            else
            {
                foreach(var error in validationResult.Errors)
                {
                    Debug.LogError(error);
                }
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
            messageText && 
            registerButton && 
            loginButton;

        private void OnDestroy()
        {
            registerButton.onClick.RemoveAllListeners();
            loginButton.onClick.RemoveAllListeners();
        }
    }
}