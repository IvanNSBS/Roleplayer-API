using UnityEngine;
using INUlib.RPG.AI.Movement.Steering;
using INUlib.RPG.AI.Movement.Steering.Behaviour;

namespace INUlib.RPG.AI.Movement.Steering.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AISteeringMover : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private MovementType _type;
        [SerializeField] private Transform _target;
        [SerializeField] private float _sightRadius;
        [SerializeField] private float _acceptDistance;
        [SerializeField] private float _speed;
        #endregion


        #region Fields
        private Rigidbody2D _rb;
        private SteeringPolicy _followBehaviour;
        #endregion Fields
        
        
        #region MonoBehaviour Methods
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            SetMovementType(_type);
        }

        private void Update()
        {
            _followBehaviour.OnUpdate(transform.position);
        }

        private void FixedUpdate()
        {
            if(_followBehaviour.HasReachedTarget)
            {
                _rb.velocity = Vector3.zero;
                return;
            }

            var dir = _followBehaviour.CurrentMoveDirection;
            _rb.velocity = dir*_speed*Time.fixedDeltaTime;
        }

            #if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, _sightRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, _acceptDistance);
                Gizmos.color = Color.magenta;

                if(_followBehaviour != null)
                {
                    var dir = _followBehaviour.CurrentMoveDirection;
                    Gizmos.DrawLine(transform.position, transform.position + dir*_speed*Time.fixedDeltaTime);
                    var vec = SteerDirection.AverageVector(transform.position, _followBehaviour.CurrentMoveDirection, _acceptDistance, 24, 0, true);
                }
                
            }
            #endif
        #endregion MonoBehaviour Methods


        #region Methods
        public void SetMovementType(MovementType type)
        {
            switch(type)
            {
                case MovementType.Wander:
                    break;
                case MovementType.Follow:
                    _followBehaviour = new FollowPolicy(_acceptDistance, _target);
                    break;
                case MovementType.Flee:
                    _followBehaviour = new FleePolicy(_acceptDistance, _target);
                    break;
                
                default:
                    break;
            }
        }
        #endregion
    }
}