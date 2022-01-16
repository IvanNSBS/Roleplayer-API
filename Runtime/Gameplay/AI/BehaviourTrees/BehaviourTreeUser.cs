using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Class that uses a BehaviourTree, creating and updating it on each frame
    /// </summary>
    public class BehaviourTreeUser : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] protected BehaviourTreeAsset _btAsset;
        #endregion

        #region Fields
        protected BehaviourTree _bt;
        #endregion

        #region Properties
        public BehaviourTree Tree => _bt;
        #endregion


        #region MonoBehaviour Methods
        protected virtual void Awake()
        {
            BTNode root = _btAsset.Root.CreateNode();
            _bt = new BehaviourTree(root);
            _bt.Start();
        }

        protected virtual void Update()
        {
            _bt?.Update();
        }
        #endregion

    }
}