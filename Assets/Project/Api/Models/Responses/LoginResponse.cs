// Models/Responses/LoginResponse.cs
namespace Project.Api.Models.Responses
{
    public class LoginResponse
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Expiration { get; set; }
    }
}