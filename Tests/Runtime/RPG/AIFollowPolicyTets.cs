using UnityEngine;
using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AI.Movement.Steering.Behaviour;

namespace Tests.Runtime.RPG.AI
{
    public class AIFollowPolicyTets
    {

        [Test]
        public void Follow_Policy_Correctly_Calculate_Direction()
        {
            Vector3 selfPos = new Vector3(1.0f, 1.0f, 0.0f);
            Vector3 targetPos = new Vector3(0.0f, 1.0f, 0.0f);

            FollowPolicy pol = new FollowPolicy(0.2f, targetPos);
            pol.OnUpdate(selfPos);

            Assert.IsTrue(pol.CurrentMoveDirection == new Vector3(-1.0f, 0.0f, 0.0f));
        }

        [Test]
        public void Follow_Policy_Target_Was_Reached()
        {
            Vector3 selfPos = new Vector3(0.05f, 1.0f, 0.0f);
            Vector3 targetPos = new Vector3(0.0f, 1.0f, 0.0f);

            FollowPolicy pol = new FollowPolicy(0.2f, targetPos);
            pol.OnUpdate(selfPos);

            Assert.IsTrue(pol.HasReachedTarget);
        }

        [Test]
        public void Wont_Change_If_Dont_Have_Target()
        {
            Vector3 selfPos = new Vector3(1.0f, 1.0f, 0.0f);
            FollowPolicy pol = new FollowPolicy(0.2f);
            pol.OnUpdate(selfPos);
            pol.SetTarget(null);

            Assert.IsTrue(pol.CurrentMoveDirection == new Vector3(0.0f, 0.0f, 0.0f));
        }

        [Test]
        public void Target_Is_Correctly_Unset()
        {
            Vector3 selfPos = new Vector3(0.05f, 1.0f, 0.0f);
            Vector3 targetPos = new Vector3(0.0f, 1.0f, 0.0f);

            FollowPolicy pol = new FollowPolicy(0.2f, targetPos);
            pol.SetTarget(null);

            Assert.IsTrue(!pol.HasTarget);
        }
    }
}