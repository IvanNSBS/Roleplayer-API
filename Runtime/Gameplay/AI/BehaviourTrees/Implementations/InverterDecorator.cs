namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class InverterDecorator : DecoratorNode
    {
        #region Constructors
        public InverterDecorator() : base() { }
        public InverterDecorator(BTNode child) : base(child) { }
        #endregion


        #region Methods
        protected override NodeState Evaluate()
        {
            if(_child == null) 
                return NodeState.Success; 

            switch(_child.Update())
            {
                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Success:
                    return NodeState.Failure;

                case NodeState.Failure:
                    return NodeState.Success;

                default:
                    return NodeState.Success;
            }
        }

        protected override void OnStart() { }
        protected override void OnFinish() { }
        #endregion
    }
}