using NSubstitute;
using INUlib.Gameplay.AI.BehaviourTrees;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Runtime.Gameplay.AI
{
    public class BehaviourTreeTests
    {
        #region Mock Clases
        public class MockBTNode : BTNode
        {
            public int started = 0;
            public int finished = 0;

            protected override NodeState Evaluate() => NodeState.Success;
            protected override void OnStart() => started++;
            protected override void OnFinish() => finished++;
        } 
        #endregion


        #region Setup
        #endregion


        #region Tests
        [Test]
        public void Behaviour_Tree_Visits_All_Childrens()
        {
            MockBTNode node = new MockBTNode();

            SequenceNode seq1 = new SequenceNode(new List<BTNode>{node, node, node});
            SequenceNode seq2 = new SequenceNode(new List<BTNode>{node, node, node});
            SequenceNode root = new SequenceNode(new List<BTNode>{seq1, seq2});
            
            BehaviourTree tree = new BehaviourTree(root);

            for(int i = 0; i < 6; i++)
            {
                tree.Update();
                if(i < 5)
                    Assert.IsTrue(tree.TreeState == NodeState.Running);
            }

            Assert.IsTrue(node.started == 6);
            Assert.IsTrue(tree.TreeState == NodeState.Success);
        }
        #endregion
    }
}
