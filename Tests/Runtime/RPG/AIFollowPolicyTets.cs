using UnityEngine;
using NUnit.Framework;
using NSubstitute;
using INUlib.RPG.AI.Movement.Steering.Behaviour;

namespace Tests.Runtime.RPG.AI
{
    public class AIFollowPolicyTets
    {

        [Test]
        public void FollowPolicyCorrectlyCalculateDirection()
        {
            Vector3 selfPos = new Vector3(1.0f, 1.0f, 0.0f);
            Vector3 targetPos = new Vector3(0.0f, 1.0f, 0.0f);

            FollowPolicy pol = new FollowPolicy(targetPos, 0.2f);
            pol.OnUpdate(selfPos);

            Assert.IsTrue(pol.CurrentMoveDirection == new Vector3(-1.0f, 0.0f, 0.0f));
        }

        [Test]
        public void FollowPolicyTargetWasReached()
        {
            Vector3 selfPos = new Vector3(0.05f, 1.0f, 0.0f);
            Vector3 targetPos = new Vector3(0.0f, 1.0f, 0.0f);

            FollowPolicy pol = new FollowPolicy(targetPos, 0.2f);
            pol.OnUpdate(selfPos);

            Assert.IsTrue(pol.HasReachedTarget);
        }
    }
}