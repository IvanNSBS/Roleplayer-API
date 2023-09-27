using System;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Wait action. Waits an amount of time before succeeding
    /// </summary>
    [Serializable]
    public class WaitAction : ActionNode
    {
        #region Fields
        private float _currentTime;
        private float _waitTime;
        #endregion


        #region Properties
        public virtual float ElapsedTime => _currentTime;
        #endregion


        #region Constructors
        public WaitAction() {
            _waitTime = 0;
        } 

        public WaitAction(float waitTime) { 
            _currentTime = 0;
            _waitTime = waitTime;
        }
        #endregion


        #region Methods
        public void SetWaitTime(float waitTime) => _waitTime = waitTime;
        protected override NodeState Evaluate(float deltaTime)
        {
            _currentTime += deltaTime;
            return ElapsedTime >= _waitTime ? NodeState.Success : NodeState.Running;
        } 
        protected override void OnStart() => _currentTime = 0;
        protected override void OnFinish() => _currentTime = 0;
        #endregion
    }
}