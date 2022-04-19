using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;
using System;
using NUnit.Framework.Internal;
using System.Linq;

namespace Tests.Runtime.RPG.Abilities
{
    public class AbilitiesControllerTests
    {
        #region Test Factory Ability
        public class TestFactoryAbility : IAbility<ICasterInfo>
        {   
            public int Category => _category;

            private int _category = 0;
            private ICasterInfo _factoryRef;
            public bool isEqual;
            public AbilityObject obj;
            public bool removeOnUpdate;

            public TestFactoryAbility(float cd, float castTime, ICasterInfo factoryRef)
            {
                _factoryRef = factoryRef;
                Cooldown = cd;
                ChannelingTime = castTime;
            }

            public TestFactoryAbility(int cat, float cd, float castTime, ICasterInfo factoryRef)
            {
                _category = cat;
                _factoryRef = factoryRef;
                Cooldown = cd;
                ChannelingTime = castTime;
            }

            public CastObjects Cast(ICasterInfo dataFactory) 
            {
                isEqual = dataFactory == _factoryRef;
                CastHandlerPolicy policy = Substitute.For<CastHandlerPolicy>();
                AbilityObject abilityObject = Substitute.ForPartsOf<AbilityObject>();
                if(removeOnUpdate)
                    abilityObject.When(x => x.OnUpdate(Arg.Any<float>())).Do(x => {
                        abilityObject.FinishCast();
                        abilityObject.DiscardAbility();
                    });
                obj = abilityObject;
                return new CastObjects(policy, abilityObject);
            }

            public float Cooldown {get; set;}
            public float ChannelingTime {get;}
        }
        #endregion


        #region Test Abilities Controller
        public class TestAbilitiesController : AbilitiesController<IAbility<ICasterInfo>, ICasterInfo>
        {
            public bool casted = false;
            public int Clicks => _castHandler.TimesCastCalled;
            

            public TestAbilitiesController(uint slotAmnt, ICasterInfo caster) : base(slotAmnt, caster)
            {
            }

            protected override void UnleashAbility()
            {
                base.UnleashAbility();
                casted = true;
            }
        }
        #endregion
 

        #region Mock Tests
        private ICasterInfo _mockFactory;
        private TestAbilitiesController _controller;
        private IAbility<ICasterInfo> _mockAbility1;
        private IAbility<ICasterInfo> _mockAbility2;
        private IAbility<ICasterInfo> _mockAbility3;
        private float _cd = 5;
        private float _castTime = 1;
        
        [SetUp]
        public void Setup() 
        {
            _mockFactory = Substitute.For<ICasterInfo>();
            _controller = new TestAbilitiesController(3, _mockFactory);

            PrepareMockAbility(_mockAbility1, 0);
            PrepareMockAbility(_mockAbility2, 1);
            PrepareMockAbility(_mockAbility3, 2);
        }

        private void PrepareMockAbility(IAbility<ICasterInfo> ability, uint slot, bool reset = true)
        {
            ability = new TestFactoryAbility((int)slot, _cd, _castTime, _mockFactory);
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
            var testAbility = new TestFactoryAbility(_cd, 0, _mockFactory);
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
            var mockAbility = Substitute.For<IAbility<ICasterInfo>>();
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
            Assert.IsTrue(_controller.casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Finish_Cast(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            Assert.IsTrue(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown == 5f);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Doesnt_Go_In_CD_Right_After_Start_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            _controller.Update(_castTime - 0.2f);
            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd(slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_CurrentCD_Doesnt_Change_While_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            Assert.IsTrue(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown == 0);
            _controller.Update(_castTime - 0.2f);
            Assert.IsTrue(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown == 0);
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

            Assert.IsTrue(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown == _cd - elapsed);
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
            TestFactoryAbility ab = new TestFactoryAbility(_cd, 0, _mockFactory);
            _controller.SetAbility(slot, ab);
            _controller.StartChanneling(slot);
            Assert.IsTrue(_controller.casted);
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
            _controller.FinishAbilityCasting();
            _controller.StartChanneling(slot);

            Assert.IsTrue(_controller.GetCastingAbility() == null && _controller.CooldownsHandler.IsAbilityOnCd(slot));
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

            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            Assert.IsNull(_controller.GetCastingAbility());
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.obj.FinishCast();
            
            Assert.IsTrue(_controller.ActiveObjects.Contains(ability.obj));
            Assert.AreEqual(_controller.CastingState, CastingState.None);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Casting_During_His_Update(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.removeOnUpdate = true;
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            
            Assert.IsFalse(_controller.ActiveObjects.Contains(ability.obj));
            Assert.AreEqual(_controller.CastingState, CastingState.None);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Controller_Keeps_On_Cast_State_If_Finish_Cast_Is_Not_Called(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            
            Assert.AreEqual(_controller.CastingState, CastingState.Casting);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_The_Effect(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_castTime);
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.obj.FinishCast();
            ability.obj.DiscardAbility();
            
            Assert.IsFalse(_controller.ActiveObjects.Contains(ability.obj));
            Assert.AreEqual(_controller.CastingState, CastingState.None);
        }

        [Test]
        [TestCase(0u, 5)]
        [TestCase(1u, 3)]
        [TestCase(2u, 7)]
        [TestCase(0u, 1)]
        [TestCase(1u, 3)]
        [TestCase(2u, 6)]
        public void Cast_Policy_Correctly_Calls_On_Cast_Again_After_First_Cast(uint slot, int castTimes)
        {
            for(int i = 0; i < castTimes; i++)
                _controller.StartChanneling(slot);
            
            Assert.AreEqual(castTimes, _controller.Clicks);
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