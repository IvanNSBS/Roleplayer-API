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

        [Header("Steering")]
        [SerializeField] private float _maxSteering;
        [SerializeField][Range(0, 1)] private float _maxSteerForce;
        [SerializeField] private float _sightRadius;
        [SerializeField] private float _acceptDistance;
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

            var desired = _followBehaviour.CurrentMoveDirection.normalized * _maxSteering;

            var steer = (Vector2)desired - _rb.velocity;
            if(steer.sqrMagnitude > _maxSteerForce * _maxSteering)
                steer = steer.normalized * _maxSteerForce * _maxSteering;

            _rb.AddForce(steer);

            if(_rb.velocity.sqrMagnitude > _maxSteering*Time.fixedDeltaTime)
                _rb.velocity = _rb.velocity.normalized*(_maxSteering*Time.fixedDeltaTime);
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
                    var dir = _followBehaviour.CurrentMoveDirection.normalized * _maxSteering * Time.fixedDeltaTime;
                    Gizmos.DrawLine(transform.position, transform.position + dir);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.position, transform.position + (Vector3)_rb.velocity);
                    
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