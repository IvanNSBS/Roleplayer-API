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
			string consolePath = "res://addons/inulib/godot/Gameplay/ConsoleCommands/ConsoleView.cs";
			string iconPath = "res://addons/inulib/godot/Gameplay/ConsoleCommands/Icons/font.png";
			var script = GD.Load<Script>(consolePath);
			var texture = GD.Load<Texture2D>(iconPath);

			AddCustomType("Console Commands", "Control", script, null);
			GD.Print("INUlib plugin was initialized");
		}

		public override void _ExitTree()
		{
			RemoveCustomType("Console Commands");
		}
	}
}
#endif
