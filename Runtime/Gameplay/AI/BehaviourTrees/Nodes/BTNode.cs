using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Base Behaviour Tree Node
    /// </summary>
    public abstract class BTNode
    {
        #region Fields
        private bool _started;
        protected NodeState _state;
        protected BTNode _parent;
        #endregion

        #region Properties
        public NodeState State => _state;
        public BTNode Parent => _parent;
        #endregion

        #region Methods
        /// <summary>
        /// Updtes the state of the tree, returning whether it's running 
        /// or have failed/succeeded
        /// </summary>
        /// <returns>The current node state</returns>
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

        /// <summary>
        /// Gets all the children nodes
        /// </summary>
        /// <returns>A list containing all of the node childrens</returns>
        public abstract IReadOnlyList<BTNode> GetChildren();

        /// <summary>
        /// Sets the Node parent
        /// </summary>
        /// <param name="parent">The new node parent</param>
        public void SetParent(BTNode parent) => _parent = parent;

        /// <summary>
        /// Function to run when the tree have node have started proccessing
        /// for the first time of after it began runinng after it had finished proccessing
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Function to run when the BTNode had finished processing and returned
        /// Success or Failure
        /// </summary>
        protected abstract void OnFinish();

        /// <summary>
        /// Evaluates the Node state, returning whether it's running or has completed with
        /// a failure or success
        /// </summary>
        /// <returns>The current node state</returns>
        protected abstract NodeState Evaluate();
        #endregion
    }
}