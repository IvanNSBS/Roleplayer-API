using NSubstitute;
using NUnit.Framework;
using INUlib.RPG.AbilitiesSystem;

namespace Tests.Runtime.RPG
{
    public class CastHandlerTests
    {
        #region Setup
        private CastHandler _handler;
        private float channelingTime = 1f;
        private float castTime = 1f;
        private float recoveryTime = 1f;
        private CastingState _castingState = CastingState.None;
        private CastingState CastStateGetter() => _castingState;

        private int _abilityDrawGizmosCalled = 0;
        private int _abilityUpdatesCalled = 0;
        
        [SetUp]
        public void Setup()
        {
            _abilityUpdatesCalled = 0;
            _abilityDrawGizmosCalled = 0;
            
            AbilityObject obj = Substitute.ForPartsOf<AbilityObject>();
            CastHandlerPolicy policy = Substitute.For<CastHandlerPolicy>();
            TimelineData timelineData = new TimelineData(channelingTime, castTime, recoveryTime, AbilityCastType.FireAndForget);

            obj.When(x => x.OnUpdate(Arg.Any<float>())).Do(x =>
            {
                _abilityUpdatesCalled++;
            });
            
            obj.When(x => x.OnDrawGizmos()).Do(x =>
            {
                _abilityDrawGizmosCalled++;
            });
            
            CastObjects objs = new CastObjects(policy, obj, timelineData); 
            _handler = new CastHandler(objs, CastStateGetter);
        }
        #endregion


        #region Tests
        [Test]
        public void Casthandler_Properly_Setup_Timeline()
        {
            Assert.AreNotEqual(TimelineState.Pending, _handler.Timeline.state);
            Assert.AreEqual(0f, _handler.Timeline.ElapsedTime, "Elapsed Time was not correct");
            Assert.AreEqual(0f, _handler.Timeline.CompletePercent, "Complete Percent was not correct");
        }

        [Test]
        [TestCase(0.2453f)]
        [TestCase(0.546f)]
        [TestCase(0.856f)]
        [TestCase(0.1112f)]
        [TestCase(0.7752f)]
        public void CastHandler_Properly_Updates_Timeline(float deltaTime)
        {
            _handler.Update(deltaTime);
            Assert.AreEqual(deltaTime, _handler.Timeline.ElapsedTime);
        }
        
        [Test]
        [TestCase(0.2453f)]
        [TestCase(0.546f)]
        [TestCase(0.856f)]
        [TestCase(0.1112f)]
        [TestCase(0.7752f)]
        public void CastHandler_Properly_Updates_AbilityObject(float deltaTime)
        {
            _handler.Update(deltaTime);
            Assert.AreEqual(1, _abilityUpdatesCalled);
        }
        
        [Test]
        public void CastHandler_Properly_Calls_AbilityObject_DrawGizmos()
        {
            _handler.DrawGizmos();
            Assert.AreEqual(1, _abilityDrawGizmosCalled);
        }
        #endregion
    }
}