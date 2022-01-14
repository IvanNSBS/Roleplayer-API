namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Class that holds the implementation of a Behaviour Tree
    /// and is responsible for executing it
    /// </summary>
    public class BehaviourTree
    {
        #region Fields
        protected BTNode _root;
        protected NodeState _treeState;
        protected Blackboard _blackboard;
        #endregion


        #region Properties
        /// <summary>
        /// The current Tree Proccessing State
        /// </summary>
        public NodeState TreeState => _treeState;
        #endregion


        #region Constructor
        public BehaviourTree() { }
        public BehaviourTree(BTNode root) => _root = root;
        public BehaviourTree(Blackboard bb) => _blackboard = bb; 
        public BehaviourTree(BTNode root, Blackboard bb) 
        {
            _root = root;
            _blackboard = bb; 
        } 
        #endregion


        #region Methods
        /// <summary>
        /// Sets the root of the tree, attaching the behaviour to 
        /// </summary>
        /// <param name="node">The new root node</param>
        public void SetRoot(BTNode node) => _root = node;
        
        /// <summary>
        /// Gets the Root Node for the Behaviour Tree
        /// </summary>
        /// <returns></returns>
        public BTNode GetRoot() => _root;

        /// <summary>
        /// Ticks the tree, updating it's state
        /// </summary>
        /// <returns>The new tree state</returns>
        public NodeState Update() {
            _treeState = _root.Update();
            return _treeState;
        } 
        #endregion
    }
}