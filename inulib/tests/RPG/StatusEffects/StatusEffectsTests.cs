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

        private StatusEffectController<IStatusEffect> _manager;
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
            _manager = new StatusEffectController<IStatusEffect>();
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
            Assert.Multiple(() =>
            {
                Assert.That(_applied, Is.True);
                Assert.That(_manager.ActiveEffects, Does.Contain(_mockEffect));
            });
        }

        [Test]
        public void Different_Effect_Is_Correcly_Applied_When_Theres_An_Active_Effet()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.ApplyEffect(_mockBase);
            Assert.Multiple(() =>
            {
                Assert.That(_reapplied, Is.False);
                Assert.That(_manager.ActiveEffects.Contains(_mockEffect) && _manager.ActiveEffects.Contains(_mockBase), Is.True);
            });
        }

        [Test]
        public void Effect_Is_Reapplied_When_Trying_To_Add_An_Effect_Of_The_Same_Type()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.ApplyEffect(_mockEffect);

            Assert.That(_reapplied, Is.True);
        }

        [Test]
        public void Effect_Is_Correctly_Dispelled()
        {
            _manager.ApplyEffect(_mockEffect);
            bool result = _manager.DispelEffect(_mockEffect);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_dispelled, Is.True);
            });
        }

        [Test]
        public void Effect_Is_Correctly_Applied_After_Dispell()
        {
            _manager.ApplyEffect(_mockEffect);
            bool result = _manager.DispelEffect(_mockEffect);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_dispelled, Is.True);
            });

            _applied = false;
            _manager.ApplyEffect(_mockEffect);
            
            Assert.That(_applied, Is.True);
        }

        [Test]
        public void Nothing_Happens_When_Dispeling_An_Effect_That_Is_Not_Active()
        {
            bool result = _manager.DispelEffect(_mockEffect);
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(_dispelled, Is.False);
            });
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

            Assert.That(_elapsedTime, Is.EqualTo(deltaTime));
        }

        [Test]
        public void Effect_Completes_After_Duration()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration);

            Assert.That(_completed, Is.True);
        }

        [Test]
        public void Effect_Is_Removed_After_Completion()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration);

            Assert.That(_manager.ActiveEffects, Does.Not.Contain(_mockEffect));
        }

        [Test]
        public void Base_Status_Effect_Is_Correctly_Applied()
        {
            _manager.ApplyEffect(_mockBase);

            Assert.That(_mockBase.IsActive, Is.True);
        }

        [Test]
        public void Base_Status_Effect_Is_Not_Updated_If_Not_Applied()
        {
            _mockBase.Update(_targetDuration);
            Assert.That(_mockBase.ActiveTime, Is.EqualTo(0f));
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

            Assert.That(_mockBase.ActiveTime, Is.EqualTo(deltaTime));
        }

        [Test]
        public void Base_Status_Effect_Correctly_Finishes()
        {
            _manager.ApplyEffect(_mockBase);
            Assert.That(_mockBase.Update(_targetDuration), Is.True);
        }

        [Test]
        public void Base_Status_Effect_Is_Correctly_Reapplied()
        {
            _manager.ApplyEffect(_mockBase); 
            _manager.Update(_targetDuration*0.8f);
            _manager.ApplyEffect(_mockBase);
            
            Assert.Multiple(() =>
            {
                Assert.That(_mockBase.reaplied, Is.True);
                Assert.That(_mockBase.ActiveTime, Is.EqualTo(0f));
            });
        }

        [Test]
        public void Base_Status_Effect_Immediately_Completes_If_Complete_Is_Called()
        {
            _mockBase.Complete();
            _manager.ApplyEffect(_mockBase);

            Assert.That(_mockBase.Update(0.0f), Is.True);
        }

        [Test]
        public void Endless_Base_Status_Effect_Only_Completes_After_Calling_Complete()
        {
            _mockBase = Substitute.ForPartsOf<MockBaseStatusEffect>(-1);
            _manager.ApplyEffect(_mockBase);
            _manager.Update(150f);

            Assert.That(_mockBase.Update(0.0f), Is.False);
            _mockBase.Complete();
            Assert.That(_mockBase.Update(0.0f), Is.True);
        }

        [Test]
        public void EffectApplyStats_Is_Created_After_Being_Applying_The_First_Time()
        {
            _manager.ApplyEffect(_mockEffect);
            bool containsKey = _manager.AddedEffectStats.ContainsKey(_mockEffect.GetType());
            
            Assert.Multiple(() =>
            {
                Assert.That(containsKey, Is.True);
                Assert.That(_manager.AddedEffectStats[_mockEffect.GetType()].TimesApplied, Is.EqualTo(1));
            });
        }

        [Test]
        public void EffectApplyStats_Correctly_Updates_Inactive_Time()
        {
            float elapsedTime = _targetDuration * 2;
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration * 2f);

            Assert.That(_manager.GetEffectApplyStats(_mockEffect).InactiveTime, Is.EqualTo(elapsedTime));
        }

        [Test]
        public void EffectApplyStats_Wont_Update_Inactive_Time_If_Effect_Is_Active()
        {
            _manager.ApplyEffect(_mockBase);
            _manager.Update(_targetDuration * 0.5f);

            Assert.That(_manager.GetEffectApplyStats(_mockBase).InactiveTime, Is.EqualTo(0));
        }

        [Test]
        public void EffectApplyStats_Resets_Inactive_Time_After_Applying_Effect_Again()
        {
            float elapsedTime = _targetDuration * 2;
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(_targetDuration * 2f);
            _manager.ApplyEffect(_mockEffect);

            Assert.That(_manager.GetEffectApplyStats(_mockEffect).InactiveTime, Is.EqualTo(0));
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

            Assert.That(_manager.GetEffectApplyStats(_mockEffect).TimesApplied, Is.EqualTo(timesApplied));
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

            Assert.That(_manager.GetEffectApplyStats(_mockEffect).SecondsSinceFirstApply, Is.EqualTo(elapsedTime));
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

            Assert.That(_manager.GetEffectApplyStats(_mockEffect).SecondsSinceLastApply, Is.EqualTo(elapsedTime));
        }

        [Test]
        public void EffectApplyStats_Resets_Time_Since_Last_Apply_After_Reapply()
        {
            _manager.ApplyEffect(_mockEffect);
            _manager.Update(5f);
            _manager.ApplyEffect(_mockEffect);

            Assert.That(_manager.GetEffectApplyStats(_mockEffect).SecondsSinceLastApply, Is.EqualTo(0));
        }

        [Test]
        public void EffectApplyStats_Is_Reset_After_Target_Inactive_Time()
        {
            _manager.ApplyEffect(_mockBase);
            _manager.Update(_targetDuration);
            _manager.Update(StatusEffectController<IStatusEffect>.DEFAULT_EFFECT_STATS_RESET_TIME);

            Assert.That(_manager.GetEffectApplyStats(_mockBase).TimesApplied, Is.EqualTo(0));
        }

        [Test]
        public void EffectApplyStats_Will_Never_Reset_If_Target_Manager_Inactive_Time_Is_Less_Than_Zero()
        {
            _manager = new StatusEffectController<IStatusEffect>(-1);
            _manager.ApplyEffect(_mockBase);
            _manager.Update(_targetDuration);
            _manager.Update(StatusEffectController<IStatusEffect>.DEFAULT_EFFECT_STATS_RESET_TIME);
        
            Assert.That(_manager.GetEffectApplyStats(_mockBase).TimesApplied, Is.EqualTo(1));
        }

        [Test]
        public void StatusEffectAdded_Event_Is_Called_On_Apply()
        {
            bool added = false;
            _manager.onStatusEffectAdded += x => added = true;
            _manager.ApplyEffect(_mockBase);

            Assert.That(added, Is.True);
        }

        [Test]
        public void StatusEffectAdded_Event_Is_Not_Called_On_Reapply()
        {
            bool added = false;
            _manager.onStatusEffectAdded += x => added = !added;
            _manager.ApplyEffect(_mockBase);
            _manager.ApplyEffect(_mockBase);

            Assert.That(added, Is.True);
        }

        [Test]
        public void StatusEffectRemoved_Event_Is_Called_On_Finish()
        {
            bool removed = false;

            _manager.onStatusEffectFinished += (x,y) => removed = true;
            _manager.ApplyEffect(_mockBase);
            _mockBase.Complete();
            _manager.Update(0.01f);

            Assert.That(removed, Is.True);
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
            
            Assert.Multiple(() =>
            {
                Assert.That(removed, Is.True);
                Assert.That(index, Is.EqualTo(0));
            });
        }
        #endregion
    }
}