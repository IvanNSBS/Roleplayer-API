using UnityEngine;
using INUlib.RPG.AI.Movement.Behaviour;
using System;

namespace INUlib.RPG.AI.Movement.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AISteeringMover : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Config")]
        [SerializeField] private bool _autoInitialize;
        [SerializeField] private bool _debugAvoid;

        [Header("Steering")]
        [SerializeField] private Transform _target;
        [SerializeField] private MovementType _type;
        [SerializeField] private SteeringData _steeringData;
        [SerializeField] private float _sightRadius;
        #endregion


        #region Fields
        private bool _moveActive;
        private Rigidbody2D _rb;
        private SteeringBehaviour _followBehaviour;
        private Vector3 _colPos => GetComponent<Collider2D>().bounds.center;
        #endregion Fields


        #region Properties
        public Transform Target => _target;
        public bool MoveActive => _moveActive;
        #endregion


        #region MonoBehaviour Methods
        private void Awake()
        {
            if(_autoInitialize)
                Initialize();
        }

        private void Update()
        {
            if(_followBehaviour != null && _moveActive) {
                _followBehaviour.OnUpdate(_colPos, _target ? _target.position : null);
                _followBehaviour.DebugAvoid = _debugAvoid;
            }
        }


        private void FixedUpdate()
        {
            if(_moveActive)
                _followBehaviour?.OnFixedUpdate(_colPos, _target ? _target.position : null);
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
        public void Initialize()
        {
            _rb = GetComponent<Rigidbody2D>();
            _followBehaviour = new SteeringBehaviour(_rb, _steeringData);
            _moveActive = true;
            SetMovementType(_type);
        }

        public void SetSteeringData(SteeringData data)
        {
            _steeringData = data;
            _followBehaviour.SetSteeringData(_steeringData);
        }

        public void SetMovementType(MovementType type)
        {
            _type = type;
            _followBehaviour.SetMovementType(type);
        }

        public void SetTarget(Transform target) => _target = target;
        public void SetSpeed(float speed) => _followBehaviour.SteeringData.SetSpeed(speed);
        public void AddOnMoveFinished(Action onFinish) => _followBehaviour.OnMoveFinished += onFinish;
        public void RemoveOnMoveFinished(Action onFinish) => _followBehaviour.OnMoveFinished -= onFinish;

        public void ToggleMovement(bool active)
        {
            _moveActive = active;
            if(!active)
                _rb.velocity = Vector3.zero;
        }
        #endregion
    }
}