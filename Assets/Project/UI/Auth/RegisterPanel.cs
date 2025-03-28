using UnityEngine;
using UnityEngine.UI;
using Project.Core.Events;
using Project.Api.Models.Validators;
using Project.Api.Models.Requests;
using Project.Api.User;
using TMPro;
using System.Collections;

namespace Project.UI.Auth 
{
    /// <summary>
    /// Manages the user registration panel, handling form validation and API communication
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Validates user input for registration form</item>
    /// <item>Communicates with user API service for registration</item>
    /// <item>Handles error display and success redirection</item>
    /// <item>Manages UI component interactions and event triggering</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
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
       private IUserApiService _userApiService;

       private void Start()
       {
           if (!AreAllComponentsAssigned()) 
           {
               Debug.LogError("All components are not assigned");
               return;
           }
           _eventManager = EventManager.Instance;
           _registerValidator = new RegisterValidator();
           _userApiService = new UserApiService();
           
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
               RegisterUser(registerRequest);
           }
           else
           {
               DisplayValidationErrors(validationResult.Errors);
           }
       }

       private async void RegisterUser(RegisterRequest registerRequest)
       {
           try 
           {
               registerButton.interactable = false;
               var response = await _userApiService.Register(registerRequest);
        
               if (response.Success)
               {
                   Debug.Log($"Register response: {response.Success}");
                   errorText.gameObject.SetActive(true);
                   errorText.color = Color.green;
                   errorText.text = "Registration successful! redirecting to login...";
                   
                   StartCoroutine(RedirectAfterDelay(1.5f));
               }
               else
               {
                   DisplayErrors(response.Errors);
               }
           }
           catch (System.Exception e)
           {
               Debug.LogError($"Register API error: {e.Message}");
               errorText.gameObject.SetActive(true);
               errorText.text = "Registration failed. Please try again.";
           }
           finally
           {
               registerButton.interactable = true;
           }
       }

       private void DisplayErrors(string[] errors)
       {
           errorText.gameObject.SetActive(true);
           if (errors.Length > 0)
           {
               errorText.text = errors[0];
               foreach (var error in errors)
               {
                   Debug.Log($"Register error: {error}");
               }
           }
       }

       private void DisplayValidationErrors(System.Collections.Generic.List<string> errors)
       {
           foreach(var error in errors)
           {
               Debug.LogError(error);
           }
           errorText.gameObject.SetActive(true);
           if (errors.Count > 0)
           {
               errorText.text = errors[0];
           }
       }

       // Coroutine pour attendre avant de rediriger
       private IEnumerator RedirectAfterDelay(float delay)
       {
           yield return new WaitForSeconds(delay);
           _eventManager.TriggerEvent(NavigationEventType.ToLogin);
       }

       private RegisterRequest CreateRegisterRequest()
       {
           return new RegisterRequest
           {
               username = usernameInput.text.Trim(),
               email = emailInput.text.Trim(),
               password = passwordInput.text
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