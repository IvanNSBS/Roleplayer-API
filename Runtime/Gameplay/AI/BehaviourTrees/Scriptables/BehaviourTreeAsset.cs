using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    [Serializable]
    public class SerializedNode {
        [HideInInspector] public string name; 
        [HideInInspector] public string guid;
        public Vector2 pos; 
        public BTNode node;

        public SerializedNode(Vector2 p, BTNode n) 
        {
            Type t = n.GetType();

            string finalName = t.Name.Replace("Node", "").Replace("Decorator", "").Replace("Action", "")
                            .Replace("Composite", "");
            
            name = finalName;
            guid = GUID.Generate().ToString();
            pos = p;
            node = n;
            node.guid = guid;
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
        public SerializedNode CreateNode(Type t, Vector2 pos)
        {
            BTNode node = Activator.CreateInstance(t) as BTNode;
            SerializedNode serialized = new SerializedNode(pos, node);

            _inspectorNodes.Add(serialized);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            return serialized;
        }

        public bool RemoveNode(SerializedNode node)
        {
            bool removed = _inspectorNodes.Remove(node);
            if(removed)
            {
               EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }

            return removed;
        }
        #endregion
    }    
}