using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedSelector : SerializedComposite
    {
        protected override BTNode NodeFactory() => new SelectorNode();
    }
}