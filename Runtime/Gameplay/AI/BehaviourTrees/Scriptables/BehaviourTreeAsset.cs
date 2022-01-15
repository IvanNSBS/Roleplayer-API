using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    [CreateAssetMenu(menuName="INUlib/AI/Behaviour Tree", fileName="Behaviour Tree")]
    public class BehaviourTreeAsset : ScriptableObject
    {
        #region Inspector Fields
        #endregion

        #region Fields
        [SerializeField] private List<BTNode> _nodes = new List<BTNode>();
        #endregion

        #region Properties
        public List<BTNode> Nodes => _nodes;
        #endregion


        #region Methods
        public TNodeType CreateNode<TNodeType>() where TNodeType : BTNode, new()
        {
            TNodeType node = new TNodeType();
            _nodes.Add(node);

            AssetDatabase.SaveAssets();

            Debug.Log("New Nodes Count: " + _nodes.Count);
            return node;
        }

        public bool RemoveNode(BTNode node)
        {
            bool removed = _nodes.Remove(node);
            if(removed)
                AssetDatabase.SaveAssets();

            return removed;
        }
        #endregion
    }    
}