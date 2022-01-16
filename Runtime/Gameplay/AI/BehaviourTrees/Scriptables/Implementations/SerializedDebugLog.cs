using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class SerializedDebugLog : SerializedAction
    {
        [SerializeField] private string _debugMsg;
        public override BTNode CreateNode() => new DebugLogAction(_debugMsg);
    }
}