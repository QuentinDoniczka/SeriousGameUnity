using System;

namespace Project.Api.Models.Requests
{
    [Serializable]
    public class LoginRequest
    {
        public string email;
        public string password;
    }
}