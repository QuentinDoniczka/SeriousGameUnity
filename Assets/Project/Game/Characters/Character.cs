using UnityEngine;

namespace Project.Game.Characters
{
    public class Character
    {
        public GameObject Instance { get; private set; }
        public string PrefabPath { get; private set; }
        private CharacterController _controller;

        public Vector2 Position 
        { 
            get => Instance?.transform.position ?? Vector2.zero;
            set 
            { 
                if (Instance != null)
                {
                    // Utiliser le contrôleur si disponible
                    if (_controller != null)
                    {
                        _controller.MoveTo(value);
                    }
                    else
                    {
                        Instance.transform.position = value;
                    }
                }
            }
        }

        public Character(GameObject instance, string prefabPath)
        {
            Instance = instance;
            PrefabPath = prefabPath;
            
            // Si le contrôleur n'existe pas déjà, l'ajouter
            _controller = Instance.GetComponent<CharacterController>();
            if (_controller == null)
            {
                _controller = Instance.AddComponent<CharacterController>();
            }
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