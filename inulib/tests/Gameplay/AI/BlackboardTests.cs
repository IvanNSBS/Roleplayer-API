using NSubstitute;
using NUnit.Framework;
using INUlib.Gameplay.AI.BehaviourTrees;

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

            Assert.That(_blackboard.GetProperty<int>("val"), Is.EqualTo(val));
        }

        [Test]
        public void Blackboard_Properly_Sets_And_Sets_Reference_Property()
        {
            BTNode node = Substitute.For<BTNode>();
            _blackboard.SetProperty(node, "node");

            Assert.That(_blackboard.GetProperty<BTNode>("node"), Is.EqualTo(node));
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Blackboard_Correctly_Checks_If_Property_Exists(bool exists)
        {
            if(exists)
                _blackboard.SetProperty(5, "val");

            Assert.That(_blackboard.HasProperty("val"), Is.EqualTo(exists));
        }
        #endregion
    }
}