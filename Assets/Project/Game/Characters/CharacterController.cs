using UnityEngine;

namespace Project.Game.Characters
{
    public class CharacterController : MonoBehaviour
    {
        private Animator _animator;
        private bool _overridePosition = false;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            if (_overridePosition)
            {
                transform.position = _targetPosition;
            }
        }

        public void MoveTo(Vector3 position)
        {
            _targetPosition = position;
            _overridePosition = true;
        }

        public void StopOverriding()
        {
            _overridePosition = false;
        }
    }
}