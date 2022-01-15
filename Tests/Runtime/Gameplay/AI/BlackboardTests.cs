using System;
using System.Collections.Generic;
using INUlib.Gameplay.AI.BehaviourTrees;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Runtime.Gameplay.AI
{
    public class BlackboardTests
    {
        #region Setup
        private Blackboard _blackboard;

        [SetUp]
        public void Setup() => _blackboard = new Blackboard();
        #endregion

        #region Tests
        [Test]
        public void Blackboard_Properly_Gets_And_Sets_Value_Property()
        {
            int val = 5;
            _blackboard.SetProperty(val, "val");

            Assert.IsTrue(_blackboard.GetProperty<int>("val") == val);
        }

        [Test]
        public void Blackboard_Properly_Sets_And_Sets_Reference_Property()
        {
            BTNode node = Substitute.For<BTNode>();
            _blackboard.SetProperty(node, "node");

            Assert.IsTrue(_blackboard.GetProperty<BTNode>("node") == node);
        }

        [Test]
        public void Blackboard_Throws_Exception_When_Property_Doesnt_Exist()
        {
            Assert.Throws<KeyNotFoundException>(() => _blackboard.GetProperty<float>("random_name"));
        }

        [Test]
        public void Blackboard_Cast_Fails_If_Invalid_Type_Is_Provided()
        {
            uint val = 5;
            _blackboard.SetProperty(val, "val");

            Assert.Throws<InvalidCastException>(() => _blackboard.GetProperty<bool>("val"));
        }
        #endregion
    }
}