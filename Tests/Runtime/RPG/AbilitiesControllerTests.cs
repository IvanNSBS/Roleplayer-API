using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;
using System;

namespace Tests.Runtime.RPG.Abilities
{
    public class AbilitiesControllerTests
    {
        #region Test Factory Ability
        public class TestFactoryAbility : IAbility<IAbilityCaster>
        {   
            private IAbilityCaster _factoryRef;
            private Action _actionRef;
            public bool isEqual;

            public TestFactoryAbility(float cd, float castTime, IAbilityCaster factoryRef, Action notifyFinish)
            {
                _actionRef = notifyFinish;
                _factoryRef = factoryRef;
                Cooldown = cd;
                ChannelingTime = castTime;
            }

            public void Cast(IAbilityCaster dataFactory, Action notifyFinishCast) 
                            => isEqual = dataFactory == _factoryRef && _actionRef == notifyFinishCast;
            public void OnChannelingStarted(IAbilityCaster dataFactory) { }
            public void OnChannelingCompleted(IAbilityCaster dataFactory) { }
            public void OnChannelingCanceled(IAbilityCaster dataFactory) { }

            public float CurrentCooldown {get; set;}
            public float Cooldown {get; set;}
            public float ChannelingTime {get;}
        }
        #endregion

        #region Mock Tests
        private IAbilityCaster _mockFactory;
        private AbilitiesController<IAbility<IAbilityCaster>, IAbilityCaster> _controller;
        private IAbility<IAbilityCaster> _mockAbility1;
        private IAbility<IAbilityCaster> _mockAbility2;
        private IAbility<IAbilityCaster> _mockAbility3;
        private bool _casted;
        private float _cd = 5;
        private float _castTime = 1;
        
        [SetUp]
        public void Setup() 
        {
            _casted = false;
            _mockFactory = Substitute.For<IAbilityCaster>();
            _controller = new AbilitiesController<IAbility<IAbilityCaster>, IAbilityCaster>(3, _mockFactory);

            PrepareMockAbility(_mockAbility1, 0);
            PrepareMockAbility(_mockAbility2, 1);
            PrepareMockAbility(_mockAbility3, 2);
        }

        private void PrepareMockAbility(IAbility<IAbilityCaster> ability, uint slot, bool reset = true)
        {
            ability = Substitute.For<IAbility<IAbilityCaster>>();
            ability.Cooldown.Returns(_cd);
            ability.ChannelingTime.Returns(_castTime);
            ability.When(x => x.Cast(_mockFactory, _controller.FinishAbilityCasting)).Do(x => {
                _casted = true;
                if(reset)
                    _controller.FinishAbilityCasting();
            });

            _controller.SetAbility(slot, ability);
        }
        #endregion


        #region Methods
        [Test]
        public void Controller_Is_Constructed_Correctly()
        {
            Assert.IsTrue(_controller.AbilitySlots == 3);
            Assert.IsTrue(_controller.DataHub == _mockFactory);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_Correct_Data_Factory_For_Casting(uint slot)
        {
            var testAbility = new TestFactoryAbility(_cd, 0, _mockFactory, _controller.FinishAbilityCasting);
            _controller.SetAbility(slot, testAbility);
            _controller.StartChanneling(slot);

            Assert.IsTrue(testAbility.isEqual);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Is_Set_In_Slot(uint slot)
        {
            var mockAbility = Substitute.For<IAbility<IAbilityCaster>>();
            _controller.SetAbility(slot, mockAbility);

            Assert.IsTrue(_controller.GetAbility(slot) == mockAbility);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Is_Unleashed_After_Finish_Cast(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(1);
            Assert.IsTrue(_casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Finish_Cast(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd((int)slot));
            Assert.IsTrue(_controller.CooldownsHandler.GetCooldownInfo((int)slot).currentCooldown == 5f);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Doesnt_Go_In_CD_Right_After_Start_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd((int)slot));
            _controller.Update(_castTime - 0.2f);
            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd((int)slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_CurrentCD_Doesnt_Change_While_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
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
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            _controller.Update(elapsed);

            Assert.IsTrue(_controller.CooldownsHandler.GetCooldownInfo((int)slot).currentCooldown == _cd - elapsed);
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
            _controller.StartChanneling(slot1);
            _controller.Update(_castTime*0.5f);
            _controller.StartChanneling(slot2);

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
            _controller.StartChanneling(slot);
            _controller.Update(_castTime*castTimePct);

            Assert.IsTrue(_controller.ElapsedChannelingTime == _castTime*castTimePct);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Spell_With_No_Cast_Time_Are_Cast_Instantly(uint slot)
        {
            _controller.GetAbility(slot).ChannelingTime.Returns(0);
            _controller.StartChanneling(slot);
            Assert.IsTrue(_casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cant_Cast_While_On_Cooldown(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(1f);
            _controller.Update(1f);
            _controller.StartChanneling(slot);

            Assert.IsTrue(_controller.GetCastingAbility() == null && _controller.CooldownsHandler.IsAbilityOnCd((int)slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Can_Cancel_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime*0.5f);
            _controller.CancelChanneling();

            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd((int)slot));
            Assert.IsNull(_controller.GetCastingAbility());
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void On_Channeling_Start_Is_Called(uint slot)
        {
            bool evtCalled = false;
            var ability = _controller.GetAbility(slot);
            ability.When(x => x.OnChannelingStarted(Arg.Any<IAbilityCaster>())).Do(x => evtCalled = true);

            _controller.StartChanneling(slot);

            Assert.IsTrue(evtCalled);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void On_Channeling_Completed_Is_Called(uint slot)
        {
            bool evtCalled = false;
            var ability = _controller.GetAbility(slot);
            ability.When(x => x.OnChannelingCompleted(Arg.Any<IAbilityCaster>())).Do(x => evtCalled = true);

            _controller.StartChanneling(slot);
            _controller.Update(_castTime);

            Assert.IsTrue(evtCalled);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void On_Channeling_Canceled_Is_Called(uint slot)
        {
            bool evtCalled = false;
            var ability = _controller.GetAbility(slot);
            ability.When(x => x.OnChannelingCanceled(Arg.Any<IAbilityCaster>())).Do(x => evtCalled = true);

            _controller.StartChanneling(slot);
            _controller.Update(_castTime*0.1f);
            _controller.CancelChanneling();

            Assert.IsTrue(evtCalled);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Channeling_After_Start_Channeling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime*0.1f);

            Assert.IsTrue(_controller.CastingState == CastingState.Channeling);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_None_After_Cancel_Channeling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime*0.1f);
            _controller.CancelChanneling();

            Assert.IsTrue(_controller.CastingState == CastingState.None);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Casting_After_Channeling_Completes(uint slot)
        {
            PrepareMockAbility(_controller.GetAbility(slot), slot, false);
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);

            Assert.IsTrue(_controller.CastingState == CastingState.Casting);
        }
        #endregion
    }
}