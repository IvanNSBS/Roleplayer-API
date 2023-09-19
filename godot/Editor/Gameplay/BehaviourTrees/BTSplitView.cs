using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace INUlib.UEditor.Gameplay.BehaviourTrees
{
    // Two Pane Split View is not Experimental, but for some reason it won't show up
    // in the default things to add, so we need to do the same as we did with the BT Graph View
    public class BTSplitView : TwoPaneSplitView
    {
        #region Uxml Factory
        public new class UxmlFactory : UxmlFactory<BTSplitView, TwoPaneSplitView.UxmlTraits> { }
        #endregion
    }

}