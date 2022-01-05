using UnityEngine;
using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AbilitiesSystem;

namespace Tests.Runtime.RPG.Abilities
{
    public class AIFollowPolicyTets
    {
        #region Mock Tests
        AbilitiesController _controller;
        bool _control;

        [SetUp]
        public void Setup() {
            _controller = new AbilitiesController(2);
            var mockAbilityA = Substitute.For<IAbility>();
            mockAbilityA.Cooldown.Returns(5);
            _controller.SetAbility(0, mockAbilityA);
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
        public void Ability_Goes_On_Cooldown_After_Cast()
        {
            _controller.Cast(0);
            Assert.IsTrue(_controller.IsAbilityOnCd(0));
            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == 5f);
        }

        [Test]
        [TestCase(2f)]
        [TestCase(1f)]
        [TestCase(1.5f)]
        [TestCase(0f)]
        [TestCase(5f)]
        public void Cooldown_Is_Updated(float elapsed)
        {
            _controller.Cast(0);
            _controller.Update(elapsed);

            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == 5f - elapsed);
        }

        [Test]
        public void Cant_Cast_While_On_Cooldown()
        {
            _controller.Cast(0);
            _controller.Update(2f);
            _controller.Cast(0);

            Assert.IsTrue(_controller.GetAbility(0).CurrentCooldown == 3f);
        }
        #endregion
    }
}