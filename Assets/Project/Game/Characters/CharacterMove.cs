using UnityEngine;

namespace Project.Game.Characters
{
    public class CharacterMove : MonoBehaviour
    {
        private Animator _animator;
        private bool _overridePosition = false;
        private Vector3 _targetPosition;
        private float _moveSpeed = 2.0f;
        private bool _isMoving = false;
        private Vector3 _lastPosition;
        
        [SerializeField] private bool _forcePosition = true;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _lastPosition = transform.position;
        }
        
        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (_overridePosition && _isMoving)
            {
                float step = _moveSpeed * Time.fixedDeltaTime;
                Vector3 newPosition = Vector3.MoveTowards(transform.position, _targetPosition, step);
                transform.position = newPosition;
                
                if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
                {
                    transform.position = _targetPosition;
                    _isMoving = false;
                }
                
                _lastPosition = transform.position;
            }
        }
        
        private void LateUpdate()
        {
            if (_overridePosition)
            {
                if (_isMoving)
                {
                    if (_forcePosition && transform.position != _lastPosition)
                    {
                        transform.position = _lastPosition;
                    }
                }
                else if (_forcePosition)
                {
                    transform.position = _targetPosition;
                }
            }
        }

        public void MoveTo(Vector3 position, float speed = 0)
        {
            _targetPosition = position;
            _overridePosition = true;
            
            if (speed > 0)
            {
                _moveSpeed = speed;
                _isMoving = true;
                _lastPosition = transform.position;
            }
            else
            {
                transform.position = _targetPosition;
                _lastPosition = _targetPosition;
                _isMoving = false;
            }
        }

        public void StopOverriding()
        {
            _overridePosition = false;
            _isMoving = false;
        }
        
        public bool IsMoving()
        {
            return _isMoving && _overridePosition;
        }
    }
}