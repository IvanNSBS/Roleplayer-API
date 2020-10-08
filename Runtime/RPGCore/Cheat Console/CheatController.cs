using System;
using System.Linq;
using UnityEngine;
using RPGCore.Utils.Extensions;
using System.Collections.Generic;
using System.Xml.Schema;
using RPGCore.CheatConsole.CommandExamples;

namespace RPGCore.CheatConsole
{
    public class CheatController : MonoBehaviour
    {
        #region Fields
        private bool m_showConsole = false;
        private string m_inputString = "";
        private Dictionary<string, CheatCommand> m_commands;
        private List<string> m_usedCommandsBuffer;
        private Vector2 m_bufferScroll;
        #endregion Fields

        #region Properites
        public Dictionary<string, CheatCommand> Commands => m_commands;

        public List<string> UsedCommandsBuffer
        {
            get => m_usedCommandsBuffer;
            set => m_usedCommandsBuffer = value;
        }
        #endregion
        

        #region Methods
        private void ToggleDebug()
        {
            m_showConsole = !m_showConsole;
        }

        private void HandleInput()
        {
            if (m_showConsole)
            {
                m_usedCommandsBuffer.Add(m_inputString);
                string[] split = m_inputString.Split(' ');
                string[] args = split.SubArray(1);
                foreach (var splitted in args)
                {
                    Debug.Log($"Split: {splitted}");    
                }
                m_commands[split[0]].Invoke(args);
                m_inputString = "";
            }
            ToggleDebug();
        }
        #endregion
        
        
        #region MonoBehaviour Methods
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleInput();
            }
        }

        private void Awake()
        {
            m_commands = new Dictionary<string, CheatCommand>();
            m_usedCommandsBuffer = new List<string>();
            
            // IEnumerable<CheatCommand> exporters = typeof(CheatCommand)
            //     .Assembly.GetTypes()
            //     .Where(t => t.IsSubclassOf(typeof(CheatCommand)) && !t.IsAbstract)
            //     .Select(t => (CheatCommand)Activator.CreateInstance(t));
            //
            // foreach (var cheat in exporters)
            // {
            //     Debug.Log($"Cheats found: {cheat.CommandId}");
            //     m_commands.Add(cheat.CommandId, cheat);                
            // }
            
            var printDebug = new PrintDebugMessage();
            m_commands.Add(printDebug.CommandId, printDebug);
            var showallcheats = new ShowAllCheats(this);
            m_commands.Add(showallcheats.CommandId, showallcheats);
        }

        private void OnGUI()
        {
            if (!m_showConsole)
                return;

            float y = 0f;
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0, 0, Screen.width - 30, 20*m_usedCommandsBuffer.Count);
            m_bufferScroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), m_bufferScroll, viewport);
            
            for (int i = 0; i < m_usedCommandsBuffer.Count; i++)
            {
                string message = m_usedCommandsBuffer[i];
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