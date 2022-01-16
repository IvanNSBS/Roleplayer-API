using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    [CreateAssetMenu(menuName="INU lib/AI/Behaviour Tree", fileName="Behaviour Tree")]
    public class BehaviourTreeAsset : ScriptableObject
    {
        #region Serialized Fields
        [SerializeField] private RootNode _root;
        [SerializeField] private List<SerializedBTNode> _inspectorNodes = new List<SerializedBTNode>();
        #endregion

        #region Properties
        public List<SerializedBTNode> InspectorNodes => _inspectorNodes;
        #endregion


        #region Methods
        public BehaviourTree CreateTree() => new BehaviourTree(_root.CreateNode());

        public void CreateRoot()
        {
            if(_root == null)
                _root = CreateNode(typeof(RootNode), Vector2.zero) as RootNode;
        }
        
        public SerializedBTNode CreateNode(Type t, Vector2 pos)
        {
            SerializedBTNode serialized = CreateBTNode(t, pos);

            _inspectorNodes.Add(serialized);

            AssetDatabase.AddObjectToAsset(serialized, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
 
            return serialized;
        }

        public bool RemoveNode(SerializedBTNode node)
        {
            bool removed = _inspectorNodes.Remove(node);
            if(removed)
            {
                if(node == _root)
                    _root = null;

                AssetDatabase.RemoveObjectFromAsset(node);
                AssetDatabase.SaveAssets();
            }

            return removed;
        }

        private static SerializedBTNode CreateBTNode(Type t, Vector2 pos)
        {
            SerializedBTNode node = ScriptableObject.CreateInstance(t) as SerializedBTNode;
            string finalName = t.Name.Replace("Node", "").Replace("Decorator", "").Replace("Action", "")
                            .Replace("Composite", "").Replace("Serialized", "");
            
            node.name = finalName;
            node.guid = GUID.Generate().ToString();
            node.pos = pos;
        
            return node;
        }
        #endregion
    }    
}