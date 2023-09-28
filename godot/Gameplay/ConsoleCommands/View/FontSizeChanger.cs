// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using System.Globalization;
// using INUlib.Gameplay.Debugging.Settings;

// namespace INUlib.Gameplay.Debugging.Console.View
// {
//     [RequireComponent(typeof(Button))]
//     public class FontSizeChanger : MonoBehaviour
//     {
//         #region Inspector Fields
//         [SerializeField] private TextMeshProUGUI m_currentSizeText;
//         [SerializeField] private List<TextMeshProUGUI> m_fontsToUpdate;
//         [SerializeField] private float m_sizeChange = 0.5f;
//         [SerializeField] private bool m_decrease = false;
//         #endregion Inspector Fields

//         #region Fields
//         private Button m_button;
//         private DebugSettings m_debugSettings;
//         #endregion Fields


//         #region MonoBehaviour Methods
//         private void Awake()
//         {
//             m_debugSettings = DebugSettings.GetDebugSettings();
//             m_button = GetComponent<Button>();
//             m_button.onClick.AddListener(ChangeSize);
            
//             m_currentSizeText.text = m_debugSettings.FontSize.ToString(CultureInfo.InvariantCulture);
//         }
//         #endregion MonoBehaviour Methods

//         #region Methods
//         private void ChangeSize()
//         {
//             if (m_decrease)
//                 m_debugSettings.FontSize -= m_sizeChange;
//             else
//                 m_debugSettings.FontSize += m_sizeChange;

//             foreach (var font in m_fontsToUpdate)
//                 font.fontSize = m_debugSettings.FontSize;

//             m_currentSizeText.text = m_debugSettings.FontSize.ToString(CultureInfo.InvariantCulture);
//         }
//         #endregion Methods
//     }
// }