using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedRepeater : SerializedDecorator
    {
        [SerializeField] private int _repeatCount;
        protected override BTNode NodeFactory() => new RepeaterDecorator(_repeatCount);
    }
}