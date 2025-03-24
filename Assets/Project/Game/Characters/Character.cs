using UnityEngine;

namespace Project.Game.Characters
{
    public class Character
    {
        public GameObject Instance { get; private set; }
        public string PrefabPath { get; private set; }
        public Vector2 Position 
        { 
            get => Instance?.transform.position ?? Vector2.zero;
            set 
            { 
                if (Instance != null)
                    Instance.transform.position = value;
            }
        }

        public Character(GameObject instance, string prefabPath)
        {
            Instance = instance;
            PrefabPath = prefabPath;
        }

        public void SetParent(Transform parent)
        {
            if (Instance != null && parent != null)
            {
                Instance.transform.SetParent(parent);
            }
        }

        public void Destroy()
        {
            if (Instance != null)
            {
                Object.Destroy(Instance);
                Instance = null;
            }
        }
    }
}