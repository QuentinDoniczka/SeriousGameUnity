// ApiServiceManager.cs
using Project.Api.User;
using UnityEngine;

namespace Project.Api
{
    public class ApiServiceManager : MonoBehaviour
    {
        private static ApiServiceManager _instance;
        public static ApiServiceManager Instance => _instance;

        public IUserApiService UserService { get; private set; }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }

        private void InitializeServices()
        {
            UserService = new UserApiService();
        }
    }
}