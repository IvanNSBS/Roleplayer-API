using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedSelector : SerializedComposite
    {
        public override BTNode CreateNode() { 
            SelectorNode node = new SelectorNode();
            foreach(var child in childs)
                node.AddChild(child.CreateNode());

            return node;
        } 
    }
}