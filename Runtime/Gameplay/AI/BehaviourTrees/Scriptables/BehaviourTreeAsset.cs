using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    [CreateAssetMenu(menuName="INU lib/AI/Behaviour Tree", fileName="Behaviour Tree")]
    public class BehaviourTreeAsset : ScriptableObject
    {
        #region Inspector Fields
        #endregion

        #region Fields
        [SerializeReference] private List<BTNode> _nodes = new List<BTNode>();
        #endregion

        #region Properties
        public List<BTNode> Nodes => _nodes;
        #endregion


        #region Methods
        public BTNode CreateNode(Type t)
        {
            BTNode node = Activator.CreateInstance(t) as BTNode;

            string finalName = t.Name.Replace("Node", "").Replace("Decorator", "").Replace("Action", "")
                                     .Replace("Composite", "");
            
            node.name = finalName;

            _nodes.Add(node);
            AssetDatabase.SaveAssets();

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