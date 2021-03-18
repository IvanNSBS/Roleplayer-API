using UnityEngine;
using RPGCore.RPGConsole.Data;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace RPGCore.RPGConsole.View.Debugger
{
    public class DebuggerMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Inspector Fields
        [SerializeField] private RectTransform m_moveTarget;
        #endregion Inspector Fields
        
        #region Fields
        private bool m_isMoving;
        private Vector2 m_initialClickPosition;
        private Vector2 m_initialPosition;
        private ConsoleSettings m_consoleSettings;
        #endregion Fields


        #region MonoBehaviour Methods
        private void Awake()
        {
            m_consoleSettings = ConsoleSettings.GetConsoleSettings();
        }
        
        private void Update()
        {
            if (m_isMoving)
                HandleMove();
        }

        private void OnDisable()
        {
            m_consoleSettings.OverwriteConsolePosition(m_moveTarget.anchoredPosition);
        }
        #endregion MonoBehaviour Methods
        
        
        #region Methods
        private void HandleMove()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            m_moveTarget.anchoredPosition = m_initialPosition + (mousePosition - m_initialClickPosition);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            m_isMoving = true;
            m_initialClickPosition = eventData.position;
            m_initialPosition = m_moveTarget.anchoredPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_isMoving = false;
        }
        #endregion Methods
    }
}