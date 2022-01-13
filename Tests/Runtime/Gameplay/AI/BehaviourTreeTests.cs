using INUlib.Gameplay.AI.BehaviourTrees;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

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

        [Test]
        public void Sequence_Succeeds_If_It_Has_No_Children()
        {
            SequenceNode seq = new SequenceNode();
            Assert.IsTrue(seq.Update() == NodeState.Success);
        }

        [Test]
        [TestCase(4, 1)]
        [TestCase(4, 2)]
        [TestCase(2, 0)]
        [TestCase(4, 3)]
        public void Sequence_Node_Fails_If_One_Child_Fails(int childsCount, int failsAtIdx)
        {
            SequenceNode seq = new SequenceNode();

            for(int i = 0; i < childsCount; i++)
            {
                var mock = Substitute.For<BTNode>();
                mock.Update().Returns( i == failsAtIdx ? NodeState.Failure : NodeState.Success);
                seq.AddChild(mock);
            }

            for(int i = 0; i <= failsAtIdx; i++)
            {
                if(i == failsAtIdx)
                    Assert.IsTrue(seq.Update() == NodeState.Failure);
                else
                    Assert.IsTrue(seq.Update() == NodeState.Running);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Sequence_Node_Succeds_When_All_Childs_Succeeds(int childsCount)
        {
            SequenceNode seq = new SequenceNode();

            for(int i = 0; i < childsCount; i++)
            {
                var mock = Substitute.For<BTNode>();
                mock.Update().Returns(NodeState.Success);
                seq.AddChild(mock);
            }

            if(childsCount == 0)
            {
                Assert.IsTrue(seq.Update() == NodeState.Success);
            }
            else
            {
                for(int i = 0; i < childsCount; i++)
                {
                    if(i == childsCount - 1)
                        Assert.IsTrue(seq.Update() == NodeState.Success);
                    else
                        Assert.IsTrue(seq.Update() == NodeState.Running);
                }
            }
        }

        [Test]
        public void Sequence_Node_Stays_Running_When_A_Child_Is_Running()
        {
            var mock = Substitute.For<BTNode>();
            mock.Update().Returns(NodeState.Running);

            SequenceNode seq = new SequenceNode();
            seq.AddChild(mock);
            
            Assert.IsTrue(seq.Update() == NodeState.Running);
        }

        [Test]
        [TestCase(NodeState.Success)]
        [TestCase(NodeState.Failure)]
        public void Sequence_Properly_Resets_After_Finishing(NodeState finishState)
        {
            SequenceNode seq = new SequenceNode();
            int finishIdx = finishState == NodeState.Success ? 2 : 1;

            for(int i = 0; i < 3; i++)
            {
                var mock = Substitute.For<BTNode>();
                mock.Update().Returns(i == finishIdx ? finishState : NodeState.Success);
                seq.AddChild(mock);
            }

            for(int j = 0; j < 2; j++)
            {
                for(int i = 0; i <= finishIdx; i++)
                {
                    var expected = i == finishIdx ? finishState : NodeState.Running;

                    var update = seq.Update();
                    Assert.IsTrue(update == expected);
                }
            }
        }
        #endregion
    }
}