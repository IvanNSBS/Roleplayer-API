using Godot;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Class that uses a BehaviourTree, creating and updating it on each frame
    /// </summary>
    public partial class BehaviourTreeUser : Node
    {
        #region Inspector Fields
        // [Export]
        // protected BehaviourTreeAsset _btAsset;
        #endregion

        #region Fields
        protected BehaviourTree _bt;
        #endregion

        #region Properties
        public BehaviourTree Tree => _bt;
        #endregion


        #region MonoBehaviour Methods
        public override void _Ready()
        {
            _bt = new BehaviourTree();
            // _btAsset.SetupTree(_bt);
            _bt.Start();
        }

        public override void _Process(double deltaTime)
        {
            _bt?.Update((float)deltaTime);
        }
        #endregion

    }
}