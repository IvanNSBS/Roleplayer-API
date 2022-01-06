using UnityEngine;
using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;

namespace Tests.Runtime.RPG.Abilities
{
    public class AbilitiesControllerTests
    {
        #region Mock Tests
        private AbilitiesController _controller;
        private bool _casted;
        private float _cd = 5;
        private float _castTime = 1;

        [SetUp]
        public void Setup() {
            _casted = false;
            _controller = new AbilitiesController(2);
            var mock = Substitute.For<IAbility>();
            mock.Cooldown.Returns(_cd);
            mock.CastTime.Returns(_castTime);
            mock.When(x => x.Cast()).Do(x => _casted = true);
            _controller.SetAbility(0, mock);
        }
        #endregion


        #region Methods
        [Test]
        public void Ability_Is_Set_In_Slot()
        {
            var mockAbility = Substitute.For<IAbility>();
            _controller.SetAbility(0, mockAbility);

            Assert.IsTrue(_controller.GetAbility(0) == mockAbility);
        }

        [Test]
        public void Ability_Is_Unleashed_After_Finish_Cast()
        {
            _controller.StartCast(0);
            _controller.Update(1);
            Assert.IsTrue(_casted);
        }

        [Test]
        public void Ability_Goes_On_Cooldown_After_Finish_Cast()
        {
            _controller.StartCast(0);
            _controller.Update(_castTime);
            Assert.IsTrue(_controller.IsAbilityOnCd(0));
            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == 5f);
        }

        public void Ability_Doesnt_Go_In_CD_Right_After_Start_Casting()
        {
            _controller.StartCast(0);
            Assert.IsFalse(_controller.IsAbilityOnCd(0));
            _controller.Update(_castTime - 0.2f);
            Assert.IsFalse(_controller.IsAbilityOnCd(0));
        }

        [Test]
        public void Ability_CurrentCD_Doesnt_Change_While_Casting()
        {
            _controller.StartCast(0);
            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == 0);
            _controller.Update(_castTime - 0.2f);
            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == 0);
        }

        [Test]
        [TestCase(2f)]
        [TestCase(1f)]
        [TestCase(1.5f)]
        [TestCase(0f)]
        [TestCase(5f)]
        public void Cooldown_Is_Updated(float elapsed)
        {
            _controller.StartCast(0);
            _controller.Update(_castTime);
            _controller.Update(elapsed);

            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == _cd - elapsed);
        }

        [Test]
        public void Cant_Cast_While_Casting_Another_Spell()
        {
            var mockB = Substitute.For<IAbility>();
            _controller.SetAbility(1, mockB);

            _controller.StartCast(0);
            _controller.Update(_castTime*0.5f);
            _controller.StartCast(1);

            Assert.IsTrue(_controller.GetCastingAbility() == _controller.GetAbility(0));
        }

        [Test]
        public void Elapsed_Casting_Time_Is_Updated()
        {
            _controller.StartCast(0);
            _controller.Update(_castTime*0.5f);

            Assert.IsTrue(_controller.ElapsedCastingTime == _castTime*0.5f);
        }

        [Test]
        public void Cant_Cast_While_On_Cooldown()
        {
            _controller.StartCast(0);
            _controller.Update(1f);
            _controller.Update(1f);
            _controller.StartCast(0);

            Assert.IsTrue(_controller.GetCastingAbility() == null && _controller.IsAbilityOnCd(0));
        }

        [Test]
        public void Can_Cancel_Casting()
        {
            _controller.StartCast(0);
            _controller.Update(_castTime*0.5f);
            _controller.CancelCast();

            Assert.IsFalse(_controller.IsAbilityOnCd(0));
            Assert.IsNull(_controller.GetCastingAbility());
        }
        #endregion
    }
}