using INUlib.Gameplay.AI.BehaviourTrees;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Runtime.Gameplay.AI
{
    public class BehaviourTreeTests
    {
        #region Mock Clases
        public class MockBTNode : BTNode
        {
            public int started = 0;
            public int finished = 0;

            protected override NodeState OnUpdate() => NodeState.Success;
            protected override void OnStart() => started++;
            protected override void OnFinish() => finished++;
        } 
        #endregion

        #region Setup
        #endregion


        #region Tests
        [Test]
        public void Behaviour_Tree_Node_Triggers_All_Events()
        {
            MockBTNode node = new MockBTNode();
            node.Update();

            Assert.IsTrue(node.State == NodeState.Success);
            Assert.IsTrue(node.finished == 1 && node.started == 1);
        }

        [Test]
        public void Behaviour_Tree_Node_Correctly_Restarts_After_Finish()
        {
            MockBTNode node = new MockBTNode();

            node.Update();
            Assert.IsTrue(node.State == NodeState.Success);

            node.Update();
            Assert.IsTrue(node.State == NodeState.Success);
            Assert.IsTrue(node.finished == 2 && node.started == 2);
        }
        #endregion
    }
}