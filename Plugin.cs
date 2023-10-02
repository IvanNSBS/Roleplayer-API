#if TOOLS
using Godot;

namespace INUlib
{
	[Tool]
	public partial class INUlibPlugin : EditorPlugin
	{
		public override void _EnterTree()
		{
			// Initialization of the plugin goes here.
			// string consolePath = "res://addons/inulib/godot/Gameplay/ConsoleCommands/ConsoleView.cs";
			// var script = GD.Load<Script>(consolePath);
			// AddCustomType("Console Commands", "CheatConsole", script, null);
		}

		public override void _ExitTree()
		{
			// RemoveCustomType("Console Commands");
		}
	}
}
#endif
