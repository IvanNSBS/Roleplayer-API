using System;
using INUlib.Utils;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Wait action. Waits an amount of time before succeeding
    /// </summary>
    [Serializable]
    public class WaitAction : ActionNode
    {
        #region Fields
        private Timer _timer;
        private float _waitTime;
        #endregion


        #region Properties
        public virtual float ElapsedTime => _timer.ElapsedTime();
        #endregion


        #region Constructors
        public WaitAction(){
            _timer = new Timer();
            _waitTime = 0;
        } 

        public WaitAction(float waitTime) { 
            _timer = new Timer();
            _waitTime = waitTime;
        }
        #endregion


        #region Methods
        public void SetWaitTime(float waitTime) => _waitTime = waitTime;
        protected override NodeState Evaluate() => ElapsedTime >= _waitTime ? NodeState.Success : NodeState.Running;
        protected override void OnStart() => _timer.Restart();
        protected override void OnFinish() => _timer.Restart();
        #endregion
    }
}