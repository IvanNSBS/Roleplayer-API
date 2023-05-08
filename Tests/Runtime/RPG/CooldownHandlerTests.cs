using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;
using UnityEngine;
using System.Linq.Expressions;
using System;

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
            float clampedCdr = 1 - Mathf.Clamp(gcd + ccd, 0, maxCdr);
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
            Func<float, float, float> testFunc = (gcd, ccd) => 0.7f * Mathf.Clamp01(gcd) + 0.3f * Mathf.Clamp01(ccd);
            _handler.SetCdrFunction(testFunc);

            _handler.GlobalCDR = gcd;
            _handler.MaxCdrValue = maxCdr;
            _handler.AddCategoryCdr(category, ccd);

            float value = _handler.GetCooldownWithCdr((uint)category);
            float clampedCdr = 1 - Mathf.Clamp(testFunc(gcd, ccd), 0, maxCdr);
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
        public void CooldownHandler_Correctly_Increases_Cooldown(uint slotIdx, float inc)
        {
            _handler.IncreaseCooldown(slotIdx, inc);
            CooldownInfo info = _handler.GetCooldownInfo(slotIdx);

            float expected = Mathf.Clamp(inc, 0, _abilityMaxCd * (1 - _handler.GlobalCDR));
            Assert.AreEqual(expected, info.currentCooldown);
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
            float expected = Mathf.Clamp(maxCd - dec, 0, _abilityMaxCd);
            Assert.AreEqual(expected, info.currentCooldown);
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

            Assert.AreEqual(info.totalCooldown, info.currentCooldown);
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
            float expected = Mathf.Clamp(maxCd - deltaTime, 0, _abilityMaxCd);

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

            Assert.AreEqual(cdInfo.totalCooldown, cdInfo.currentCooldown);
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
            Assert.AreEqual(Mathf.Clamp01(maxCdr), _handler.MaxCdrValue);
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
            float expected = Mathf.Clamp(cdr, 0, _handler.MaxCdrValue);

            Assert.AreEqual(expected, _handler.GetCategoryCdr(0));
        }

        [Test]
        [TestCase(0.3f)]
        [TestCase(2.0f)]
        public void CooldownHandler_Properly_Sets_Category_Cdr(float cdr)
        {
            _handler.AddCategoryCdr(0, cdr);

            Assert.AreEqual(Mathf.Clamp(cdr, 0, _handler.MaxCdrValue), _handler.GetCategoryCdr(0));
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
        [TestCase(0u, 0.00f)]
        [TestCase(0u, 1.20f)]
        [TestCase(0u, 7.50f)]
        [TestCase(0u, 3.00f)]
        [TestCase(1u, 0.00f)]
        [TestCase(1u, 1.20f)]
        [TestCase(1u, 7.50f)]
        [TestCase(1u, 3.00f)]
        [TestCase(2u, 0.00f)]
        [TestCase(2u, 1.20f)]
        [TestCase(2u, 7.50f)]
        [TestCase(2u, 3.00f)]
        [TestCase(3u, 0.00f)]
        [TestCase(3u, 1.20f)]
        [TestCase(3u, 7.50f)]
        [TestCase(3u, 3.00f)]
        [TestCase(4u, 0.00f)]
        [TestCase(4u, 1.20f)]
        [TestCase(4u, 7.50f)]
        [TestCase(4u, 3.00f)]
        public void CooldownHandler_Properly_Returns_If_Ability_Is_On_Cooldown(uint slot, float currentCd)
        {
            _handler.IncreaseCooldown(slot, currentCd);
            bool expected = currentCd > 0;


            Assert.AreEqual(expected, _handler.IsAbilityOnCd(slot));
        }
        #endregion
    }
}