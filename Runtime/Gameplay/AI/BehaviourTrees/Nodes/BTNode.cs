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

            _state = Evaluate();

            if(_state != NodeState.Running)
            {
                _started = false;                
                OnFinish();
            }

            return _state;
        }

        protected abstract void OnStart();
        protected abstract void OnFinish();
        protected abstract NodeState Evaluate();
        #endregion
    }
}