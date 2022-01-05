using UnityEngine;

namespace INUlib.Core.ManagedTweens
{
    /// <summary>
    /// ManagedTarget for a Unity Transform
    /// Resets the transform scale, location and rotation
    /// to the given values
    /// </summary>
    public class ManagedTransform : IManagedTarget
    {
        #region Fields
        private Transform _t;
        private Vector3 _defaultScale = Vector3.one;
        private Vector3? _defaultPosition = Vector3.zero;
        private Quaternion _defaultRotation = Quaternion.identity;
        #endregion

        #region Properties
        public Transform Transform => _t;
        #endregion


        #region Constructors
        public ManagedTransform(Transform t) {
            _t = t;
            _defaultScale = t.localScale;
            _defaultPosition = t.position;
            _defaultRotation = t.rotation;
        }

        public ManagedTransform(Transform t, Vector3 dfScale, Vector3 dfPos, Quaternion dfRotation)
        {
            _t = t;
            _defaultScale = dfScale;
            _defaultPosition = dfPos;
            _defaultRotation = dfRotation;
        }

        public ManagedTransform(Transform t, Vector3 dfScale, Quaternion dfRotation)
        {
            _t = t;
            _defaultScale = dfScale;
            _defaultPosition = null;
            _defaultRotation = dfRotation;
        }
        #endregion


        #region Methods
        public void Reset()
        {
            _t.rotation = _defaultRotation;
            _t.localScale = _defaultScale;

            if(_defaultPosition.HasValue)
                _t.localPosition = _defaultPosition.Value;
        }
        #endregion
    }
}