using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;

namespace Tests.Runtime.RPG.Abilities
{
    public class AbilitiesControllerTests
    {
        #region Test Factory Ability
        public class TestFactoryAbility : IAbility<IAbilityDataHub>
        {   
            private IAbilityDataHub _factoryRef;
            public bool isEqual;

            public TestFactoryAbility(float cd, float castTime, IAbilityDataHub factoryRef)
            {
                _factoryRef = factoryRef;
                Cooldown = cd;
                CastTime = castTime;
            }

            public void Cast(IAbilityDataHub dataFactory) => isEqual = dataFactory == _factoryRef;
            public float CurrentCooldown {get; set;}
            public float Cooldown {get; set;}
            public float CastTime {get;}
        }
        #endregion

        #region Mock Tests
        private IAbilityDataHub _mockFactory;
        private AbilitiesController<IAbility<IAbilityDataHub>, IAbilityDataHub> _controller;
        private IAbility<IAbilityDataHub> _mockAbility1;
        private IAbility<IAbilityDataHub> _mockAbility2;
        private IAbility<IAbilityDataHub> _mockAbility3;
        private bool _casted;
        private float _cd = 5;
        private float _castTime = 1;

        [SetUp]
        public void Setup() 
        {
            _casted = false;
            _mockFactory = Substitute.For<IAbilityDataHub>();
            _controller = new AbilitiesController<IAbility<IAbilityDataHub>, IAbilityDataHub>(3, _mockFactory);

            PrepareMockAbility(_mockAbility1, 0);
            PrepareMockAbility(_mockAbility2, 1);
            PrepareMockAbility(_mockAbility3, 2);
        }

        private void PrepareMockAbility(IAbility<IAbilityDataHub> ability, uint slot)
        {
            ability = Substitute.For<IAbility<IAbilityDataHub>>();
            ability.Cooldown.Returns(_cd);
            ability.CastTime.Returns(_castTime);
            ability.When(x => x.Cast(_mockFactory)).Do(x => _casted = true);

            _controller.SetAbility(slot, ability);
        }
        #endregion


        #region Methods
        [Test]
        public void Controller_Is_Constructed_Correctly()
        {
            Assert.IsTrue(_controller.AbilitySlots == 3);
            Assert.IsTrue(_controller.DataFactory == _mockFactory);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_Correct_Data_Factory_For_Casting(uint slot)
        {
            var testAbility = new TestFactoryAbility(_cd, 0, _mockFactory);
            _controller.SetAbility(slot, testAbility);
            _controller.StartCast(slot);

            Assert.IsTrue(testAbility.isEqual);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Is_Set_In_Slot(uint slot)
        {
            var mockAbility = Substitute.For<IAbility<IAbilityDataHub>>();
            _controller.SetAbility(slot, mockAbility);

            Assert.IsTrue(_controller.GetAbility(slot) == mockAbility);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Is_Unleashed_After_Finish_Cast(uint slot)
        {
            _controller.StartCast(slot);
            _controller.Update(1);
            Assert.IsTrue(_casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Finish_Cast(uint slot)
        {
            _controller.StartCast(slot);
            _controller.Update(_castTime);
            Assert.IsTrue(_controller.IsAbilityOnCd(slot));
            Assert.IsTrue(_controller.GetAbility(slot).CurrentCooldown == 5f);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Doesnt_Go_In_CD_Right_After_Start_Casting(uint slot)
        {
            _controller.StartCast(slot);
            Assert.IsFalse(_controller.IsAbilityOnCd(slot));
            _controller.Update(_castTime - 0.2f);
            Assert.IsFalse(_controller.IsAbilityOnCd(slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_CurrentCD_Doesnt_Change_While_Casting(uint slot)
        {
            _controller.StartCast(slot);
            Assert.IsTrue(_controller.GetAbility(slot).CurrentCooldown == 0);
            _controller.Update(_castTime - 0.2f);
            Assert.IsTrue(_controller.GetAbility(slot).CurrentCooldown == 0);
        }

        [Test]
        [TestCase(2f, 0u)]
        [TestCase(1f, 1u)]
        [TestCase(1.5f, 2u)]
        [TestCase(0f, 1u)]
        [TestCase(5f, 0u)]
        [TestCase(5f, 1u)]
        [TestCase(5f, 2u)]
        public void Cooldown_Is_Updated(float elapsed, uint slot)
        {
            _controller.StartCast(slot);
            _controller.Update(_castTime);
            _controller.Update(elapsed);

            Assert.IsTrue(_controller.GetAbility(slot).CurrentCooldown == _cd - elapsed);
        }

        [Test]
        [TestCase(0u, 0u)]
        [TestCase(0u, 1u)]
        [TestCase(0u, 2u)]
        [TestCase(1u, 2u)]
        [TestCase(2u, 1u)]
        [TestCase(1u, 0u)]
        public void Cant_Cast_While_Casting_Another_Spell(uint slot1, uint slot2)
        {
            _controller.StartCast(slot1);
            _controller.Update(_castTime*0.5f);
            _controller.StartCast(slot2);

            Assert.IsTrue(_controller.GetCastingAbility() == _controller.GetAbility(slot1));
        }

        [Test]
        [TestCase(0u, 0.11f)]
        [TestCase(1u, 0.321f)]
        [TestCase(2u, 0.87f)]
        [TestCase(0u, 0.24f)]
        [TestCase(1u, 0.67f)]
        [TestCase(2u, 0.4f)]
        public void Elapsed_Casting_Time_Is_Updated(uint slot, float castTimePct)
        {
            _controller.StartCast(slot);
            _controller.Update(_castTime*castTimePct);

            Assert.IsTrue(_controller.ElapsedCastingTime == _castTime*castTimePct);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Spell_With_No_Cast_Time_Are_Cast_Instantly(uint slot)
        {
            _controller.GetAbility(slot).CastTime.Returns(0);
            _controller.StartCast(slot);
            Assert.IsTrue(_casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cant_Cast_While_On_Cooldown(uint slot)
        {
            _controller.StartCast(slot);
            _controller.Update(1f);
            _controller.Update(1f);
            _controller.StartCast(slot);

            Assert.IsTrue(_controller.GetCastingAbility() == null && _controller.IsAbilityOnCd(slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Can_Cancel_Casting(uint slot)
        {
            _controller.StartCast(slot);
            _controller.Update(_castTime*0.5f);
            _controller.CancelCast();

            Assert.IsFalse(_controller.IsAbilityOnCd(slot));
            Assert.IsNull(_controller.GetCastingAbility());
        }
        #endregion
    }
}