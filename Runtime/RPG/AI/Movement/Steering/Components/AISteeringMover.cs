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
        [SerializeField] private float _desiredSpeed;
        [SerializeField] private float _maxSteerForce;
        [SerializeField] private float _sightRadius;
        [SerializeField] private float _acceptDistance;
        [SerializeField] [Range(4, 24)] int _rays = 12;
        #endregion


        #region Fields
        private Rigidbody2D _rb;
        private SteeringPolicy _followBehaviour;
        private float _colliderSize = 1;
        private Vector3 _colPos => GetComponent<Collider2D>().bounds.center;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _colliderSize = GetComponent<Collider2D>().bounds.extents.x;
            SetMovementType(_type);
        }

        private void Update()
        {
            _followBehaviour.OnUpdate(transform.position);
        }


        Vector3 _desired;
        private void FixedUpdate()
        {
            if(_followBehaviour.HasReachedTarget)
            {
                _rb.velocity = Vector3.zero;
                return;
            }

            _desired = _followBehaviour.CurrentDesiredDirection.normalized * _desiredSpeed;
            _desired = SteerDirection.Avoid(_colPos, _desired, _colliderSize*2f, _rays, 0, false);

            var steer = (Vector2)_desired - _rb.velocity;
            if(steer.magnitude > _maxSteerForce)
                steer = steer.normalized * _maxSteerForce;

            _rb.AddForce(steer);

            if(_rb.velocity.magnitude > _desiredSpeed*Time.fixedDeltaTime)
                _rb.velocity = _rb.velocity.normalized*(_desiredSpeed*Time.fixedDeltaTime);
        }

            #if UNITY_EDITOR
            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_colPos, _sightRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_colPos, _acceptDistance);

                if(_followBehaviour != null)
                {
                    var dir = _desired * Time.fixedDeltaTime;
                    var vec = SteerDirection.Avoid(_colPos, _desired, _colliderSize*2f, _rays, 0, true);
                    
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(_colPos, _colPos + dir);
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