using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedWait : SerializedAction
    {
        [SerializeField] private float _waitTime;
        protected override BTNode NodeFactory() => new WaitAction(_waitTime);
    }
}