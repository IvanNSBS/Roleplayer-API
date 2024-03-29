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

            public AbilityBehaviour obj;
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
                AbilityBehaviour abilityBehaviour = Substitute.ForPartsOf<AbilityBehaviour>();

                if (removeOnUpdate)
                {
                    abilityBehaviour.When(x => x.OnUpdate(Arg.Any<float>(), Arg.Any<CastingState>())).Do(x =>
                    {
                        float deltaTime = (float)x[0];
                        if (deltaTime > 0.01f)
                        {
                            abilityBehaviour.InvokeNotifyDiscard();
                        }
                    });
                }

                abilityBehaviour.When(x => x.OnAbilityUnleashed()).Do(x => casted++);
                
                abilityBehaviour.When(x => x.OnCancelRequested(Arg.Any<CastingState>())).Do(x =>
                {
                    interrupted = true;
                });
                
                abilityBehaviour.When(x => x.OnForcedInterrupt()).Do(x =>
                {
                    interrupted = true;
                });
                
                obj = abilityBehaviour;
                return new CastObjects(abilityBehaviour, CastTimeline, ConcentrationEndCondition);
            }

            public bool CanCast(ICasterInfo caster) => CanCastAbility;

            public bool ConcentrationEndCondition() => shouldFinishConcentration;

            public int Charges => 1;
            public bool CanCastAbility {get; set;}
            public float Cooldown {get; set;}
            public TimelineData CastTimeline => new (ChannelingTime, OverchannellingTime, CastTime, RecoveryTime, UnleashTime, AbilityCastType);

            public float CastTime { get; set; }
            public float UnleashTime { get; set; }
            
            public float ChannelingTime { get; set; }
            public float OverchannellingTime { get; set; }
            public StartCooldownPolicy StartCooldownPolicy { get; set; }
            
            public float RecoveryTime { get; set;  }
            public AbilityCastType AbilityCastType { get; set; }
            public DiscardPolicy DiscardPolicy { get; set; }
            public TimelineData[] OnRecastTimelines { get; set; }
        }
        #endregion


        #region Test Abilities Controller
        public class TestAbilitiesController : AbilitiesController<IAbility<ICasterInfo>, ICasterInfo>
        {
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
            Assert.Multiple(() =>
            {
                Assert.That(_controller.AbilitySlots, Is.EqualTo(3));
                Assert.That(_controller.CasterInfo, Is.EqualTo(_mockFactory));
            });
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

            Assert.That(testAbility.isEqual, Is.True);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Is_Set_In_Slot(uint slot)
        {
            var mockAbility = Substitute.For<IAbility<ICasterInfo>>();
            _controller.SetAbility(slot, mockAbility);

            Assert.That(_controller.GetAbility(slot), Is.EqualTo(mockAbility));
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
            Assert.That(ability.casted, Is.EqualTo(1));
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
            
            _controller.GetCastHandler().AbilityBehaviour.InvokeNotifyDiscard();
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True, "Ability was not on cooldown");
                Assert.That(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown, Is.EqualTo(_cd));
            });
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Ability_Goes_On_Cooldown_After_Unleash_With_After_Unleash_Policy(uint slot)
        {
            TestFactoryAbility ability = (TestFactoryAbility)_controller.GetAbility(slot); 
            ability.StartCooldownPolicy = StartCooldownPolicy.AfterUnleash;
            ability.DiscardPolicy = DiscardPolicy.Manual;
            ability.UnleashTime = 0.3f;
            ability.CastTime = 1f;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(ability.UnleashTime);
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True, "Ability was not on cooldown");
                Assert.That(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown, Is.EqualTo(_cd));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True);
                Assert.That(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown, Is.EqualTo(_cd));
            });
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True);
                Assert.That(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown, Is.EqualTo(_cd));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True);
                Assert.That(_controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown, Is.EqualTo(_cd));
            });
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void PutOnCooldown_Wont_Reset_Cooldown_If_Force_Reset_Cooldown_Is_Set_To_False(uint slot)
        {
            _controller.CooldownsHandler.PutOnCooldown(slot);
            _controller.Update(_cd * 0.5f);
            float expected = _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown;
            
            _controller.CooldownsHandler.PutOnCooldown(slot);
            float newCooldown = _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown;
            
            Assert.That(newCooldown, Is.EqualTo(expected));
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void PutOnCooldown_Resets_Cooldown_If_Force_Reset_Cooldown_Is_Set_To_True(uint slot)
        {
            _controller.CooldownsHandler.PutOnCooldown(slot);
            _controller.Update(_cd * 0.5f);
            _controller.CooldownsHandler.PutOnCooldown(slot, forceResetCooldown:true);
            float newCooldown = _controller.CooldownsHandler.GetCooldownInfo(slot).currentCooldown;
            
            Assert.That(newCooldown, Is.EqualTo(_cd));
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
            
            Assert.That(currentCooldown, Is.EqualTo(expected));
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

            Assert.That(_controller.GetCastingAbility(), Is.EqualTo(_controller.GetAbility(slot1)));
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
            
            Assert.That(ab.casted, Is.EqualTo(1));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cant_Cast_Multiple_Charges_Ability_While_On_Cooldown_And_There_Is_Not_Enough_Charges(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            _controller.CooldownsHandler.SetAbilityMaxCharges(slot, 2, true);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            ability.StartCooldownPolicy = StartCooldownPolicy.AfterCasting;
            ability.DiscardPolicy = DiscardPolicy.Auto;
            ability.Cooldown = 30f;

            for (int i = 0; i < 2; i++)
            {
                _controller.StartChanneling(slot);
                _controller.Update(_channelingTime);
                _controller.Update(_overChannelingTime);
                _controller.Update(ability.UnleashTime);
                _controller.Update(ability.CastTime - ability.UnleashTime);
                _controller.Update(_recoveryTime);
            }
            
            _controller.StartChanneling(slot);
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.GetCastingAbility(), Is.Null, "Casting ability was not null");
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True, "Ability was not on Cooldown");
            });
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cant_Cast_Single_Charge_Ability_While_On_Regular_Cooldown(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            _controller.StartChanneling(slot);
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.GetCastingAbility(), Is.Null, "Casting ability was not null");
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True, "Ability was not on Cooldown");
            });
        }

        [Test]
        [TestCase(0u, 2)]
        [TestCase(1u, 1)]
        [TestCase(2u, 3)]
        public void Cant_Cast_Single_And_Multiple_Charge_Ability_While_On_Secondary_Cooldown(uint slot, int maxCharges)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            _controller.CooldownsHandler.SetAbilityMaxCharges(slot, maxCharges, true);

            _controller.CooldownsHandler.PutOnCastPrevention(slot, 10f);
            _controller.StartChanneling(slot);
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCastPrevention(slot), Is.True, "Ability was not on secondary Cd");
                Assert.That(_controller.GetCastingAbility(), Is.Null, "Casting should've been null");
            });
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Can_Cast_Multiple_Charges_Ability_While_On_Regular_Cooldown_If_There_Is_Enough_Charges(uint slot)
        {
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            _controller.CooldownsHandler.SetAbilityMaxCharges(slot,3, true);
            _controller.CooldownsHandler.PutOnCooldown(slot);
            _controller.CooldownsHandler.ConsumeCharges(slot, 1);
            
            _controller.StartChanneling(slot);
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.AbilityHasCharges(slot), Is.True, "Ability didnt have enough charges");
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.True, "Ability was not on cooldown");
                Assert.That(_controller.GetCastingAbility(), Is.Not.Null, "Casting should NOT have been null");
            });
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CooldownsHandler.IsAbilityOnCd(slot), Is.False);
                Assert.That(_controller.GetCastingAbility(), Is.Null);
            });
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.ActiveAbilities.FirstOrDefault(x => x.AbilityBehaviour == ability.obj), Is.Null);
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
            });
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

            var found = _controller.ActiveAbilities.First(x => x.AbilityBehaviour == ability.obj);
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.ActiveAbilities, Does.Contain(found));
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Concentrating));
            });
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
            
            var found = _controller.ActiveAbilities.First(x => x.AbilityBehaviour == ability.obj);
            Assert.Multiple(() =>
            {
                Assert.That(_controller.ActiveAbilities, Does.Contain(found));
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.CastRecovery));
            });
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
            
            var found = _controller.ActiveAbilities.First(x => x.AbilityBehaviour == ability.obj);
            Assert.Multiple(() =>
            {
                Assert.That(_controller.ActiveAbilities, Does.Contain(found));
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.CastRecovery));
            });
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
        
            Assert.That(_controller.ActiveAbilities, Has.Count.EqualTo(1));
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
            
            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
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
            
            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
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
            
            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Casting));
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
            var found = _controller.ActiveAbilities.First(x => x.AbilityBehaviour == ability.obj);
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            ability.obj.InvokeNotifyDiscard();
            
            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
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
            var found = _controller.ActiveAbilities.First(x => x.AbilityBehaviour == ability.obj);
            
            ability.obj.InvokeNotifyDiscard();
            Assert.That(_controller.ActiveAbilities, Does.Not.Contain(found));
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
            var found = _controller.ActiveAbilities.First(x => x.AbilityBehaviour == ability.obj);
            
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            Assert.That(_controller.ActiveAbilities, Does.Not.Contain(found));
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
            var ability = (TestFactoryAbility)_controller.GetAbility(slot);
            ability.AbilityCastType = AbilityCastType.FireAndForget;
            ability.DiscardPolicy = DiscardPolicy.Manual;

            _controller.StartChanneling(slot);
            var handler = _controller.GetCastHandler();
            _controller.Update(_channelingTime);
            _controller.Update(_overChannelingTime);
            _controller.Update(_castTime);
            _controller.Update(_recoveryTime);
            
            for (int i = 0; i < castTimes; i++)
            {
                _controller.StartChanneling(slot);
                _controller.Update(_channelingTime);
                _controller.Update(_overChannelingTime);
                _controller.Update(_castTime);
                _controller.Update(_recoveryTime);
            }
            
            Assert.That(handler.TimesRecastCalled, Is.EqualTo(castTimes));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        public void Cast_State_Goes_To_Channeling_After_Start_Channeling(uint slot)
        {
            _controller.StartChanneling(slot);
            _controller.Update(_channelingTime*0.1f);

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Channeling));
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.OverChanneling));
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
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
            
            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.CastRecovery));
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.CastRecovery));
                Assert.That(((TestFactoryAbility)_controller.GetAbility(slot)).interrupted, Is.True);
            });
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.CastRecovery));
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.CastRecovery));
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Casting));
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Casting));
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

            Assert.That(expected, Is.EqualTo(result));
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Casting));
            _controller.CancelCast();
            
            Assert.That(ability.interrupted, Is.True);
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Casting));
            _controller.ForceInterruptCast();
            
            Assert.That(ability.interrupted, Is.True);
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Concentrating));
            
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
            
            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.Channeling));
            
            _controller.CancelCast();
            Assert.That(ability.interrupted, Is.True);
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

            Assert.That(_controller.CastingState, Is.EqualTo(CastingState.OverChanneling));
            _controller.CancelCast();
            
            Assert.That(ability.interrupted, Is.True);
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.GetCastingAbility(), Is.Null, "Was still casting something after cancel");
                Assert.That(_controller.GetCastHandler(), Is.Null, "Cast Handler was not null after cancel");
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None), "Cast state was not None after cancel");
            });
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.ActiveAbilities, Does.Contain(handler), "Active Abilities does not contain the previous ability CastHandler");
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
                Assert.That(_controller.GetCastingAbility(), Is.Null, "No ability should be cast at this moment");
            });

            _controller.StartChanneling(0);
            Assert.That(handler.TimesRecastCalled, Is.EqualTo(1));
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_controller.ActiveAbilities, Does.Contain(handler), "Active Abilities does not contain the previous ability CastHandler");
                Assert.That(_controller.CastingState, Is.EqualTo(CastingState.None));
                Assert.That(_controller.GetCastingAbility(), Is.Null, "No ability should be cast at this moment");
            });
            _controller.StartChanneling(1);
            Assert.That(_controller.GetCastingAbility(), Is.SameAs(_controller.GetAbility(1)));
        }
        #endregion
    }
}