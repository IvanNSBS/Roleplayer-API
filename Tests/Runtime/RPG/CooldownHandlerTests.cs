using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;
using System;
using INUlib.Utils.Math;

namespace Tests.Runtime.RPG.Abilities
{
    public class CooldownHandlerTests
    {
        #region Setup
        private class MockCooldownHandler : CooldownHandler
        {
            public MockCooldownHandler(IAbility<ICasterInfo>[] abilities) : base(abilities) { }

            public bool ClampCooldown(uint slot, float newValue) => this.ClampAbilityCooldown(slot, newValue);
            public float GetCooldownWithCdr(uint slot) => this.GetAbilityCooldownWithCdr(slot);
        }

        private int _abilityCharges = 1;
        private int _slotCount = 5;
        private float _abilityMaxCd = 10f;
        private IAbility<ICasterInfo>[] _abilities;
        private MockCooldownHandler _handler;

        [SetUp]
        public void Setup()
        {
            _abilities = new IAbility<ICasterInfo>[_slotCount];
            for(int i = 0; i < _slotCount; i++)
            {
                int idx = i;
                IAbility<ICasterInfo> ability = Substitute.For<IAbility<ICasterInfo>>();
                ability.Charges.Returns(_abilityCharges);
                ability.Cooldown.Returns(_abilityMaxCd);
                ability.Category.Returns(idx);

                _abilities[i] = ability;
            }

            _handler = new MockCooldownHandler(_abilities);
        }
        #endregion


        #region Tests
        [Test]
        [TestCase(0, 0.00f, 0.00f, 0.8f)]
        [TestCase(0, 0.20f, 0.10f, 0.8f)]
        [TestCase(1, 0.30f, 0.20f, 0.8f)]
        [TestCase(1, 0.45f, 0.45f, 0.8f)]
        [TestCase(2, 0.60f, 0.60f, 0.8f)]
        [TestCase(2, 0.75f, 0.00f, 0.8f)]
        [TestCase(3, 0.75f, 0.50f, 0.6f)]
        [TestCase(3, 0.90f, 0.20f, 0.7f)]
        [TestCase(4, 0.40f, 0.04f, 0.7f)]
        [TestCase(4, 0.30f, 0.40f, 0.7f)]
        public void Cooldown_Handler_Correctly_Calculates_Cooldown_Reduction(int category, float gcd, float ccd, float maxCdr)
        {
            _handler.GlobalCDR = gcd;
            _handler.MaxCdrValue = maxCdr;
            _handler.AddCategoryCdr(category, ccd);

            float value = _handler.GetCooldownWithCdr((uint)category);
            float clampedCdr = 1 - INUMath.Clamp(gcd + ccd, 0, maxCdr);
            float expected = _abilityMaxCd * clampedCdr;

            Assert.AreEqual(expected, value);
        }

        [Test]
        [TestCase(0, 0.00f, 0.00f, 0.8f)]
        [TestCase(0, 0.20f, 0.10f, 0.8f)]
        [TestCase(1, 0.30f, 0.30f, 0.8f)]
        [TestCase(1, 0.45f, 0.45f, 0.8f)]
        [TestCase(2, 0.60f, 0.60f, 0.8f)]
        [TestCase(2, 0.75f, 0.00f, 0.8f)]
        [TestCase(3, 0.75f, 0.50f, 0.6f)]
        [TestCase(3, 0.90f, 0.20f, 0.7f)]
        [TestCase(4, 0.40f, 0.04f, 0.7f)]
        [TestCase(4, 0.30f, 0.40f, 0.7f)]
        public void CooldownHandler_Correctly_Uses_CDR_Function_To_Calculate_Cdr(int category, float gcd, float ccd, float maxCdr)
        {
            Func<float, float, float> testFunc = (gcd, ccd) => 0.7f * INUMath.Clamp01(gcd) + 0.3f * INUMath.Clamp01(ccd);
            _handler.SetCdrFunction(testFunc);

            _handler.GlobalCDR = gcd;
            _handler.MaxCdrValue = maxCdr;
            _handler.AddCategoryCdr(category, ccd);

            float value = _handler.GetCooldownWithCdr((uint)category);
            float clampedCdr = 1 - INUMath.Clamp(testFunc(gcd, ccd), 0, maxCdr);
            float expected = _abilityMaxCd * clampedCdr;

            Assert.AreEqual(expected, value);
        }
        
