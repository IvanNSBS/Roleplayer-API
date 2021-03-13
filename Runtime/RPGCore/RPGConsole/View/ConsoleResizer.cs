using System;
using UnityEngine;
using RPGCore.RPGConsole.Data;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace RPGCore.RPGConsole.View
{
    public class ConsoleResizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Inspector Fields
        [SerializeField] private RectTransform m_consoleContent;
        #endregion Inspector Fields
        
        #region Fields
        private bool m_isResizing;
        private Vector2 m_initialClickPosition;
        private Vector2 m_initialSizeDelta;
        private ConsoleSettings m_consoleSettings;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
        }

        private void Update()
        {
            if (m_isResizing)
                HandleResize();
        }

        private void OnDisable()
        {
            m_consoleSettings.OverwriteConsoleSize(m_consoleContent.sizeDelta);
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        private void HandleResize()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            m_consoleContent.sizeDelta = m_initialSizeDelta + (mousePosition - m_initialClickPosition);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            m_isResizing = true;
            m_initialClickPosition = eventData.position;
            m_initialSizeDelta = m_consoleContent.sizeDelta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_isResizing = false;
        }
        #endregion Methods

    }
}