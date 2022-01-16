using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedDebugLog : SerializedAction
    {
        [SerializeField] private string _debugMsg;
        protected override BTNode NodeFactory() => new DebugLogAction(_debugMsg);
    }
}