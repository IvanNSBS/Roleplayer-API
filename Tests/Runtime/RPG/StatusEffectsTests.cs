using System.Linq;
using NSubstitute;
using NUnit.Framework;
using INUlib.RPG.StatusEffectSystem;

namespace Tests.Runtime.RPG.StatusEffects
{
    public class StatusEffectsTests
    {
        #region Setup
        public abstract class MockBaseStatusEffect : BaseStatusEffect<MockBaseStatusEffect> 
        {
            public bool reaplied = false;
            public float ActiveTime => _activeTime;
            public bool IsActive => _applied;
            public float Duration => _duration;

            public MockBaseStatusEffect(float a) : base(a) { }

            protected override void OnReapply(MockBaseStatusEffect effect, EffectApplyStats stats)
            {
                _activeTime = 0f;
                reaplied = true;
            }
        }

        private StatusEffectManager<IStatusEffect> _manager;
        private IStatusEffect _mockEffect;
        private MockBaseStatusEffect _mockBase;

        private float _elapsedTime;
        private float _targetDuration = 5f;
        private bool _applied;
        private bool _dispelled;
        private bool _completed;
        private bool _reapplied;

        [SetUp]
        public void Setup()
        {
            _applied = false;
            _dispelled = false;
            _completed = false;
            _elapsedTime = 0;
            _manager = new StatusEffectManager<IStatusEffect>();
            _mockEffect = Substitute.For<IStatusEffect>();
            _mockBase = Substitute.ForPartsOf<MockBaseStatusEffect>(_targetDuration);

            _mockEffect.When(x => x.Apply(Arg.Any<EffectApplyStats>())).Do(x => _applied = true);
            _mockEffect.When(x => x.OnDispel()).Do(x => _dispelled = true);
            _mockEffect.When(x => x.OnComplete()).Do(x => _completed = true);
            _mockEffect.When(x => x.Reapply(Arg.Any<IStatusEffect>(), Arg.Any<EffectApplyStats>())).Do(x => _reapplied = true);
            _mockEffect.When(x => x.Update(Arg.Any<float>())).Do(x => _elapsedTime += (float)x[0]);
            _mockEffect.Update(Arg.Any<float>()).Returns(x => _elapsedTime + (float)x[0] >= _targetDuration);
        }
        #endregion


        #region Tests
        [Test]
        public void Effect_Is_Correctly_Applied()
        {
            _manager.ApplyEffect(_mockEffect);
            Assert.IsTrue(_applied);
            Assert.IsTrue(_manager.ActiveEffects.Contains(_mockEffect));
        }

        [Test]
        public void Different_Effect_Is_Correcly_Applied_When_Theres_An_Active_Effet()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.ApplyEffect(_mockBase);

            Assert.IsFalse(_reapplied);
            Assert.IsTrue(_manager.ActiveEffects.Contains(_mockEffect) && _manager.ActiveEffects.Contains(_mockBase));
        }

        [Test]
        public void Effect_Is_Reapplied_When_Trying_To_Add_An_Effect_Of_The_Same_Type()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.ApplyEffect(_mockEffect);

