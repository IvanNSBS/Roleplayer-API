namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class DecoratorNode : BTNode
    {
        #region Fields
        protected BTNode _child;
        #endregion

        #region Properties
        public BTNode Child => _child;
        #endregion


        #region Constructors
        public DecoratorNode() { }
        public DecoratorNode(BTNode child) => _child = child;
        #endregion


        #region Methods
        public void SetChild(BTNode child) => _child = child;
        #endregion
    }
}