using UnityEngine;

namespace Project.Game.Characters
{
    public class Character
    {
        public GameObject Instance { get; private set; }
        public string PrefabPath { get; private set; }
        private CharacterMove _move;
        public Vector2 StartPosition { get; private set; }

        public Vector2 Position 
        { 
            get => Instance?.transform.position ?? Vector2.zero;
            set 
            { 
                if (Instance != null)
                {
                    if (_move != null)
                    {
                        _move.MoveTo(value, 0);
                    }
                    else
                    {
                        Instance.transform.position = value;
                    }
                }
            }
        }
        
        public void MoveToRandomPositionInZone(GameObject zone, float speed = 2.0f)
        {
            if (zone == null || Instance == null) return;
    
            Vector2 randomPosition = Utilities.ZoneUtility.GetRandomPositionInZone(zone);
            MoveTo(randomPosition, speed);
        }

        public void MoveToPositionInZone(GameObject zone, Vector2 normalizedPosition, float speed = 1.5f)
        {
            if (zone == null || Instance == null) return;
    
            Vector2 destination = Utilities.ZoneUtility.GetPositionInZone(zone, normalizedPosition);
            MoveTo(destination, speed);
        }

        public bool IsMoving => _move != null && _move.IsMoving();

        public Character(GameObject instance, string prefabPath)
        {
            Instance = instance;
            PrefabPath = prefabPath;
            StartPosition = instance.transform.position;
            
            _move = Instance.GetComponent<CharacterMove>();
            if (_move == null)
            {
                _move = Instance.AddComponent<CharacterMove>();
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
        
        public void MoveTo(Vector2 position, float speed = 1.5f)
        {
            if (Instance != null)
            {
                if (_move != null)
                {
                    _move.MoveTo(position, speed);
                }
                else
                {
                    Instance.transform.position = position;
                }
            }
        }
        
        public void MoveToStartPosition(float speed = 1.5f)
        {
            MoveTo(StartPosition, speed);
        }
    }
}