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

            protected override void OnReapply(MockBaseStatusEffect effect)
            {
                _activeTime = 0f;
                reaplied = true;
            }
        }

        private StatusEffectManager _manager;
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
            _manager = new StatusEffectManager();
            _mockEffect = Substitute.For<IStatusEffect>();
            _mockBase = Substitute.ForPartsOf<MockBaseStatusEffect>(_targetDuration);

            _mockEffect.When(x => x.Apply()).Do(x => _applied = true);
            _mockEffect.When(x => x.OnDispel()).Do(x => _dispelled = true);
            _mockEffect.When(x => x.OnComplete()).Do(x => _completed = true);
            _mockEffect.When(x => x.Reapply(Arg.Any<IStatusEffect>())).Do(x => _reapplied = true);
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
        #endregion
    }
}