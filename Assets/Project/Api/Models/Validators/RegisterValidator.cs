using Project.Api.Constants;
using Project.Api.Models.Requests;

namespace Project.Api.Models.Validators
{
    public class RegisterValidator : IValidator<RegisterRequest>
    {
        public ValidationResult Validate(RegisterRequest model)
        {
            var result = new ValidationResult();

            ValidateUsername(model.username, result);
            ValidateEmail(model.email, result);
            ValidatePassword(model.password, result);
    
            return result;
        }

        private void ValidateUsername(string username, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                result.Errors.Add(ValidationMessages.RequiredUsername);
                return;
            }

            if (username.Length < 3)
            {
                result.Errors.Add(ValidationMessages.InvalidUsernameLength);
            }
        }

        private void ValidateEmail(string email, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                result.Errors.Add(ValidationMessages.RequiredEmail);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                result.Errors.Add(ValidationMessages.InvalidEmail);
            }
        }

        private void ValidatePassword(string password, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add(ValidationMessages.RequiredPassword);
                return;
            }

            if (password.Length < 6)
            {
                result.Errors.Add(ValidationMessages.InvalidPasswordLength);
            }
        }
    }
}