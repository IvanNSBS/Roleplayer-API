namespace RPGCore.CheatConsole
{
    public class ClearCheatLog : CheatCommand
    {
        #region Constructors
        public ClearCheatLog(CheatController controller):base(controller, "clear", "Clears Cheat Log", "clear")
        {
        }
        #endregion Constructors
        
        
        #region Methods

        public override void Invoke(params string[] args)
        {
            m_cheatController.CheatLogBuffer.Clear();
        }

        #endregion Methods
    }
}