using System;
using INUlib.Core;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Wait action. Waits an amount of time before succeeding
    /// </summary>
    [Serializable]
    public class DebugLogAction : ActionNode
    {
        #region Fields
        private string _logMsg;
        #endregion


        #region Constructors
        public DebugLogAction(string logMsg) => _logMsg = logMsg; 
        #endregion


        #region Methods
        protected override NodeState Evaluate(float deltaTime) {
            Logger.Debug(_logMsg);
            return NodeState.Success;
        }

        protected override void OnStart() { }
        protected override void OnFinish() { }
        #endregion
    }
}