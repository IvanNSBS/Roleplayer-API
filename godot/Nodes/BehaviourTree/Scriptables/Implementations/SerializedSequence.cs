using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedSequence : SerializedComposite
    {
        protected override BTNode NodeFactory() => new SequenceNode(); 
    }
}