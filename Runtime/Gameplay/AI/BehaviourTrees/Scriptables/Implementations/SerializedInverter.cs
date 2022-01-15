using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedInverter : SerializedDecorator
    {
        [SerializeField] private int _repeatCount;
        public override BTNode CreateNode() {

            InverterDecorator node = new InverterDecorator();

            if(childs.Count > 0)
                node.SetChild(childs[0].CreateNode());

            return node;
        }
    }
}