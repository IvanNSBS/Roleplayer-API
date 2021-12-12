using UnityEngine;
using INUlib.RPG.AI.Movement;
using INUlib.RPG.AI.Movement.Behaviour;

namespace INUlib.RPG.AI.Movement.Components
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
        private SteeringBehaviour _followBehaviour;
        private float _colliderSize = 1;
        private Vector3 _colPos => GetComponent<Collider2D>().bounds.center;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _colliderSize = GetComponent<Collider2D>().bounds.extents.x;
            _followBehaviour = new SteeringBehaviour(_acceptDistance, _desiredSpeed, _maxSteerForce);
            _followBehaviour.SetTarget(_target);
            SetMovementType(_type);
        }

        private void Update()
        {
            _followBehaviour.CalculateDesiredSpeed(_colPos);
        }


        Vector3 _desired;
        private void FixedUpdate()
        {
            _desired = _followBehaviour.DesiredSpeed;
            _desired = SteerDirection.Avoid(_colPos, _desired, _colliderSize*2f, _rays, 0, false);

            var steer = (Vector2)_desired - _rb.velocity;
            if(steer.magnitude > _maxSteerForce)
                steer = steer.normalized * _maxSteerForce;

            _rb.AddForce(steer);

            float finalDesired = _followBehaviour.DesiredSpeed.magnitude*Time.fixedDeltaTime;
            if(_rb.velocity.magnitude > finalDesired)
                _rb.velocity = _rb.velocity.normalized*finalDesired;
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
            _followBehaviour.SetMovementType(type);
        }
        #endregion
    }
}