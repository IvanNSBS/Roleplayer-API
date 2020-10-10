using UnityEngine;
using RPGCore.Utils.Extensions;
using System.Collections.Generic;

namespace RPGCore.CheatConsole
{
    public class CheatController : MonoBehaviour
    {
        #region Fields
        private bool m_showConsole = false;
        private string m_inputString = "";
        private Dictionary<string, CheatCommand> m_commands;
        private List<string> m_cheatLogBuffer;
        private Vector2 m_bufferScroll;
        #endregion Fields

        #region Properites
        public Dictionary<string, CheatCommand> Commands => m_commands;

        public List<string> CheatLogBuffer
        {
            get => m_cheatLogBuffer;
            set => m_cheatLogBuffer = value;
        }
        #endregion
        

        #region Methods
        protected void SetShowDebug(bool newValue)
        {
            m_showConsole = newValue;
        }

        protected void HandleInput(bool hideConsoleAfter = false)
        {
            if (m_inputString == "")
            {
                SetShowDebug(hideConsoleAfter);
            }
            m_cheatLogBuffer.Add($">>> {m_inputString} <<<");
            string[] split = m_inputString.Split(' ');
            string[] args = split.SubArray(1);
            if(m_commands.ContainsKey(split[0]))
                m_commands[split[0]].Invoke(args);
            else
                m_cheatLogBuffer.Add($"Command not recognized: {split[0]}");
            m_inputString = "";
            
            SetShowDebug(hideConsoleAfter);
        }
        #endregion
        
        
        #region MonoBehaviour Methods
        protected virtual void Awake()
        {
            m_commands = new Dictionary<string, CheatCommand>();
            m_cheatLogBuffer = new List<string>();
            
            var showallcheats = new ShowAllCheats(this);
            m_commands.Add(showallcheats.CommandId, showallcheats);
            var clearCheatLog = new ClearCheatLog(this);
            m_commands.Add(clearCheatLog.CommandId, clearCheatLog);
        }

        protected virtual void OnGUI()
        {
            if (!m_showConsole)
                return;

            float y = 0f;
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0, 0, Screen.width - 30, 20*m_cheatLogBuffer.Count);
            m_bufferScroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), m_bufferScroll, viewport);
            
            for (int i = 0; i < m_cheatLogBuffer.Count; i++)
            {
                string message = m_cheatLogBuffer[i];
                Rect labelRect = new Rect(5f, 20*i, viewport.width - 100, 20);
                GUI.Label(labelRect, message);
            }
            GUI.EndScrollView();
            y += 100;
            
            GUI.Box(new Rect(0, y, Screen.width, 30), "");
            GUI.backgroundColor = new Color(0,0,0,0);
            m_inputString = GUI.TextField(new Rect(10f, y + 5f, Screen.width-20f, 20f), m_inputString);
        }
        #endregion MonoBehaviour Methods
    }
}