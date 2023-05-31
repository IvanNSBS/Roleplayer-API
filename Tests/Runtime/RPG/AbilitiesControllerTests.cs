using NSubstitute;
using System.Linq;
using NUnit.Framework;
using INUlib.RPG.AbilitiesSystem;

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
            public bool interrupted;
            public bool shouldFinishConcentration;
            public int casted;
            
            public TestFactoryAbility(float cd, float channelingTime, float overchannellingTime, float castTime, float recoveryTime, ICasterInfo factoryRef)
            {
                _factoryRef = factoryRef;
                Cooldown = cd;
                CanCastAbility = true;

                OverchannellingTime = overchannellingTime;
                ChannelingTime = channelingTime;
                RecoveryTime = recoveryTime;
                CastTime = castTime;
                AbilityCastType = AbilityCastType.Concentration;
            }

            public TestFactoryAbility(int cat, float cd, float channelingTime, float overchannellingTime, float castTime, float recoveryTime, ICasterInfo factoryRef)
            {
                _category = cat;
                _factoryRef = factoryRef;
                Cooldown = cd;
                
                CanCastAbility = true;
                
                ChannelingTime = channelingTime;
                OverchannellingTime = overchannellingTime;
                RecoveryTime = recoveryTime;
                CastTime = castTime;
                AbilityCastType = AbilityCastType.Concentration;
            }

            public CastObjects Cast(ICasterInfo dataFactory, uint fromSlot) 
            {
                isEqual = dataFactory == _factoryRef;
                CastHandlerPolicy policy = Substitute.For<CastHandlerPolicy>();
                AbilityObject abilityObject = Substitute.ForPartsOf<AbilityObject>();

                if (removeOnUpdate)
                {
                    abilityObject.When(x => x.OnUpdate(Arg.Any<float>(), Arg.Any<CastingState>())).Do(x =>
                    {
                        float deltaTime = (float)x[0];
                        if (deltaTime > 0.01f)
                        {
                            abilityObject.InvokeNotifyDiscard();
                        }
                    });
                }

                abilityObject.When(x => x.UnleashAbility()).Do(x => casted++);
                
                abilityObject.When(x => x.OnCancelRequested()).Do(x =>
                {
                    interrupted = true;
                });
                
                abilityObject.When(x => x.OnForcedInterrupt()).Do(x =>
                {
                    interrupted = true;
                });
                
                policy.When(x => x.OnCancelRequested(Arg.Any<CastingState>())).Do(x =>
                {
                    interrupted = true;
                });
                
                obj = abilityObject;
                return new CastObjects(policy, abilityObject, CastTimeline, ConcentrationEndCondition);
            }

            public bool CanCast(ICasterInfo caster) => CanCastAbility;

            public bool ConcentrationEndCondition() => shouldFinishConcentration;

            public int Charges => 1;
            public bool CanCastAbility {get; set;}
            public float Cooldown {get; set;}
            public TimelineData CastTimeline => new (ChannelingTime, OverchannellingTime, CastTime, RecoveryTime, 0, AbilityCastType);

            public float CastTime { get; set; }
            
            public float ChannelingTime { get; set; }
            public float OverchannellingTime { get; set; }
            public StartCooldownPolicy StartCooldownPolicy { get; set; }
            
            public float RecoveryTime { get; set;  }
            public AbilityCastType AbilityCastType { get; set; }
            public DiscardPolicy DiscardPolicy { get; set; }
        }
        #endregion


        #region Test Abilities Controller
        public class TestAbilitiesController : AbilitiesController<IAbility<ICasterInfo>, ICasterInfo>
        {
            public int Clicks => _castHandler.TimesCastCalled;

            public TestAbilitiesController(uint slotAmnt, ICasterInfo caster) : base(slotAmnt, caster)
            {
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
        private float _overChannelingTime = 0.2f;
        private float _channelingTime = 1;
        private float _castTime = 0.5f;
        private float _recoveryTime = 1f;
        
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
            ability = new TestFactoryAbility((int)slot, _cd, _channelingTime, _overChannelingTime, _castTime, _recoveryTime, _mockFactory);
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
            var testAbility = new TestFactoryAbility(_cd, _channelingTime, _overChannelingTime, _castTime, _recoveryTime, _mockFactory);
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
        public void Ability_Is_Unleashed_During_Cast(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);

            TestFactoryAbility ability = (TestFactoryAbility)_controller.GetAbility(slot);
            Assert.AreEqual(1, ability.casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Discard_With_After_Discard_Policy(uint slot)
        {
            TestFactoryAbility ability = (TestFactoryAbility)_controller.GetAbility(slot); 
            ability.StartCooldownPolicy = StartCooldownPolicy.AfterDiscard;
            ability.DiscardPolicy = DiscardPolicy.Manual;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            _controller.GetCastHandler().AbilityObject.InvokeNotifyDiscard();
            
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd(slot), "Ability was not on cooldown");
            Assert.AreEqual(_cd, _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Finish_Overchannelling_With_After_Channeling_CD_Policy(uint slot)
        {
            ((TestFactoryAbility)_controller.GetAbility(slot)).StartCooldownPolicy = StartCooldownPolicy.AfterChanneling;
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            Assert.AreEqual(_cd, _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Finish_Channelling_If_Theres_No_Overchanneling_With_After_Channeling_CD_Policy(uint slot)
        {
            ((TestFactoryAbility)_controller.GetAbility(slot)).StartCooldownPolicy = StartCooldownPolicy.AfterChanneling;
            ((TestFactoryAbility)_controller.GetAbility(slot)).OverchannellingTime = 0;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            Assert.AreEqual(_cd, _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Finish_Casting_With_After_Casting_CD_Policy(uint slot)
        {
            ((TestFactoryAbility)_controller.GetAbility(slot)).StartCooldownPolicy = StartCooldownPolicy.AfterCasting;
            ((TestFactoryAbility)_controller.GetAbility(slot)).AbilityCastType = AbilityCastType.FireAndForget;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            Assert.AreEqual(_cd, _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Doesnt_Go_In_CD_Right_After_Start_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            _controller.Update(_channelingTime - 0.2f);
            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd(slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_CurrentCD_Doesnt_Change_While_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            Assert.AreEqual(
                0, _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown, 
                "Cooldown was supposed to be zero right after channeling"
            );
            _controller.Update(_channelingTime - 0.2f);
            Assert.AreEqual(
                0, _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown,
                "Cooldown was supposed to be zero after Update since Channeling Didn't Finish"
            );
        }

        [Test]
        [TestCase(2f, 0u)]
        [TestCase(1f, 1u)]
        [TestCase(1.5f, 2u)]
        [TestCase(0f, 1u)]
        [TestCase(5.2f, 0u)]
        [TestCase(5.54f, 1u)]
        [TestCase(7.345f, 2u)]
        public void Cooldown_Is_Updated(float elapsed, uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(elapsed);

            float currentCooldown = _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown;
            float expected = _cd - elapsed;
            if (expected < 0)
                expected = 0;
            
            Assert.AreEqual(expected, currentCooldown);
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
            _controller.Update(_channelingTime*0.5f);
            _controller.StartChanneling(slot2);

            Assert.IsTrue(_controller.GetCastingAbility() == _controller.GetAbility(slot1));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Spell_With_No_Cast_Time_Are_Cast_Instantly(uint slot)
        {
            TestFactoryAbility ab = new TestFactoryAbility(_cd, 0, 0,0,0, _mockFactory);
            _controller.SetAbility(slot, ab);
            _controller.StartChanneling(slot);
            _controller.Update(0);
            
            Assert.AreEqual(1, ab.casted);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cant_Cast_While_On_Cooldown(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            _controller.StartChanneling(slot);

            Assert.IsNull(_controller.GetCastingAbility(), "Casting ability was not null");
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnCd(slot), "Ability was not on Cooldown");
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cant_Cast_While_On_Secondary_Cooldown(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;

            _controller.CooldownsHandler.PutOnSecondaryCooldown(slot, 10f);
            _controller.StartChanneling(slot);
            
            Assert.IsTrue(_controller.CooldownsHandler.IsAbilityOnSecondaryCd(slot), "Ability was not on secondary Cd");
            Assert.IsNull(_controller.GetCastingAbility(), "Casting should've been null");
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Can_Cancel_Channeling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.5f);
            _controller.CancelCast();
            _controller.Update(_recoveryTime);

            Assert.IsFalse(_controller.CooldownsHandler.IsAbilityOnCd(slot));
            Assert.IsNull(_controller.GetCastingAbility());
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Every_Cast_Process(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.shouldFinishConcentration = true;
            ability.DiscardPolicy = DiscardPolicy.Auto;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(0);
            _controller.Update(_recoveryTime);

            Assert.IsNull(_controller.ActiveAbilities.FirstOrDefault(x => x.AbilityObject == ability.obj));
            Assert.AreEqual(CastingState.None, _controller.CastingState);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Casting(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);

            var ability = (TestFactoryAbility)_controller.GetAbility(slot);

            var found = _controller.ActiveAbilities.First(x => x.AbilityObject == ability.obj);
            Assert.IsTrue(_controller.ActiveAbilities.Contains(found));
            Assert.AreEqual(CastingState.Concentrating, _controller.CastingState);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Concentration(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.shouldFinishConcentration = true;
            _controller.Update(0);
            
            var found = _controller.ActiveAbilities.First(x => x.AbilityObject == ability.obj);
            Assert.IsTrue(_controller.ActiveAbilities.Contains(found));
            Assert.AreEqual(CastingState.CastRecovery, _controller.CastingState);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Cast_If_Is_Fire_And_Forget(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            
            var found = _controller.ActiveAbilities.First(x => x.AbilityObject == ability.obj);
            Assert.IsTrue(_controller.ActiveAbilities.Contains(found));
            Assert.AreEqual(CastingState.CastRecovery, _controller.CastingState);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(0u)]
        [TestCase(0u)]
        public void Cant_Recast_Spell_While_Chaneling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.1f);
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.1f);
        
            Assert.AreEqual(1, _controller.ActiveAbilities.Count);
        }

        [Test]
        [TestCase(0u, 1.2f)]
        [TestCase(1u, 6.3f)]
        [TestCase(2u, 3.54f)]
        public void AbilityObject_Will_Finish_Cast_After_Release_Time_If_Is_Fire_And_Forget(uint slot, float releaseTime)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            ability.RecoveryTime = releaseTime;

            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(ability.RecoveryTime);
            
            Assert.AreEqual(_controller.CastingState, CastingState.None);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Will_Finish_Cast_After_Channeling_If_Theres_No_Release_Time_And_Is_Fire_And_Forget(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            ability.RecoveryTime = 0;
            ability.CastTime = 0;
            ability.OverchannellingTime = 0;
                
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            
            Assert.AreEqual(CastingState.None, _controller.CastingState);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Controller_Keeps_On_Cast_State_If_Finish_Cast_Is_Not_Called(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            
            Assert.AreEqual(_controller.CastingState, CastingState.Casting);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Finishes_Entire_Cast_Process(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            
            _controller.StartChanneling(slot);
            var found = _controller.ActiveAbilities.First(x => x.AbilityObject == ability.obj);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            ability.obj.InvokeNotifyDiscard();
            
            Assert.AreEqual(CastingState.None, _controller.CastingState);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void AbilityObject_Correctly_Notifies_Discard(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            
            _controller.StartChanneling(slot);
            var found = _controller.ActiveAbilities.First(x => x.AbilityObject == ability.obj);
            
            ability.obj.InvokeNotifyDiscard();
            Assert.IsFalse(_controller.ActiveAbilities.Contains(found));
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Object_Is_Automatically_Discarded_With_Auto_Discard_Policy(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            ability.DiscardPolicy = DiscardPolicy.Auto;
            
            _controller.StartChanneling(slot);
            var found = _controller.ActiveAbilities.First(x => x.AbilityObject == ability.obj);
            
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            Assert.IsFalse(_controller.ActiveAbilities.Contains(found));
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
            _controller.Update(_channelingTime*0.1f);

            Assert.IsTrue(_controller.CastingState == CastingState.Channeling);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Overchannelling_After_Finish_Channeling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime*0.01f);

            Assert.IsTrue(_controller.CastingState == CastingState.OverChanneling);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_None_After_Forced_Interrupt(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.1f);
            _controller.ForceInterruptCast();

            Assert.AreEqual(CastingState.None, _controller.CastingState);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Recovery_After_Cancel_While_Overchannelling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime*0.1f);
            _controller.CancelCast();
            
            Assert.AreEqual(CastingState.CastRecovery, _controller.CastingState);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Recovery_After_Cancel_While_Casting(uint slot)
        {
            ((TestFactoryAbility)_controller.GetAbility(slot)).RecoveryTime = _channelingTime;
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime * 0.01f);
            _controller.CancelCast();
            
            Assert.AreEqual(CastingState.CastRecovery, _controller.CastingState);
            Assert.IsTrue(((TestFactoryAbility)_controller.GetAbility(slot)).interrupted);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Recovery_After_Cancel_While_Channeling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.1f);
            _controller.CancelCast();

            Assert.AreEqual(CastingState.CastRecovery, _controller.CastingState);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Recovery_After_Cancel_While_Concentrating(uint slot)
        {
            ((TestFactoryAbility)_controller.GetAbility(slot)).AbilityCastType = AbilityCastType.Concentration;
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.CancelCast();

            Assert.AreEqual(CastingState.CastRecovery, _controller.CastingState);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Casting_After_Overchannelling_Completes(uint slot)
        {
            PrepareMockAbility(_controller.GetAbility(slot), slot, false);
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);

            Assert.IsTrue(_controller.CastingState == CastingState.Casting);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Casting_After_Channelling_If_Theres_No_Overchannelling_Time(uint slot)
        {
            ((TestFactoryAbility)_controller.GetAbility(slot)).OverchannellingTime = 0;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);

            Assert.AreEqual(CastingState.Casting, _controller.CastingState);
        }
        
        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void Can_Only_Cast_If_Requirements_Are_Met(bool requirementsAreMet, bool expected)
        {
            var testAbility = new TestFactoryAbility(_cd, 0, 0, 0, 0, _mockFactory);
            testAbility.CanCastAbility = requirementsAreMet;
            _controller.SetAbility(0, testAbility);
            
            bool result = _controller.StartChanneling(0);

            Assert.IsTrue(expected == result);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_CancelRequest_While_Casting(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);

            Assert.AreEqual(CastingState.Casting, _controller.CastingState);
            _controller.CancelCast();
            
            Assert.IsTrue(ability.interrupted);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_Forced_Interrupt_While_Casting(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);

            Assert.AreEqual(CastingState.Casting, _controller.CastingState);
            _controller.ForceInterruptCast();
            
            Assert.IsTrue(ability.interrupted);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_Interrupt_While_Concentrating(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            bool result = _controller.StartChanneling(slot);

            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);

            Assert.AreEqual(CastingState.Concentrating, _controller.CastingState);
            
            _controller.CancelCast();
            Assert.IsTrue(ability.interrupted);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_Interrupt_While_Channeling(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.1f);
            
            Assert.AreEqual(CastingState.Channeling, _controller.CastingState);
            
            _controller.CancelCast();
            Assert.IsTrue(ability.interrupted);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Receives_Interrupt_While_Overchanneling(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);

            Assert.AreEqual(CastingState.OverChanneling, _controller.CastingState);
            _controller.CancelCast();
            
            Assert.IsTrue(ability.interrupted);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Casting_Is_Completely_Cleared_When_Forced_Interrupt(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.ForceInterruptCast();
            
            Assert.IsNull(_controller.GetCastingAbility(), "Was still casting something after cancel");
            Assert.IsNull(_controller.GetCastHandler(), "Cast Handler was not null after cancel");
            Assert.AreEqual(CastingState.None, _controller.CastingState, "Cast state was not None after cancel");
        }

        [Test]
        public void Ability_That_Wasnt_Discarded_After_Cast_Received_On_Cast_Event()
        {
            TestFactoryAbility ab0 = (TestFactoryAbility)_controller.GetAbility(0);
            TestFactoryAbility ab1 = (TestFactoryAbility)_controller.GetAbility(1);
            ab0.DiscardPolicy = DiscardPolicy.Manual;
            ab1.DiscardPolicy = DiscardPolicy.Manual;
            ab0.shouldFinishConcentration = true;

            _controller.StartChanneling(0);
            var handler = _controller.GetCastHandler();
            
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(0f);
            _controller.Update(_recoveryTime);
            
            Assert.IsTrue(
                _controller.ActiveAbilities.Contains(handler), 
                "Active Abilities does not contain the previous ability CastHandler"
            );
            Assert.AreEqual(CastingState.None, _controller.CastingState);
            Assert.IsNull(_controller.GetCastingAbility(), "No ability should be cast at this moment");
            
            _controller.StartChanneling(0);
            Assert.AreEqual(2, handler.TimesCastCalled);            
        }

        [Test]
        public void Can_Cast_Another_Ability_After_Cast_When_Previous_Ability_Was_Not_Discarded_But_Finished_Cast()
        {
            TestFactoryAbility ab0 = (TestFactoryAbility)_controller.GetAbility(0);
            TestFactoryAbility ab1 = (TestFactoryAbility)_controller.GetAbility(1);
            ab0.DiscardPolicy = DiscardPolicy.Manual;
            ab1.DiscardPolicy = DiscardPolicy.Manual;
            ab0.shouldFinishConcentration = true;

            _controller.StartChanneling(0);
            var handler = _controller.GetCastHandler();
            
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(0);
            _controller.Update(_recoveryTime);
            
            Assert.IsTrue(
                _controller.ActiveAbilities.Contains(handler), 
                "Active Abilities does not contain the previous ability CastHandler"
            );
            Assert.AreEqual(CastingState.None, _controller.CastingState);
            Assert.IsNull(_controller.GetCastingAbility(), "No ability should be cast at this moment");
            
            _controller.StartChanneling(1);
            Assert.AreSame(_controller.GetAbility(1), _controller.GetCastingAbility());
        }
        #endregion
    }
}