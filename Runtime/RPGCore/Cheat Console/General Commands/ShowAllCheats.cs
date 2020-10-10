namespace RPGCore.CheatConsole
{
    public class ShowAllCheats : CheatCommand
    {
        #region Constructors
        public ShowAllCheats(CheatController controller):base(controller, "help", "Shows all available commands", "help")
        {
        }
        #endregion Constructors
        
        
        #region Methods

        public override void Invoke(params string[] args)
        {
            foreach (var commands in m_cheatController.Commands)
            {
                m_cheatController.CheatLogBuffer.Add($"{commands.Value.CommandFormat} - {commands.Value.CommandDescription}");
            }
        }

        #endregion Methods
    }
}