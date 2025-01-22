// LoginResponse.cs
using System;

namespace Project.Models
{
    [Serializable]
    public class LoginResponse
    {
        public string email;
        public string username;
        public string token;
        public string expiration;
    }
}