            Assert.IsTrue(_reapplied);
        }

        [Test]
        public void Effect_Is_Correctly_Dispelled()
        {
            _manager.ApplyEffect(_mockEffect);
            bool result = _manager.DispelEffect(_mockEffect);

            Assert.IsTrue(result);
            Assert.IsTrue(_dispelled);
        }

        [Test]
        public void Nothing_Happens_When_Dispeling_An_Effect_That_Is_Not_Active()
        {
            bool result = _manager.DispelEffect(_mockEffect);
            
            Assert.IsFalse(result);
            Assert.IsFalse(_dispelled);
        } 

        [Test]
        [TestCase(0f)]
        [TestCase(0.2f)]
        [TestCase(0.5f)]
        [TestCase(2f)]
        [TestCase(10f)]
        public void Effect_Is_Correctly_Updated(float deltaTime)
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(deltaTime);

            Assert.AreEqual(deltaTime, _elapsedTime);
        }

        [Test]
        public void Effect_Completes_After_Duration()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration);

            Assert.IsTrue(_completed);
        }

        [Test]
        public void Effect_Is_Removed_After_Completion()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration);

            Assert.IsFalse(_manager.ActiveEffects.Contains(_mockEffect));
        }

        [Test]
        public void Base_Status_Effect_Is_Correctly_Applied()
        {
            _manager.ApplyEffect(_mockBase);

            Assert.IsTrue(_mockBase.IsActive);
        }

        [Test]
        public void Base_Status_Effect_Is_Not_Updated_If_Not_Applied()
        {
            _mockBase.Update(_targetDuration);
            Assert.AreEqual(0f, _mockBase.ActiveTime);
        }

        [Test]
        [TestCase(0f)]
        [TestCase(0.3f)]
        [TestCase(0.9f)]
        [TestCase(2.5f)]
        public void Base_Status_Effect_Is_Updated_If_Applied(float deltaTime)
        {
            _manager.ApplyEffect(_mockBase);
            _manager.Update(deltaTime);

            Assert.AreEqual(deltaTime, _mockBase.ActiveTime);
        }

        [Test]
        public void Base_Status_Effect_Correctly_Finishes()
        {
            _manager.ApplyEffect(_mockBase);
            Assert.IsTrue(_mockBase.Update(_targetDuration));
        }

        [Test]
        public void Base_Status_Effect_Is_Correctly_Reapplied()
        {
            _manager.ApplyEffect(_mockBase); 
            _manager.Update(_targetDuration*0.8f);
            _manager.ApplyEffect(_mockBase); 

            Assert.IsTrue(_mockBase.reaplied);
            Assert.AreEqual(0f, _mockBase.ActiveTime);
        }

        [Test]
        public void Base_Status_Effect_Immediately_Completes_If_Complete_Is_Called()
        {
            _mockBase.Complete();
            _manager.ApplyEffect(_mockBase);

            Assert.IsTrue(_mockBase.Update(0.0f));
        }

        [Test]
        public void Endless_Base_Status_Effect_Only_Completes_After_Calling_Complete()
        {
            _mockBase = Substitute.ForPartsOf<MockBaseStatusEffect>(-1);
            _manager.ApplyEffect(_mockBase);
            _manager.Update(150f);

            Assert.IsFalse(_mockBase.Update(0.0f));
            _mockBase.Complete();
            Assert.IsTrue(_mockBase.Update(0.0f));
        }

        [Test]
        public void EffectApplyStats_Is_Created_After_Being_Applying_The_First_Time()
        {
            _manager.ApplyEffect(_mockEffect);
            bool containsKey = _manager.AddedEffectStats.ContainsKey(_mockEffect.GetType());
        
            Assert.IsTrue(containsKey);
            Assert.AreEqual(1, _manager.AddedEffectStats[_mockEffect.GetType()].TimesApplied);
        }

        [Test]
        public void EffectApplyStats_Correctly_Updates_Inactive_Time()
        {
            float elapsedTime = _targetDuration * 2;
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration * 2f);

            Assert.AreEqual(elapsedTime, _manager.GetEffectApplyStats(_mockEffect).InactiveTime);
        }

        [Test]
        public void EffectApplyStats_Wont_Update_Inactive_Time_If_Effect_Is_Active()
        {
            _manager.ApplyEffect(_mockBase);
            _manager.Update(_targetDuration * 0.5f);

            Assert.AreEqual(0, _manager.GetEffectApplyStats(_mockBase).InactiveTime);
        }

        [Test]
        public void EffectApplyStats_Resets_Inactive_Time_After_Applying_Effect_Again()
        {
            float elapsedTime = _targetDuration * 2;
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration * 2f);
            _manager.ApplyEffect(_mockEffect);

            Assert.AreEqual(0, _manager.GetEffectApplyStats(_mockEffect).InactiveTime);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(12)]
        public void EffectApplyStats_Correctly_Updates_Times_Applied(int timesApplied)
        {
            for(int i = 0; i < timesApplied; i++)
                _manager.ApplyEffect(_mockEffect);

            Assert.AreEqual(timesApplied, _manager.GetEffectApplyStats(_mockEffect).TimesApplied);
        }
        
        [Test]
        [TestCase(0f)]
        [TestCase(0.5f)]
        [TestCase(1.2f)]
        [TestCase(5.43f)]
        [TestCase(3.22f)]
        [TestCase(22.5f)]
        public void EffectApplyStats_Correctly_Updates_Time_Since_First_Apply(float elapsedTime)
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(elapsedTime);

            Assert.AreEqual(elapsedTime, _manager.GetEffectApplyStats(_mockEffect).SecondsSinceFirstApply);
        }

        [Test]
        [TestCase(0f)]
        [TestCase(0.5f)]
        [TestCase(1.2f)]
        [TestCase(5.43f)]
        [TestCase(3.22f)]
        [TestCase(22.5f)]
        public void EffectApplyStats_Correctly_Updates_Time_Since_Last_Apply(float elapsedTime)
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(elapsedTime);

            Assert.AreEqual(elapsedTime, _manager.GetEffectApplyStats(_mockEffect).SecondsSinceLastApply);
        }

        [Test]
        public void EffectApplyStats_Resets_Time_Since_Last_Apply_After_Reapply()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(5f);
            _manager.ApplyEffect(_mockEffect);

            Assert.AreEqual(0, _manager.GetEffectApplyStats(_mockEffect).SecondsSinceLastApply);
        }

        [Test]
        public void EffectApplyStats_Is_Reset_After_Target_Inactive_Time()
        {
            _manager.ApplyEffect(_mockBase);
            _manager.Update(_targetDuration);
            _manager.Update(StatusEffectManager<IStatusEffect>.DEFAULT_EFFECT_STATS_RESET_TIME);

            Assert.AreEqual(0, _manager.GetEffectApplyStats(_mockBase).TimesApplied);
        }

        [Test]
        public void EffectApplyStats_Will_Never_Reset_If_Target_Manager_Inactive_Time_Is_Less_Than_Zero()
        {
            _manager = new StatusEffectManager<IStatusEffect>(-1);
            _manager.ApplyEffect(_mockBase);
            _manager.Update(_targetDuration);
            _manager.Update(StatusEffectManager<IStatusEffect>.DEFAULT_EFFECT_STATS_RESET_TIME);
        
            Assert.AreEqual(1, _manager.GetEffectApplyStats(_mockBase).TimesApplied);
        }

        [Test]
        public void StatusEffectAdded_Event_Is_Called_On_Apply()
        {
            bool added = false;
            _manager.onStatusEffectAdded += x => added = true;
            _manager.ApplyEffect(_mockBase);

            Assert.IsTrue(added);
        }

        [Test]
        public void StatusEffectAdded_Event_Is_Not_Called_On_Reapply()
        {
            bool added = false;
            _manager.onStatusEffectAdded += x => added = !added;
            _manager.ApplyEffect(_mockBase);
            _manager.ApplyEffect(_mockBase);

            Assert.IsTrue(added);
        }

        [Test]
        public void StatusEffectRemoved_Event_Is_Called_On_Finish()
        {
            bool removed = false;

            _manager.onStatusEffectFinished += (x,y) => removed = true;
            _manager.ApplyEffect(_mockBase);
            _mockBase.Complete();
            _manager.Update(0.01f);

            Assert.IsTrue(removed);
        }

        [Test]
        public void StatusEffectRemoved_Event_Is_Called_On_Dispell()
        {
            bool removed = false;
            int index = -1;

            _manager.onStatusEffectFinished += (x,y) => {
                removed = true;
                index = y;
            };

            _manager.ApplyEffect(_mockBase);
            _manager.DispelEffect(_mockBase);
            _manager.Update(0.01f);

            Assert.IsTrue(removed);
            Assert.AreEqual(0, index);
        }
        #endregion
    }
}