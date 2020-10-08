namespace RPGCore.CheatConsole
{
    public class ShowAllCheats : CheatCommand
    {
        #region Fields
        private CheatController m_controller;
        #endregion Fields
        
        #region Constructors
        public ShowAllCheats(CheatController controller):base("help", "Shows all available commands", "help")
        {
            m_controller = controller;
        }
        #endregion Constructors
        
        
        #region Methods

        public override void Invoke(params string[] args)
        {
            foreach (var commands in m_controller.Commands)
            {
                m_controller.UsedCommandsBuffer.Add($"{commands.Value.CommandFormat} - {commands.Value.CommandDescription}");
            }
        }

        #endregion Methods
    }
}