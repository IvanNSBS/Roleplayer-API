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
        private float overChannellingTime = 1f;
        private float castTime = 1f;
        private float recoveryTime = 1f;
        private CastingState _castingState = CastingState.None;
        private CastingState CastStateGetter() => _castingState;

        private int _abilityDrawGizmosCalled = 0;
        private int _abilityUpdatesCalled = 0;
        
        private float _elapsedOverChannel = 0;
        private float _maxOverchannel = 0;

        public bool finishConcentration;
        
        [SetUp]
        public void Setup()
        {
            finishConcentration = false;
            _abilityUpdatesCalled = 0;
            _abilityDrawGizmosCalled = 0;
            _elapsedOverChannel = 0;
            _maxOverchannel = 0;
            
            AbilityBehaviour obj = Substitute.ForPartsOf<AbilityBehaviour>();
            IAbilityBase parent = Substitute.For<IAbilityBase>();
            TimelineData timelineData = new TimelineData(channelingTime, overChannellingTime, castTime, recoveryTime, 0, AbilityCastType.FireAndForget);

            obj.When(x => x.OnUpdate(Arg.Any<float>(), Arg.Any<CastingState>())).Do(x =>
            {
                _abilityUpdatesCalled++;
            });
            
            obj.When(x => x.OnDrawGizmos()).Do(x =>
            {
                _abilityDrawGizmosCalled++;
            });
            
            obj.When(x => x.OnOverchannel(Arg.Any<float>(), Arg.Any<float>())).Do(x =>
            {
                _elapsedOverChannel = (float)x[0];
                _maxOverchannel = (float)x[1];
            });
            
            CastObjects objs = new CastObjects(obj, timelineData, () => finishConcentration); 
            _handler = new CastHandler(parent, objs, CastStateGetter);
        }
        #endregion


        #region Tests
        [Test]
        public void Casthandler_Properly_Setup_Timeline()
        {
            Assert.AreNotEqual(TimelineState.Pending, _handler.Timeline.state);
            Assert.AreEqual(0f, _handler.Timeline.TotalElapsedTime, "Elapsed Time was not correct");
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
            Assert.AreEqual(deltaTime, _handler.Timeline.TotalElapsedTime);
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

        [Test]
        public void CastHandler_Properly_Finishes_Concentration_On_Update()
        {
            int fired = 0;
            _handler.Timeline.Timeline_And_Recovery_Finished += () => fired++;
            _handler.Update(channelingTime);
            _handler.Update(overChannellingTime);
            finishConcentration = true;
            
            _handler.Update(recoveryTime);
            _handler.Update(recoveryTime);
            Assert.AreEqual(1, fired, $"Expected timeline to finish and fire 1 time, but it was fired {fired} times");
        }

        [Test]
        public void CastHandler_Properly_Notifies_Ability_Object_Of_Overchanneling()
        {
            float elapsed = 0.2f;
            
            _handler.Update(channelingTime);
            _castingState = CastingState.OverChanneling;
            
            _handler.Update(elapsed);
            
            Assert.That(_elapsedOverChannel, Is.EqualTo(elapsed).Within(0.0001f), "Elapsed Overchanneling time was not the same as the passed parameter");
            Assert.AreEqual(overChannellingTime, _maxOverchannel, "Overchannel Duration was not the same as the passed parameter");
        }
        #endregion
    }
}