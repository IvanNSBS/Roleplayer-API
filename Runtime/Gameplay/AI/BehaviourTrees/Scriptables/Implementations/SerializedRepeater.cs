using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedRepeater : SerializedDecorator
    {
        [SerializeField] private int _repeatCount;
        public override BTNode CreateNode() {

            RepeaterDecorator node = new RepeaterDecorator(_repeatCount);

            if(childs.Count > 0)
                node.SetChild(childs[0].CreateNode());

            return node;
        }
    }
}