using UnityEditorInternal;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class BTNode
    {
        #region Fields
        private bool _started;
        protected NodeState _state;
        #endregion

        #region Properties
        public NodeState State => _state;
        #endregion

        #region Methods
        public NodeState Update()
        {
            if(!_started)
            {
                _started = true;
                OnStart();
            }

            _state = OnUpdate();

            if(_state != NodeState.Running)
                OnFinish();

            return _state;
        }

        protected abstract void OnStart();
        protected abstract void OnFinish();
        protected abstract NodeState OnUpdate();
        #endregion
    }
}