using UnityEngine;
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
        [SerializeField] private SteeringData _steeringData;
        [SerializeField] private float _sightRadius;
        [SerializeField] private bool _debugAvoid;
        #endregion


        #region Fields
        private Rigidbody2D _rb;
        private SteeringBehaviour _followBehaviour;
        private Vector3 _colPos => GetComponent<Collider2D>().bounds.center;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _followBehaviour = new SteeringBehaviour(_rb, _steeringData);
            SetMovementType(_type);
        }

        private void Update()
        {
            _followBehaviour.OnUpdate(_colPos, _target ? _target.position : null);
            _followBehaviour.DebugAvoid = _debugAvoid;
        }


        private void FixedUpdate()
        {
            _followBehaviour.OnFixedUpdate(_colPos, _target ? _target.position : null);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_colPos, _sightRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_colPos, _steeringData.AcceptDistance);
        }
        #endif

        #endregion MonoBehaviour Methods


        #region Methods
        public void SetSteeringData(SteeringData data)
        {
            _steeringData = data;
            _followBehaviour.SetSteeringData(_steeringData);
        }

        public void SetMovementType(MovementType type)
        {
            _followBehaviour.SetMovementType(type);
        }
        #endregion
    }
}