// Project/Api/Constants/UserApiConstants.cs
namespace Project.Api.Constants
{
    public static class UserApiEndpoint
    {
        private const string BaseEndpoint = BaseApiConstants.BaseUrl + "/user";
        
        public const string Register = BaseEndpoint + "/register";
        public const string Login = BaseEndpoint + "/login";
        public const string Profile = BaseEndpoint + "/users";
        public const string Description = "/description";
    }
}