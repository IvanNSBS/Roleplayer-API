using UnityEngine;
using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AI.Movement.Behaviour;
using INUlib.RPG.AI.Movement;

namespace Tests.Runtime.RPG.AI
{
    public class AIFollowPolicyTets
    {
        #region Mock Tests
        private MockSteerBehaviour _behaviour;
        private class MockSteerBehaviour : SteeringBehaviour
        {
            public MockSteerBehaviour(float a, float d, float m) : base(a,d,m){}
            public override Vector3 TargetPos => new Vector3(0.0f, 1.0f, 0.0f);
            public override bool HasTarget => hasTarget;
            public bool hasTarget = true;
        }
        
        [SetUp]
        public void Setup() {
            _behaviour = new MockSteerBehaviour(0.2f, 50f, 10f);
            _behaviour.SetMovementType(MovementType.Follow);
        }
        #endregion


        [Test]
        public void Follow_Policy_Correctly_Calculate_Direction()
        {
            Vector3 selfPos = new Vector3(1.0f, 1.0f, 0.0f);
            _behaviour.CalculateDesiredSpeed(selfPos);
            Assert.IsTrue(_behaviour.DesiredSpeed.normalized == new Vector3(-1.0f, 0.0f, 0.0f));
        }

        [Test]
        public void Follow_Policy_Correctly_Calculate_Speed()
        {
            Vector3 selfPos = new Vector3(1.0f, 1.0f, 0.0f);
            _behaviour.CalculateDesiredSpeed(selfPos);
            Assert.IsTrue(_behaviour.DesiredSpeed.magnitude == 50f);
        }

        [Test]
        public void Follow_Policy_Target_Was_Reached()
        {
            Vector3 selfPos = new Vector3(0.0f, 1.0f, 0.0f);
            _behaviour.CalculateDesiredSpeed(selfPos);
            Assert.IsTrue(_behaviour.HasReachedTarget);
        }

        [Test]
        public void Wont_Change_If_Dont_Have_Target()
        {
            Vector3 selfPos = new Vector3(1.0f, 1.0f, 0.0f);
            _behaviour.hasTarget = false;
            _behaviour.CalculateDesiredSpeed(selfPos);
            Assert.IsTrue(_behaviour.DesiredSpeed == Vector3.zero);
        }
    }
}