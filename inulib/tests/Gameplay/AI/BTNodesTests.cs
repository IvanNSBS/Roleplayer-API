using INUlib.Gameplay.AI.BehaviourTrees;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Tests.Runtime.Gameplay.AI
{
    public class BTNodesTests
    {
        #region Mock Clases
        public class MockBTNode : BTNode
        {
            public int started = 0;
            public int finished = 0;

            protected override NodeState Evaluate(float deltaTime) => NodeState.Success;
            protected override void OnStart() => started++;
            protected override void OnFinish() => finished++;
            public override IReadOnlyList<BTNode> GetChildren() => null;
        } 
        #endregion

        #region Setup
        #endregion


        #region BT Node Tests
        [Test]
        public void Behaviour_Tree_Node_Triggers_All_Events()
        {
            MockBTNode node = new MockBTNode();
            node.Update(0);

            Assert.IsTrue(node.State == NodeState.Success);
            Assert.IsTrue(node.finished == 1 && node.started == 1);
        }

        [Test]
        public void Behaviour_Tree_Node_Correctly_Restarts_After_Finish()
        {
            MockBTNode node = new MockBTNode();

            node.Update(0);
            Assert.That(node.State == NodeState.Success, Is.True);

            node.Update(0);
            Assert.Multiple(() =>
            {
                Assert.That(node.State, Is.EqualTo(NodeState.Success));
                Assert.That(node.finished == 2 && node.started == 2, Is.True);
            });
        }

        [Test]
        public void Composite_Properly_Sets_Node_As_Its_Child()
        {
            BTNode mock = Substitute.For<BTNode>();

            CompositeNode composite = Substitute.ForPartsOf<CompositeNode>();
            composite.AddChild(mock);

            Assert.That(mock.Parent, Is.EqualTo(composite));
        }

        [Test]
        public void Decorator_Properly_Sets_Node_As_Its_Child()
        {
            BTNode mock = Substitute.For<BTNode>();
            DecoratorNode decorator = Substitute.ForPartsOf<DecoratorNode>(mock);

            Assert.That(mock.Parent, Is.EqualTo(decorator));
        }

        [Test]
        public void Decorator_Node_Properly_Returns_Childrens()
        {
            BTNode mock = Substitute.For<BTNode>();
            DecoratorNode decorator = Substitute.ForPartsOf<DecoratorNode>();
            decorator.SetChild(mock);

            Assert.That(decorator.GetChildren(), Does.Contain(mock));
        }

        [Test]
        public void Composite_Node_Properly_Returns_Childrens()
        {
            BTNode mock = Substitute.For<BTNode>();
            CompositeNode composite = Substitute.ForPartsOf<CompositeNode>();
            composite.AddChild(mock);

            Assert.That(composite.GetChildren(), Does.Contain(mock));
        }

        [Test]
        public void Action_Node_Has_No_Children()
        {
            ActionNode action = Substitute.ForPartsOf<ActionNode>();
            Assert.That(action.GetChildren(), Is.Null);
        }
        #endregion


        #region Sequence Node Tests
        [Test]
        public void Sequence_Succeeds_If_It_Has_No_Children()
        {
            SequenceNode seq = new SequenceNode();
            Assert.That(seq.Update(0), Is.EqualTo(NodeState.Success));
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
                mock.Update(0).Returns( i == failsAtIdx ? NodeState.Failure : NodeState.Success);
                seq.AddChild(mock);
            }

            for(int i = 0; i <= failsAtIdx; i++)
            {
                if(i == failsAtIdx)
                    Assert.That(seq.Update(0), Is.EqualTo(NodeState.Failure));
                else
                    Assert.That(seq.Update(0), Is.EqualTo(NodeState.Running));
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
                mock.Update(0).Returns(NodeState.Success);
                seq.AddChild(mock);
            }

            if(childsCount == 0)
            {
                Assert.That(seq.Update(0), Is.EqualTo(NodeState.Success));
            }
            else
            {
                for(int i = 0; i < childsCount; i++)
                {
                    if(i == childsCount - 1)
                        Assert.That(seq.Update(0), Is.EqualTo(NodeState.Success));
                    else
                        Assert.That(seq.Update(0), Is.EqualTo(NodeState.Running));
                }
            }
        }

        [Test]
        public void Sequence_Node_Stays_Running_When_A_Child_Is_Running()
        {
            var mock = Substitute.For<BTNode>();
            mock.Update(0).Returns(NodeState.Running);

            SequenceNode seq = new SequenceNode();
            seq.AddChild(mock);
            
            Assert.That(seq.Update(0), Is.EqualTo(NodeState.Running));
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
                mock.Update(0).Returns(i == finishIdx ? finishState : NodeState.Success);
                seq.AddChild(mock);
            }

            for(int j = 0; j < 2; j++)
            {
                for(int i = 0; i <= finishIdx; i++)
                {
                    var expected = i == finishIdx ? finishState : NodeState.Running;

                    var update = seq.Update(0);
                    Assert.That(update, Is.EqualTo(expected));
                }
            }
        }
        #endregion


        #region Selector Node Tests
        [Test]
        public void Selector_Succeeds_If_It_Has_No_Children()
        {
            SelectorNode seq = new SelectorNode();
            Assert.That(seq.Update(0), Is.EqualTo(NodeState.Success));
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
                mock.Update(0).Returns(NodeState.Failure);
                seq.AddChild(mock);
            }

            for(int i = 0; i < childsCount; i++)
            {
                if(i == childsCount - 1)
                    Assert.That(seq.Update(0), Is.EqualTo(NodeState.Failure));
                else
                    Assert.That(seq.Update(0), Is.EqualTo(NodeState.Running));
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
                mock.Update(0).Returns(i == successAt ? NodeState.Success : NodeState.Failure);
                seq.AddChild(mock);
            }

            for(int i = 0; i <= successAt; i++)
            {
                if(i == successAt)
                    Assert.That(seq.Update(0), Is.EqualTo(NodeState.Success));
                else
                    Assert.That(seq.Update(0), Is.EqualTo(NodeState.Running));
            }
        }

        [Test]
        public void Selector_Node_Stays_Running_When_A_Child_Is_Running()
        {
            var mock = Substitute.For<BTNode>();
            mock.Update(0).Returns(NodeState.Running);

            SelectorNode seq = new SelectorNode();
            seq.AddChild(mock);
            
            Assert.That(seq.Update(0), Is.EqualTo(NodeState.Running));
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
                mock.Update(0).Returns(i == finishIdx ? finishState : NodeState.Failure);
                seq.AddChild(mock);
            }

            // if it runs correctly after the second time, it has been properly reset
            for(int j = 0; j < 2; j++)
            {
                for(int i = 0; i <= finishIdx; i++)
                {
                    var expected = i == finishIdx ? finishState : NodeState.Running;
                    Assert.That(seq.Update(0), Is.EqualTo(expected));
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
            mock.Update(0).Returns(expected);

            RepeaterDecorator repeater = new RepeaterDecorator(repeatAmnt, mock);
            for(int i = 0; i < repeatAmnt; i++) {
                NodeState actualExpected = i == repeatAmnt - 1 ? expected : NodeState.Running;
                Assert.That(repeater.Update(0), Is.EqualTo(actualExpected));
            }
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
            
            for(int i = 0; i < repeatAmnt; i++) {
                NodeState expected = i == repeatAmnt - 1 ? NodeState.Success : NodeState.Running;
                Assert.That(repeater.Update(0), Is.EqualTo(expected));
            }
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

            Assert.That(mockWait.Update(0), Is.EqualTo(expected));
        }
        #endregion


        #region InverterDecorator Node Tests
        [Test]
        [TestCase(NodeState.Running, NodeState.Running)]
        [TestCase(NodeState.Failure, NodeState.Success)]
        [TestCase(NodeState.Success, NodeState.Failure)]
        public void Inverter_Correctly_Inverts(NodeState state, NodeState expected)
        {
            var mockNode = Substitute.For<BTNode>();
            mockNode.Update(0).Returns(state);

            InverterDecorator inverter = new InverterDecorator(mockNode);
            Assert.That(inverter.Update(0), Is.EqualTo(expected));
        }
        #endregion
    }
}