namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class BehaviourTree
    {
        #region Fields
        private BTNode _root;
        private NodeState _treeState;
        private Blackboard _blackboard;
        #endregion


        #region Properties
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
        public void SetRoot(BTNode node)
        {
            _root = node;
        }

        public BTNode GetRoot() => _root;
        public void Update() => _root.Update();
        #endregion
    }
}