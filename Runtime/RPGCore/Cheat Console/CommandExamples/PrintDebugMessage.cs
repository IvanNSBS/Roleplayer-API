using UnityEngine;

namespace RPGCore.CheatConsole.CommandExamples
{
    public class PrintDebugMessage : CheatCommand
    {
        #region Constructor
        public PrintDebugMessage() : base("print-debug", "prints a debug message", "print-debug string1 string2")
        {
            
        }
        #endregion Constructor
        
        
        #region CheatCommand Methods
        public override void Invoke(params string[] args)
        {
            Debug.Log($"Parsed commands were: {args[0]} {args[1]}");
        }
        #endregion CheatCommand Methods
    }
}