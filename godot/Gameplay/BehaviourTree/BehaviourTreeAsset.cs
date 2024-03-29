// using Godot;
// using System.Collections.Generic;

// namespace INUlib.Gameplay.AI.BehaviourTrees
// {
//     public class BehaviourTreeAsset : Resource
//     {
//         #region Serialized Fields
//         [SerializeField] private RootNode _root;
//         [SerializeField] private List<SerializedBTNode> _inspectorNodes = new List<SerializedBTNode>();
//         #endregion

//         #region Properties
//         public RootNode Root => _root;
//         public List<SerializedBTNode> InspectorNodes => _inspectorNodes;
//         #endregion


//         #region Methods
//         public void SetupTree(BehaviourTree tree) => tree.SetRoot(_root.CreateNode());

//         #if UNITY_EDITOR
//         /// <summary>
//         /// Creates the Root Node View for the Behaviour Tree Editor
//         /// </summary>
//         public void CreateRoot()
//         {
//             if(_root == null)
//                 _root = CreateNode(typeof(RootNode), Vector2.zero) as RootNode;
//         }
        

//         /// <summary>
//         /// Creates a node inside te Behaviour Tree Editor Graph View
//         /// </summary>
//         /// <param name="t">The type of the node to create</param>
//         /// <param name="pos">Where in the editor ot spawn it</param>
//         /// <returns>The SerializedBTNode</returns>
//         public SerializedBTNode CreateNode(Type t, Vector2 pos)
//         {
//             SerializedBTNode serialized = CreateBTNode(t, pos);

//             _inspectorNodes.Add(serialized);

//             UnityEditor.AssetDatabase.AddObjectToAsset(serialized, this);
//             UnityEditor.EditorUtility.SetDirty(this);
//             UnityEditor.AssetDatabase.SaveAssets();
 
//             return serialized;
//         }

//         /// <summary>
//         /// Removes a node from the Behaviour Tree Editor Graph View
//         /// </summary>
//         /// <param name="node">The Serialized node to remove</param>
//         /// <returns>True if it was removed. False otherwise</returns>
//         public bool RemoveNode(SerializedBTNode node)
//         {
//             bool removed = _inspectorNodes.Remove(node);
//             if(removed)
//             {
//                 if(node == _root)
//                     _root = null;

//                 UnityEditor.AssetDatabase.RemoveObjectFromAsset(node);
//                 UnityEditor.AssetDatabase.SaveAssets();
//             }

//             return removed;
//         }

//         /// <summary>
//         /// Utility Function to create a SerializedBTNode
//         /// </summary>
//         /// <param name="t">The node type</param>
//         /// <param name="pos">Where to spawn it</param>
//         /// <returns>The created SerializedBTNode</returns>
//         private static SerializedBTNode CreateBTNode(Type t, Vector2 pos)
//         {
//             SerializedBTNode node = ScriptableObject.CreateInstance(t) as SerializedBTNode;
//             string finalName = t.Name.Replace("Node", "").Replace("Decorator", "").Replace("Action", "")
//                             .Replace("Composite", "").Replace("Serialized", "");
            
//             node.name = finalName;
//             node.guid = UnityEditor.GUID.Generate().ToString();
//             node.pos = pos;
        
//             return node;
//         }
//         #endif
//         #endregion
//     }    
// }