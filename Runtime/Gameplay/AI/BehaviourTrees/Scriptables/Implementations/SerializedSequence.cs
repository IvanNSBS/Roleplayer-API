using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedSequence : SerializedComposite
    {
        public override BTNode CreateNode() { 
            SequenceNode node = new SequenceNode();
            foreach(var child in childs)
                node.AddChild(child.CreateNode());

            return node;
        } 
    }
}