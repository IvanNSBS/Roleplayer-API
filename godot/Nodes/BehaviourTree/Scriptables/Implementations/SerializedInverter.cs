using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedInverter : SerializedDecorator
    {
        protected override BTNode NodeFactory() => new InverterDecorator();
    }
}