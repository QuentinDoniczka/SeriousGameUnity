using UnityEngine;

namespace Project.Game.Characters
{
    /// <summary>
    /// Controls character movement with smooth transitions and animation synchronization.
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Handles character movement using Vector3 position targeting</item>
    /// <item>Manages animation states during movement (Run/Idle)</item>
    /// <item>Prevents position overrides through forcePosition parameter</item>
    /// <item>Provides public interface for movement control and state checking</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class CharacterMove : MonoBehaviour
    {
        private Animator _animator;
        private bool _overridePosition = false;
        private Vector3 _targetPosition;
        private float _moveSpeed = 2.0f;
        private bool _isMoving = false;
        private Vector3 _lastPosition;
        
        [SerializeField] private bool forcePosition = true;

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
                    
                    if (_animator != null)
                    {
                        _animator.Play("Idle");
                    }
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
                    if (forcePosition && transform.position != _lastPosition)
                    {
                        transform.position = _lastPosition;
                    }
                }
                else if (forcePosition)
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
                
                if (_animator != null)
                {
                    _animator.Play("Run");
                }
            }
            else
            {
                transform.position = _targetPosition;
                _lastPosition = _targetPosition;
                _isMoving = false;
                
                if (_animator != null)
                {
                    _animator.Play("Idle");
                }
            }
        }
        public bool IsMoving()
        {
            return _isMoving && _overridePosition;
        }
    }
}