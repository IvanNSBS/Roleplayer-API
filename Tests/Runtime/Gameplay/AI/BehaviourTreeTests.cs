using INUlib.Gameplay.AI.BehaviourTrees;
using NSubstitute;
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
            public override IReadOnlyList<BTNode> GetChildren() => null;
        } 
        #endregion


        #region Setup
        #endregion


        #region Tests
        [Test]
        public void Behaviour_Tree_Visits_All_Childrens_On_Tick()
        {
            MockBTNode node = new MockBTNode();

            SequenceNode seq1 = new SequenceNode(new List<BTNode>{node, node, node});
            SequenceNode seq2 = new SequenceNode(new List<BTNode>{node, node, node});
            SequenceNode root = new SequenceNode(new List<BTNode>{seq1, seq2});
            
            BehaviourTree tree = new BehaviourTree(root);
            tree.Start();

            for(int i = 0; i < 6; i++)
            {
                tree.Update();
                if(i < 5)
                    Assert.IsTrue(tree.TreeState == NodeState.Running);
            }

            Assert.IsTrue(node.started == 6);
            Assert.IsTrue(tree.TreeState == NodeState.Success);
        }

        [Test]
        public void Behaviour_Tree_Properly_Sets_All_Children_Blackboard_On_Start()
        {
            List<BTNode> allNodes = new List<BTNode>();

            SequenceNode seq1 = new SequenceNode();
            SequenceNode seq2 = new SequenceNode();
            
            for(int i = 0; i < 4; i++)
            {
                BTNode mock = Substitute.For<BTNode>();
                seq1.AddChild(mock);
                allNodes.Add(mock);
            }
                        
            for(int i = 0; i < 4; i++)
            {
                BTNode mock = Substitute.For<BTNode>();
                seq2.AddChild(mock);
                allNodes.Add(mock);
            }

            SequenceNode root = new SequenceNode(new List<BTNode>{seq1, seq2});
            allNodes.Add(seq1);
            allNodes.Add(seq2);
            allNodes.Add(root);

            BehaviourTree tree = new BehaviourTree(root);
            tree.Start();

            bool allShareSameBB = true;
            foreach(var node in allNodes)
            {
                if(node.Blackboard != tree.Blackboard)
                {
                    allShareSameBB = false;
                    break;
                }
            }

            Assert.IsTrue(allShareSameBB);
        }
        #endregion
    }
}
