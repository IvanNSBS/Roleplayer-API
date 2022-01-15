using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    [Serializable]
    public class SerializedNode {
        [HideInInspector] public string name; 
        [HideInInspector] public Vector2 pos; 
        public BTNode node;

        public SerializedNode(Vector2 p, BTNode n) {
            Type t = n.GetType();

            string finalName = t.Name.Replace("Node", "").Replace("Decorator", "").Replace("Action", "")
                            .Replace("Composite", "");
            
            name = finalName;
            pos = p;
            node = n;
        }
    }

    [CreateAssetMenu(menuName="INU lib/AI/Behaviour Tree", fileName="Behaviour Tree")]
    public class BehaviourTreeAsset : ScriptableObject
    {
        #region Serialized Fields
        [SerializeReference] private List<SerializedNode> _inspectorNodes = new List<SerializedNode>();
        #endregion

        #region Properties
        public List<SerializedNode> InspectorNodes => _inspectorNodes;
        #endregion


        #region Methods
        public SerializedNode CreateNode(Type t)
        {
            BTNode node = Activator.CreateInstance(t) as BTNode;
            SerializedNode serialized = new SerializedNode(Vector2.zero, node);

            _inspectorNodes.Add(serialized);

            AssetDatabase.SaveAssets();

            return serialized;
        }

        public bool RemoveNode(SerializedNode node)
        {
            bool removed = _inspectorNodes.Remove(node);
            if(removed)
                AssetDatabase.SaveAssets();

            return removed;
        }
        #endregion
    }    
}