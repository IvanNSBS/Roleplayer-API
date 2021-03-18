using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RPGCore.RPGConsole.View
{
    public class ViewGroup : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private ContentSizeFitter m_logTextFitter;
        [SerializeField] private ContentSizeFitter m_logFitter;
        [SerializeField] private ScrollRect m_scrollRect;
        #endregion Inspector Fields
        
        #region Fields
        private List<ViewTab> m_tabs;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            m_tabs = GetComponentsInChildren<ViewTab>().ToList();
            // if(m_tabs.Count() > 0)
            //     m_tabs[0]
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        public void ScrollToBottom()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_logTextFitter.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_logFitter.GetComponent<RectTransform>());
            m_logTextFitter.enabled = false;
            m_logFitter.enabled = false;
            m_logTextFitter.enabled = true;
            m_logFitter.enabled = true;
            m_scrollRect.normalizedPosition = Vector2.zero;
        }
        #endregion Methods
    }
}