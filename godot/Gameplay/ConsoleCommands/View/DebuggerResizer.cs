// using INUlib.Gameplay.Debugging.Console.Data;
// using INUlib.Gameplay.Debugging.Settings;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.EventSystems;

// namespace INUlib.Gameplay.Debugging.Console.View.Debugger
// {
//     public class DebuggerResizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//     {
//         #region Inspector Fields
//         [SerializeField] private RectTransform m_consoleContent;
//         #endregion Inspector Fields
        
//         #region Fields
//         private bool m_isResizing;
//         private Vector2 m_initialClickPosition;
//         private Vector2 m_initialSizeDelta;
//         private DebugSettings m_debugSettings;
//         #endregion Fields


//         #region MonoBehaviour Methods
//         private void Awake()
//         {
//             m_debugSettings = DebugSettings.GetDebugSettings();
//             m_consoleContent.sizeDelta = m_debugSettings.ConsoleSize;
//         }

//         private void Update()
//         {
//             if (m_isResizing)
//                 HandleResize();
//         }
//         #endregion MonoBehaviour Methods
        
        
//         #region Methods
//         private void HandleResize()
//         {
//             Vector2 mousePosition = Mouse.current.position.ReadValue();
//             m_consoleContent.sizeDelta = m_initialSizeDelta + (mousePosition - m_initialClickPosition);
//             m_debugSettings.OverwriteConsoleSize(m_consoleContent.sizeDelta);
//         }
        
//         public void OnPointerDown(PointerEventData eventData)
//         {
//             m_isResizing = true;
//             m_initialClickPosition = eventData.position;
//             m_initialSizeDelta = m_consoleContent.sizeDelta;
//         }

//         public void OnPointerUp(PointerEventData eventData)
//         {
//             m_isResizing = false;
//         }
//         #endregion Methods

//     }
// }