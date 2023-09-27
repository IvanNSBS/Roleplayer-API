#if GODOT
using Godot;

namespace INUlib.Core
{
    public partial class Logger
    {
        partial void Debug(string msg) => GD.Print(msg);
        partial void Warning(string msg) => GD.PushWarning(msg);
        partial void Error(string msg) => GD.PushError(msg);
    }
}
#endif