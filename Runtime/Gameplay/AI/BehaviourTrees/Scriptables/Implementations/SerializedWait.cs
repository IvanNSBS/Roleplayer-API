using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedWait : SerializedAction
    {
        [SerializeField] private float _waitTime;
        public override BTNode CreateNode() => new WaitAction(_waitTime);
    }
}