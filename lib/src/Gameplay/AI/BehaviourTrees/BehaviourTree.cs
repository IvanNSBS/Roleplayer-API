using System;
using System.Collections.Generic;
using System.Linq;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Class that holds the implementation of a Behaviour Tree
    /// and is responsible for executing it
    /// </summary>
    public class BehaviourTree
    {
        #region Fields
        protected bool _started;
        protected BTNode _root;
        protected NodeState _treeState;
        protected Blackboard _blackboard;
        #endregion


        #region Properties
        /// <summary>
        /// The current Tree Proccessing State
        /// </summary>
        public NodeState TreeState => _treeState;

        /// <summary>
        /// The Behaviour Tree Blackboard
        /// </summary>
        public Blackboard Blackboard => _blackboard;
        #endregion


        #region Constructor
        public BehaviourTree() => _blackboard = new Blackboard();
        public BehaviourTree(BTNode root) { 
            _blackboard = new Blackboard();
            _root = root;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Sets the root of the tree, attaching the behaviour to 
        /// </summary>
        /// <param name="node">The new root node</param>
        public void SetRoot(BTNode node) => _root = node;
        
        /// <summary>
        /// Gets the Root Node for the Behaviour Tree
        /// </summary>
        /// <returns></returns>
        public BTNode GetRoot() => _root;

        /// <summary>
        /// Ticks the tree, updating it's state.
        /// Nothing will happen and it will always return Failure 
        /// if the tree hasn't been started
        /// </summary>
        /// <returns>The new tree state</returns>
        public NodeState Update(float deltaTime) {
            if(!_started) {
                _treeState = NodeState.Failure;
                return NodeState.Failure;
            }

            _treeState = _root.Update(deltaTime);
            return _treeState;
        }

        /// <summary>
        /// Starts the Behaviour Tree, sharing the blackboard among all of the
        /// behaviour tree nodes an allowing the Update to happen
        /// </summary>
        public void Start()
        {
            _started = true;

            PerformDFS(node => node.SetBlackBoard(_blackboard));
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Performs a Depth First Search on the Behaviour Tree.
        /// Optionally, it can execute a function that has the visited node as parameter once this node
        /// has been visited by the DFS algorithm
        /// </summary>
        /// <param name="OnNodeVisited">The function too execute on the node once it has been visited</param>
        public void PerformDFS(Action<BTNode> OnNodeVisited)
        {
            Stack<BTNode> stack = new Stack<BTNode>();
            HashSet<BTNode> visitedSet = new HashSet<BTNode>();
            
            stack.Push(_root);

            while(stack.Count > 0)
            {
                BTNode node = stack.Pop();
                bool visited = visitedSet.Contains(node);

                if(!visited)
                {
                    visitedSet.Add(node);
                    OnNodeVisited?.Invoke(node);

                    IReadOnlyList<BTNode> childs = node.GetChildren();
                    
                    if(childs != null)
                        // Reversing is necessary so we visit each node from left to right
                        foreach(var child in childs.Reverse())
                            stack.Push(child);
                }
            }
        }
        #endregion
    }
}