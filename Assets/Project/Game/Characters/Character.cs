using UnityEngine;

namespace Project.Game.Characters
{
    public class Character
    {
        public GameObject Instance { get; private set; }
        public string PrefabPath { get; private set; }
        private CharacterMove _move;
        private Vector3 _lastTargetPosition;

        public Vector2 Position 
        { 
            get => Instance?.transform.position ?? Vector2.zero;
            set 
            { 
                if (Instance != null)
                {
                    _lastTargetPosition = value;
                    
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

        public bool IsMoving => _move != null && _move.IsMoving();

        public Character(GameObject instance, string prefabPath)
        {
            Instance = instance;
            PrefabPath = prefabPath;
            _lastTargetPosition = instance.transform.position;
            
            _move = Instance.GetComponent<CharacterMove>();
            if (_move == null)
            {
                _move = Instance.AddComponent<CharacterMove>();
            }
            
            DisablePotentialConflictingScripts();
        }

        private void DisablePotentialConflictingScripts()
        {
            string[] potentialConflictScripts = {
                "NavMeshAgent", "Rigidbody", "Rigidbody2D", "CharacterController"
            };
            
            foreach (var scriptName in potentialConflictScripts)
            {
                if (scriptName == "CharacterController") continue;
                
                Component component = Instance.GetComponent(scriptName);
                if (component != null && component != _move)
                {
                    MonoBehaviour behaviour = component as MonoBehaviour;
                    if (behaviour != null)
                    {
                        behaviour.enabled = false;
                    }
                }
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

        public void RandomLocation(float speed = 2.0f, float minX = 0f, float maxX = 1f, float minY = 0f, float maxY = 1f)
        {
            Vector2 randomDestination = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );
            
            MoveTo(randomDestination, speed);
        }
        
        public void MoveTo(Vector2 position, float speed = 2.0f)
        {
            if (Instance != null)
            {
                _lastTargetPosition = position;
                
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
        
        public void ResetToLastTargetPosition()
        {
            if (Instance != null && _move != null)
            {
                _move.MoveTo(_lastTargetPosition, 0);
            }
        }
    }
}