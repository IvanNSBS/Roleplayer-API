// using INUlib.Gameplay.Debugging.Console.Data;
// using INUlib.Gameplay.Debugging.Settings;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.EventSystems;

// namespace INUlib.Gameplay.Debugging.Console.View.Debugger
// {
//     public class DebuggerMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//     {
//         #region Inspector Fields
//         [SerializeField] private RectTransform m_moveTarget;
//         #endregion Inspector Fields
        
//         #region Fields
//         private bool m_isMoving;
//         private Vector2 m_initialClickPosition;
//         private Vector2 m_initialPosition;
//         private DebugSettings m_debugSettings;
//         #endregion Fields


//         #region MonoBehaviour Methods
//         private void Awake()
//         {
//             m_debugSettings = DebugSettings.GetDebugSettings();
//             m_moveTarget.anchoredPosition = m_debugSettings.ConsolePosition;
//         }
        
//         private void Update()
//         {
//             if (m_isMoving)
//                 HandleMove();
//         }
//         #endregion MonoBehaviour Methods
        
        
//         #region Methods
//         private void HandleMove()
//         {
//             Vector2 mousePosition = Mouse.current.position.ReadValue();
//             m_moveTarget.anchoredPosition = m_initialPosition + (mousePosition - m_initialClickPosition);
//             m_debugSettings.OverwriteConsolePosition(m_moveTarget.anchoredPosition);
//         }
        
//         public void OnPointerDown(PointerEventData eventData)
//         {
//             m_isMoving = true;
//             m_initialClickPosition = eventData.position;
//             m_initialPosition = m_moveTarget.anchoredPosition;
//         }

//         public void OnPointerUp(PointerEventData eventData)
//         {
//             m_isMoving = false;
//         }
//         #endregion Methods
//     }
// }