        [Test]
        [TestCase(0u, 1.00f)]
        [TestCase(0u, 0.25f)]
        [TestCase(0u, 2.23f)]
        [TestCase(0u, 6.23f)]
        [TestCase(0u, 9.34f)]
        [TestCase(2u, 1.00f)]
        [TestCase(2u, 0.25f)]
        [TestCase(2u, 2.23f)]
        [TestCase(2u, 6.23f)]
        [TestCase(2u, 9.34f)]
        [TestCase(4u, 1.00f)]
        [TestCase(4u, 0.25f)]
        [TestCase(4u, 2.23f)]
        [TestCase(4u, 6.23f)]
        [TestCase(4u, 9.34f)]
        public void CooldownHandler_Correctly_Decreases_Cooldown(uint slotIdx, float dec)
        {
            _handler.PutOnCooldown(slotIdx);
            _handler.DecreaseCooldown(slotIdx, dec);
            CooldownInfo info = _handler.GetCooldownInfo(slotIdx);

            float maxCd = _abilityMaxCd * (1 - _handler.GlobalCDR);
            float expected = INUMath.Clamp(maxCd - dec, 0, _abilityMaxCd);
            Assert.AreEqual(expected, info.currentCooldown);
        }

        [Test]
        [TestCase(0u, 2.874f)]
        [TestCase(1u, 3.442f)]
        [TestCase(2u, 4.32f)]
        [TestCase(3u, 0.32f)]
        [TestCase(4u, 0.44f)]
        public void CooldownHandler_Correctly_Sets_Secondary_Cooldown(uint slotIdx, float cd)
        {
            _handler.PutOnCastPrevention(slotIdx, cd);
            float curr = _handler.GetCooldownInfo(slotIdx).remainingCastPreventionTime;

            Assert.AreEqual(cd, curr);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void CooldownHandler_Correctly_Returns_If_Ability_Is_On_Secondary_Cd(uint slotIdx)
        {
            _handler.PutOnCastPrevention(slotIdx, 10);
            Assert.IsTrue(_handler.IsAbilityOnCastPrevention(slotIdx));
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void CooldownHandler_Does_Not_Go_On_Secondary_CD_When_Using_Values_Close_And_Below_0(uint slotIdx)
        {
            _handler.PutOnCastPrevention(slotIdx, 0);
            Assert.IsFalse(_handler.IsAbilityOnCastPrevention(slotIdx));
        }
        
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void CooldownHandler_Correctly_Decreases_Secondary_Cooldown_On_Update(uint slotIdx)
        {
            _handler.PutOnCastPrevention(slotIdx, 10);
            _handler.Update(1f);

            float curr = _handler.GetCooldownInfo(slotIdx).remainingCastPreventionTime;
            Assert.AreEqual(9f, curr);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void CooldownHandler_Wont_Decrease_Cooldown_Of_Casted_Spell(uint slotIdx)
        {
            _handler.GlobalCDR = 0f;
            _handler.PutOnCooldown(slotIdx);
            _handler.Update(0.5f, _abilities[slotIdx]);
            CooldownInfo info = _handler.GetCooldownInfo(slotIdx);

            Assert.AreEqual(info.maxCooldown, info.currentCooldown);
        }

        [Test]
        [TestCase(0.00f)]
        [TestCase(0.10f)]
        [TestCase(0.20f)]
        [TestCase(0.25f)]
        [TestCase(1.50f)]
        [TestCase(7.75f)]
        public void CooldownHandler_Correctly_Updates_Cooldowns(float deltaTime)
        {
            for(uint i = 0; i < _slotCount; i++)
                _handler.PutOnCooldown(i);

            _handler.Update(deltaTime);

            float maxCd = _abilityMaxCd * (1 - _handler.GlobalCDR);
            float expected = INUMath.Clamp(maxCd - deltaTime, 0, _abilityMaxCd);

            for(uint i = 0; i < _slotCount; i++)
            {
                CooldownInfo info = _handler.GetCooldownInfo(i);
                Assert.AreEqual(expected, info.currentCooldown);                
            }
        }

        [Test]
        [TestCase(0u, 0.9f)]
        [TestCase(0u, 0.5f)]
        [TestCase(0u, 0.45f)]
        [TestCase(0u, 0.3f)]
        [TestCase(0u, 0.8f)]
        [TestCase(1u, 0.9f)]
        [TestCase(1u, 0.5f)]
        [TestCase(1u, 0.45f)]
        [TestCase(1u, 0.3f)]
        [TestCase(1u, 0.8f)]
        [TestCase(2u, 0.9f)]
        [TestCase(2u, 0.5f)]
        [TestCase(2u, 0.45f)]
        [TestCase(2u, 0.3f)]
        [TestCase(2u, 0.8f)]
        [TestCase(3u, 0.9f)]
        [TestCase(3u, 0.5f)]
        [TestCase(3u, 0.45f)]
        [TestCase(3u, 0.3f)]
        [TestCase(3u, 0.8f)]
        [TestCase(4u, 0.9f)]
        [TestCase(4u, 0.5f)]
        [TestCase(4u, 0.45f)]
        [TestCase(4u, 0.3f)]
        [TestCase(4u, 0.8f)]
        public void CooldownHandler_Correctly_Resets_Ability_Cooldown(uint slot, float cdr)
        {
            _handler.GlobalCDR = cdr;
            _handler.PutOnCooldown(slot);
            CooldownInfo cdInfo = _handler.GetCooldownInfo(slot);

            Assert.AreEqual(cdInfo.maxCooldown, cdInfo.currentCooldown);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void CooldownHandler_Correctly_Resets_Cooldown_By_Reference(uint slot)
        {
            var ability = _abilities[slot];

            Assert.IsTrue(_handler.PutOnCooldown(ability));
        }

        [Test]
        public void CooldownHandler_Returns_Null_For_Invalid_Slot_Of_Cooldown_Info()
        {
            CooldownInfo info = _handler.GetCooldownInfo(100);
        
            Assert.IsNull(info);
        }

        [Test]
        public void Cooldown_Handler_Properly_Returns_CooldownInfo_By_Ability_Reference()
        {
            var ability = _abilities[0];
            CooldownInfo info = _handler.GetCooldownInfo(ability);
            
            Assert.NotNull(info);
        }

        [Test]
        public void CooldownHandler_Returns_False_For_Reset_Cooldown_On_Invalid_Slot()
        {
            Assert.IsFalse(_handler.PutOnCooldown(1000));
        }

        [Test]
        public void CooldownHandler_Returns_False_When_Checking_If_Invalid_Slot_Is_On_Cooldown()
        {
            Assert.IsFalse(_handler.IsAbilityOnCd(1000));
        }

        [Test]
        [TestCase(1.0f)]
        [TestCase(2.4f)]
        [TestCase(0.5f)]
        [TestCase(0.32f)]
        [TestCase(0.123f)]
        public void CooldownHandler_Properly_Returns_MaxCdr(float maxCdr)
        {
            _handler.MaxCdrValue = maxCdr;
            Assert.AreEqual(INUMath.Clamp01(maxCdr), _handler.MaxCdrValue);
        }

        [Test]
        public void CooldownHandler_Properly_Updates_Category_Cdr_When_Updating_Max_Cdr()
        {
            _handler.AddCategoryCdr(0, 0.5f);
            _handler.MaxCdrValue = 0.3f;

            Assert.AreEqual(0.3f, _handler.GetCategoryCdr(0));
        }

        [Test]
        [TestCase(0.1f)]
        [TestCase(0.4f)]
        [TestCase(0.22f)]
        [TestCase(0.98f)]
        public void CooldownHandler_Properly_Sets_CategoryCdr(float cdr)
        {
            _handler.SetCategoryCdr(0, cdr);
            float expected = INUMath.Clamp(cdr, 0, _handler.MaxCdrValue);

            Assert.AreEqual(expected, _handler.GetCategoryCdr(0));
        }

        [Test]
        [TestCase(0.3f)]
        [TestCase(2.0f)]
        public void CooldownHandler_Properly_Sets_Category_Cdr(float cdr)
        {
            _handler.AddCategoryCdr(0, cdr);

            Assert.AreEqual(INUMath.Clamp(cdr, 0, _handler.MaxCdrValue), _handler.GetCategoryCdr(0));
        }

        [Test]
        public void CooldownHandler_Properly_Accumulates_Category_Cdr()
        {
            _handler.AddCategoryCdr(0, 0.2f);
            _handler.AddCategoryCdr(0, 0.2f);
        
            Assert.AreEqual(_handler.GetCategoryCdr(0), 0.4f);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Reseting_Invalid_Spell()
        {
            IAbility<ICasterInfo> ab = Substitute.For<IAbility<ICasterInfo>>();
            bool result = _handler.PutOnCooldown(null);
            bool result2 = _handler.PutOnCooldown(ab);

            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Trying_To_Increase_Cd_Of_Invalid_Spell()
        {
            IAbility<ICasterInfo> ab = Substitute.For<IAbility<ICasterInfo>>();
            bool result = _handler.IncreaseCooldown(10, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Trying_To_Decrease_Cd_Of_Invalid_Spell()
        {
            IAbility<ICasterInfo> ab = Substitute.For<IAbility<ICasterInfo>>();
            bool result = _handler.DecreaseCooldown(10, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Clamping_Invalid_Spell_Cooldown()
        {
            IAbility<ICasterInfo> ab = Substitute.For<IAbility<ICasterInfo>>();
            bool result = _handler.ClampCooldown(10, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public void CooldownHandler_Properly_Returns_Cooldown_Info_For_Ability_Reference()
        {
            var abilityRef = _abilities[0];
            Assert.IsNotNull(_handler.GetCooldownInfo(abilityRef));
        }

        [Test]
        public void CooldownHandler_Returns_Null_For_Invalid_Spell_Info()
        {
            IAbility<ICasterInfo> ab = Substitute.For<IAbility<ICasterInfo>>();
            CooldownInfo info1 = _handler.GetCooldownInfo(null);
            CooldownInfo info2 = _handler.GetCooldownInfo(ab);

            Assert.IsNull(info1);
            Assert.IsNull(info2);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void CooldownHandler_Properly_Returns_If_Ability_Is_On_Cooldown(uint slot)
        {
            _handler.PutOnCooldown(slot);
            Assert.IsTrue(_handler.IsAbilityOnCd(slot));
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Ability_Wont_Go_To_Cooldown_If_It_Has_Cooldown_Set_To_Zero(uint slot)
        {
            _abilities[slot].Cooldown.Returns(0);
            Assert.AreEqual(0, _handler.GetCooldownInfo(slot).maxCooldown);

            _handler.PutOnCooldown(slot);
            bool onCooldown = _handler.IsAbilityOnCd(slot);
            Assert.IsFalse(onCooldown, "Expected ability to not be on cooldown but it was");
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Cooldown_Handler_Properly_Adds_Spell_Charges(uint slot)
        {
            _handler.PutOnCooldown(slot);
            _handler.ConsumeCharges(slot, 1);
            var cdInfo = _handler.GetCooldownInfo(slot);
            
            Assert.AreEqual(_abilityCharges-1 , cdInfo.availableCharges);
            
            _handler.Update(_abilityMaxCd);
            cdInfo = _handler.GetCooldownInfo(slot);
            
            Assert.AreEqual(_abilityCharges, cdInfo.availableCharges);
        }

        [Test]
        [TestCase(0u, 1.00f)]
        [TestCase(0u, 0.25f)]
        [TestCase(0u, 2.23f)]
        [TestCase(0u, 6.23f)]
        [TestCase(0u, 9.34f)]
        [TestCase(2u, 1.00f)]
        [TestCase(2u, 0.25f)]
        [TestCase(2u, 2.23f)]
        [TestCase(2u, 6.23f)]
        [TestCase(2u, 9.34f)]
        [TestCase(4u, 1.00f)]
        [TestCase(4u, 0.25f)]
        [TestCase(4u, 2.23f)]
        [TestCase(4u, 6.23f)]
        [TestCase(4u, 9.34f)]
        public void CooldownHandler_Correctly_Increases_Cooldown(uint slotIdx, float inc)
        {
            _handler.PutOnCooldown(slotIdx);
            _handler.Update(inc);
            _handler.IncreaseCooldown(slotIdx, inc);
            CooldownInfo info = _handler.GetCooldownInfo(slotIdx);

            float expected = _abilityMaxCd;
            Assert.AreEqual(expected, info.currentCooldown);
        }
        
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Increase_Cooldown_Wont_Do_Anything_If_Ability_Is_Not_On_Cooldown(uint slot)
        {
            bool output = _handler.IncreaseCooldown(slot, 5);
            Assert.IsFalse(output);
        }

        [Test]
        [TestCase(0u, 2)]
        [TestCase(1u, 1)]
        [TestCase(2u, 5)]
        [TestCase(3u, 4)]
        [TestCase(4u, 3)]
        public void Ability_Correctly_Adds_Available_Charges(uint slot, int add)
        {
            _handler.RemoveAvailableCharges(slot, 100);
            _handler.AddAvailableAbilityCharges(slot, add);

            var cdInfo = _handler.GetCooldownInfo(slot);
            int maxCharges = cdInfo.maxCharges;
            int available = cdInfo.availableCharges;
            int expected = INUMath.Clamp(add, 0, maxCharges);
            
            Assert.AreEqual(expected, available);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Ability_Adds_Only_One_Charge_After_Cooldown_Finishes(uint slot)
        {
            _handler.SetAbilityMaxCharges(slot, 10, false);
            _handler.RemoveAvailableCharges(slot, 100);
            _handler.PutOnCooldown(slot);
            _handler.Update(_abilityMaxCd);

            var cdInfo = _handler.GetCooldownInfo(slot);
            int available = cdInfo.availableCharges;
            
            Assert.AreEqual(1, available);
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Ability_Adds_Charges_On_Loop_When_Updating_Cooldown_If_Charges_Were_Consumed(uint slot)
        {
            _handler.SetAbilityMaxCharges(slot, 3, false);
            _handler.RemoveAvailableCharges(slot, 100);
            _handler.PutOnCooldown(slot);
            
            for(int i = 0; i < 3; i++)
                 _handler.Update(_abilityMaxCd);
            
            var cdInfo = _handler.GetCooldownInfo(slot);
            int available = cdInfo.availableCharges;
            
            Assert.AreEqual(3, available);
        }
        
        [Test]
        [TestCase(0u, 0)]
        [TestCase(1u, 1)]
        [TestCase(2u, 5)]
        [TestCase(3u, 4)]
        [TestCase(4u, 3)]
        [TestCase(0u, 100)]
        [TestCase(1u, 100)]
        [TestCase(2u, 100)]
        [TestCase(3u, 100)]
        [TestCase(4u, 100)]
        public void Ability_Correctly_Removes_Available_Charges(uint slot, int remove)
        {
            _handler.SetAbilityMaxCharges(slot, 100, true);
            _handler.RemoveAvailableCharges(slot, remove);

            var cdInfo = _handler.GetCooldownInfo(slot);
            int maxCharges = cdInfo.maxCharges;
            int available = cdInfo.availableCharges;
            int expected = INUMath.Clamp(100 - remove, 0, maxCharges);
            
            Assert.AreEqual(expected, available);
        }

        [Test]
        [TestCase(0u, 2)]
        [TestCase(1u, 1)]
        [TestCase(2u, 7)]
        [TestCase(3u, 4)]
        [TestCase(4u, 3)]
        public void Ability_Correctly_Adds_Extra_Charges(uint slot, int amount)
        {
            _handler.AddExtraAbilityCharges(slot, amount);
            Assert.AreEqual(amount, _handler.GetCooldownInfo(slot).extraCharges);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Ability_Correctly_Removes_All_Temporary_Charges(uint slot)
        {
            _handler.AddExtraAbilityCharges(slot, 100);
            _handler.RemoveAllAbilityExtraCharges(slot);
            Assert.AreEqual(0, _handler.GetCooldownInfo(slot).extraCharges);
        }
        
        [Test]
        [TestCase(0u, 100, 32)]
        [TestCase(1u, 100, 2)]
        [TestCase(2u, 100, 200)]
        [TestCase(3u, 100, 89)]
        [TestCase(4u, 100, 44)]
        public void Ability_Correctly_Removes_Temporary_Charges(uint slot, int startAmount, int removeAmount)
        {
            _handler.AddExtraAbilityCharges(slot, startAmount);
            _handler.ConsumeExtraCharges(slot, removeAmount);

            int expected = startAmount - removeAmount;
            if (expected < 0)
                expected = 0;
            
            Assert.AreEqual(expected, _handler.GetCooldownInfo(slot).extraCharges);
        }

        [Test]
        [TestCase(0u, 4)]
        [TestCase(1u, 2)]
        [TestCase(2u, 1)]
        [TestCase(3u, 6)]
        [TestCase(4u, 3)]
        public void Ability_Correctly_Sets_Maximum_Amount_Of_Charges(uint slot, int size)
        {
            _handler.SetAbilityMaxCharges(slot, size, false);
            var cdInfo = _handler.GetCooldownInfo(slot);
            
            Assert.AreEqual(size, cdInfo.maxCharges, $"Expected max charges: {size} but got {cdInfo.maxCharges}");
        }
        
        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Ability_Loses_All_Available_Charges_When_Setting_Max_Charges_To_Zero(uint slot)
        {
            _handler.SetAbilityMaxCharges(slot, 0, false);
            var cdInfo = _handler.GetCooldownInfo(slot);
            
            Assert.AreEqual(0, cdInfo.maxCharges, $"Expected max charges: 0 but got {cdInfo.maxCharges}");
            Assert.AreEqual(0, cdInfo.availableCharges, $"Expected available charges: 0 but got {cdInfo.availableCharges}");
        }
        
        [Test]
        [TestCase(0u, 6)]
        [TestCase(1u, 5)]
        [TestCase(2u, 2)]
        [TestCase(3u, 3)]
        [TestCase(4u, 9)]
        public void Ability_Loses_Available_Charges_When_Setting_Max_Charges_To_Zero(uint slot, int removeSize)
        {
            _handler.SetAbilityMaxCharges(slot, 10, true);
            _handler.SetAbilityMaxCharges(slot, 10 - removeSize, false);
            var cdInfo = _handler.GetCooldownInfo(slot);
            int expected = 10 - removeSize < 0 ? 0 : 10 - removeSize;
            
            Assert.AreEqual(expected, cdInfo.maxCharges, $"Expected max charges: {expected} but got {cdInfo.maxCharges}");
            Assert.AreEqual(expected, cdInfo.availableCharges, $"Expected available charges: {expected} but got {cdInfo.availableCharges}");
        }
        
        [Test]
        [TestCase(0u, 4)]
        [TestCase(1u, 2)]
        [TestCase(2u, 3)]
        [TestCase(3u, 6)]
        [TestCase(4u, 3)]
        public void Ability_Correctly_Updates_Charges_When_Setting_Maximum_Amount_Of_Charges_With_Update_Current_Charges(uint slot, int size)
        {
            _handler.SetAbilityMaxCharges(slot, size, true);
            var cdInfo = _handler.GetCooldownInfo(slot);
            Assert.AreEqual(cdInfo.maxCharges, cdInfo.availableCharges);
        }

        [Test]
        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(3u)]
        [TestCase(4u)]
        public void Ability_Correctly_Uses_Extra_Charges_When_They_Are_Avaialble(uint slot)
        {
            _handler.AddExtraAbilityCharges(slot, 1);
            _handler.ConsumeCharges(slot, 1);
            var cdInfo = _handler.GetCooldownInfo(slot);

            Assert.AreEqual(0, cdInfo.extraCharges);
            Assert.AreEqual(1, cdInfo.availableCharges);
        }
        #endregion
    }
}