using INUlib.Gameplay.AI.BehaviourTrees;
using NSubstitute;
using NSubstitute.Core;
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


        #region BT Node Tests
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


        #region Sequence Node Tests
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


        #region Selector Node Tests
        [Test]
        public void Selector_Succeeds_If_It_Has_No_Children()
        {
            SelectorNode seq = new SelectorNode();
            Assert.IsTrue(seq.Update() == NodeState.Success);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Selector_Node_Fails_If_All_Of_Its_Children_Fails(int childsCount)
        {
            SelectorNode seq = new SelectorNode();

            for(int i = 0; i < childsCount; i++)
            {
                var mock = Substitute.For<BTNode>();
                mock.Update().Returns(NodeState.Failure);
                seq.AddChild(mock);
            }

            for(int i = 0; i < childsCount; i++)
            {
                if(i == childsCount - 1)
                    Assert.IsTrue(seq.Update() == NodeState.Failure);
                else
                    Assert.IsTrue(seq.Update() == NodeState.Running);
            }
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(2, 0)]
        [TestCase(3, 1)]
        [TestCase(4, 2)]
        public void Sequence_Node_Succeds_When_One_Child_Succeeds(int childsCount, int successAt)
        {
            SelectorNode seq = new SelectorNode();

            for(int i = 0; i < childsCount; i++)
            {
                var mock = Substitute.For<BTNode>();
                mock.Update().Returns(i == successAt ? NodeState.Success : NodeState.Failure);
                seq.AddChild(mock);
            }

            for(int i = 0; i <= successAt; i++)
            {
                if(i == successAt)
                    Assert.IsTrue(seq.Update() == NodeState.Success);
                else
                    Assert.IsTrue(seq.Update() == NodeState.Running);
            }
        }

        [Test]
        public void Selector_Node_Stays_Running_When_A_Child_Is_Running()
        {
            var mock = Substitute.For<BTNode>();
            mock.Update().Returns(NodeState.Running);

            SelectorNode seq = new SelectorNode();
            seq.AddChild(mock);
            
            Assert.IsTrue(seq.Update() == NodeState.Running);
        }

        [Test]
        [TestCase(NodeState.Success)]
        [TestCase(NodeState.Failure)]
        public void Selector_Properly_Resets_After_Finishing(NodeState finishState)
        {
            SelectorNode seq = new SelectorNode();
            int finishIdx = finishState == NodeState.Failure ? 2 : 1;

            for(int i = 0; i < 3; i++)
            {
                var mock = Substitute.For<BTNode>();
                mock.Update().Returns(i == finishIdx ? finishState : NodeState.Failure);
                seq.AddChild(mock);
            }

            // if it runs correctly after the second time, it has been properly reset
            for(int j = 0; j < 2; j++)
            {
                for(int i = 0; i <= finishIdx; i++)
                {
                    var expected = i == finishIdx ? finishState : NodeState.Running;
                    Assert.IsTrue(seq.Update() == expected);
                }
            }
        }
        #endregion


        #region Repeater Decorator Tests
        [Test]
        [TestCase(0, NodeState.Success)]
        [TestCase(1, NodeState.Success)]
        [TestCase(2, NodeState.Failure)]
        [TestCase(3, NodeState.Success)]
        [TestCase(4, NodeState.Failure)]
        public void Repeater_Only_Completes_After_Repeating_All_Times(int repeatAmnt, NodeState expected)
        {
            BTNode mock = Substitute.For<BTNode>();
            mock.Update().Returns(expected);

            RepeaterDecorator repeater = new RepeaterDecorator(repeatAmnt, mock);
            for(int i = 0; i < repeatAmnt; i++)
                Assert.IsTrue(repeater.Update() == (i == repeatAmnt - 1 ? expected : NodeState.Running));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Repeater_Succeeds_If_Dont_Have_Child(int repeatAmnt)
        {
            RepeaterDecorator repeater = new RepeaterDecorator(repeatAmnt);
            
            for(int i = 0; i < repeatAmnt; i++)
                Assert.IsTrue(repeater.Update() == (i == repeatAmnt - 1 ? NodeState.Success : NodeState.Running));
        }
        #endregion


        #region Wait Action Node Tests
        [Test]
        [TestCase(0.0f, 0.1f, NodeState.Success)]
        [TestCase(1.0f, 0.0f, NodeState.Running)]
        [TestCase(0.2f, 1.0f, NodeState.Success)]
        public void Wait_Action_Finishes_After_Time_Passes(float wait, float elapsed, NodeState expected)
        {
            var mockWait = Substitute.ForPartsOf<WaitAction>(wait);
            mockWait.ElapsedTime.Returns(elapsed);

            Assert.IsTrue(mockWait.Update() == expected);
        }
        #endregion
    }
}