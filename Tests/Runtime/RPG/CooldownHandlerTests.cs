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
            public MockCooldownHandler(IAbility<IAbilityCaster>[] abilities) : base(abilities) { }

            public bool ClampCooldown(int slot, float newValue) => this.ClampAbilityCooldown(slot, newValue);
            public float GetCooldownWithCdr(int slot) => this.GetAbilityCooldownWithCdr(slot);
        }

        private int _slotCount = 5;
        private float _abilityMaxCd = 10f;
        private IAbility<IAbilityCaster>[] _abilities;
        private MockCooldownHandler _handler;

        [SetUp]
        public void Setup()
        {
            _abilities = new IAbility<IAbilityCaster>[_slotCount];
            for(int i = 0; i < _slotCount; i++)
            {
                int idx = i;
                IAbility<IAbilityCaster> ability = Substitute.For<IAbility<IAbilityCaster>>();
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

            float value = _handler.GetCooldownWithCdr(category);
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

            float value = _handler.GetCooldownWithCdr(category);
            float clampedCdr = 1 - Mathf.Clamp(testFunc(gcd, ccd), 0, maxCdr);
            float expected = _abilityMaxCd * clampedCdr;

            Assert.AreEqual(expected, value);
        }
        
        [Test]
        [TestCase(0, 1.00f)]
        [TestCase(0, 0.25f)]
        [TestCase(0, 2.23f)]
        [TestCase(0, 6.23f)]
        [TestCase(0, 9.34f)]
        [TestCase(2, 1.00f)]
        [TestCase(2, 0.25f)]
        [TestCase(2, 2.23f)]
        [TestCase(2, 6.23f)]
        [TestCase(2, 9.34f)]
        [TestCase(4, 1.00f)]
        [TestCase(4, 0.25f)]
        [TestCase(4, 2.23f)]
        [TestCase(4, 6.23f)]
        [TestCase(4, 9.34f)]
        public void CooldownHandler_Correctly_Increases_Cooldown(int slotIdx, float inc)
        {
            _handler.IncreaseCooldown(slotIdx, inc);
            CooldownInfo info = _handler.GetCooldownInfo(slotIdx);

            float expected = Mathf.Clamp(inc, 0, _abilityMaxCd * (1 - _handler.GlobalCDR));
            Assert.AreEqual(expected, info.currentCooldown);
        }

        [Test]
        [TestCase(0, 1.00f)]
        [TestCase(0, 0.25f)]
        [TestCase(0, 2.23f)]
        [TestCase(0, 6.23f)]
        [TestCase(0, 9.34f)]
        [TestCase(2, 1.00f)]
        [TestCase(2, 0.25f)]
        [TestCase(2, 2.23f)]
        [TestCase(2, 6.23f)]
        [TestCase(2, 9.34f)]
        [TestCase(4, 1.00f)]
        [TestCase(4, 0.25f)]
        [TestCase(4, 2.23f)]
        [TestCase(4, 6.23f)]
        [TestCase(4, 9.34f)]
        public void CooldownHandler_Correctly_Decreases_Cooldown(int slotIdx, float dec)
        {
            _handler.ResetCooldown(slotIdx);
            _handler.DecreaseCooldown(slotIdx, dec);
            CooldownInfo info = _handler.GetCooldownInfo(slotIdx);

            float maxCd = _abilityMaxCd * (1 - _handler.GlobalCDR);
            float expected = Mathf.Clamp(maxCd - dec, 0, _abilityMaxCd);
            Assert.AreEqual(expected, info.currentCooldown);
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
            for(int i = 0; i < _slotCount; i++)
                _handler.ResetCooldown(i);

            _handler.Update(deltaTime);

            float maxCd = _abilityMaxCd * (1 - _handler.GlobalCDR);
            float expected = Mathf.Clamp(maxCd - deltaTime, 0, _abilityMaxCd);

            for(int i = 0; i < _slotCount; i++)
            {
                CooldownInfo info = _handler.GetCooldownInfo(i);
                Assert.AreEqual(expected, info.currentCooldown);                
            }
        }

        [Test]
        [TestCase(0, 0.9f)]
        [TestCase(0, 0.5f)]
        [TestCase(0, 0.45f)]
        [TestCase(0, 0.3f)]
        [TestCase(0, 0.8f)]
        [TestCase(1, 0.9f)]
        [TestCase(1, 0.5f)]
        [TestCase(1, 0.45f)]
        [TestCase(1, 0.3f)]
        [TestCase(1, 0.8f)]
        [TestCase(2, 0.9f)]
        [TestCase(2, 0.5f)]
        [TestCase(2, 0.45f)]
        [TestCase(2, 0.3f)]
        [TestCase(2, 0.8f)]
        [TestCase(3, 0.9f)]
        [TestCase(3, 0.5f)]
        [TestCase(3, 0.45f)]
        [TestCase(3, 0.3f)]
        [TestCase(3, 0.8f)]
        [TestCase(4, 0.9f)]
        [TestCase(4, 0.5f)]
        [TestCase(4, 0.45f)]
        [TestCase(4, 0.3f)]
        [TestCase(4, 0.8f)]
        public void CooldownHandler_Correctly_Resets_Ability_Cooldown(int slot, float cdr)
        {
            _handler.GlobalCDR = cdr;
            _handler.ResetCooldown(slot);
            CooldownInfo cdInfo = _handler.GetCooldownInfo(slot);

            Assert.AreEqual(cdInfo.totalCooldown, cdInfo.currentCooldown);
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
        public void CooldownHandler_Wont_Do_Anything_When_Reseting_Invalid_Spell()
        {
            IAbility<IAbilityCaster> ab = Substitute.For<IAbility<IAbilityCaster>>();
            bool result = _handler.ResetCooldown(-1);
            bool result2 = _handler.ResetCooldown(ab);

            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Trying_To_Increase_Cd_Of_Invalid_Spell()
        {
            IAbility<IAbilityCaster> ab = Substitute.For<IAbility<IAbilityCaster>>();
            bool result = _handler.IncreaseCooldown(10, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Trying_To_Decrease_Cd_Of_Invalid_Spell()
        {
            IAbility<IAbilityCaster> ab = Substitute.For<IAbility<IAbilityCaster>>();
            bool result = _handler.DecreaseCooldown(10, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public void CooldownHandler_Wont_Do_Anything_When_Clamping_Invalid_Spell_Cooldown()
        {
            IAbility<IAbilityCaster> ab = Substitute.For<IAbility<IAbilityCaster>>();
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
            IAbility<IAbilityCaster> ab = Substitute.For<IAbility<IAbilityCaster>>();
            CooldownInfo info1 = _handler.GetCooldownInfo(-1);
            CooldownInfo info2 = _handler.GetCooldownInfo(ab);

            Assert.IsNull(info1);
            Assert.IsNull(info2);
        }

        [Test]
        [TestCase(0, 0.00f)]
        [TestCase(0, 1.20f)]
        [TestCase(0, 7.50f)]
        [TestCase(0, 3.00f)]
        [TestCase(1, 0.00f)]
        [TestCase(1, 1.20f)]
        [TestCase(1, 7.50f)]
        [TestCase(1, 3.00f)]
        [TestCase(2, 0.00f)]
        [TestCase(2, 1.20f)]
        [TestCase(2, 7.50f)]
        [TestCase(2, 3.00f)]
        [TestCase(3, 0.00f)]
        [TestCase(3, 1.20f)]
        [TestCase(3, 7.50f)]
        [TestCase(3, 3.00f)]
        [TestCase(4, 0.00f)]
        [TestCase(4, 1.20f)]
        [TestCase(4, 7.50f)]
        [TestCase(4, 3.00f)]
        public void CooldownHandler_Properly_Returns_If_Ability_Is_On_Cooldown(int slot, float currentCd)
        {
            _handler.IncreaseCooldown(slot, currentCd);
            bool expected = currentCd > 0;


            Assert.AreEqual(expected, _handler.IsAbilityOnCd(slot));
        }
        #endregion
    }
}