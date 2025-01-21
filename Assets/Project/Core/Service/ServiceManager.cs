// _Project/Core/ServiceManager.cs

using Project.Database;
using UnityEngine;

namespace Project.Core.Service
{
    public class ServiceManager : MonoBehaviour
    {
        private static ServiceManager _instance;
        public static ServiceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ServiceManager");
                    _instance = go.AddComponent<ServiceManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public SqlManager Sql { get; private set; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializeServices();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeServices()
        {
            var sqlObj = new GameObject("SqlManager") 
            { 
                transform = { parent = transform }
            };
            Sql = sqlObj.AddComponent<SqlManager>();
        }
    }
}