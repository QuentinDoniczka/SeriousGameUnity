// ApiConfig.cs
namespace Project.Api
{
    public static class ApiConfig
    {
        private const string BaseUrl = "http://localhost:5000/api";
        
        public const string UserEndpoint = BaseUrl + "/user";
    }
}