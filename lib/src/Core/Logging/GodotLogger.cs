#if GODOT
using Godot;

namespace INUlib.Core
{
    public partial class Logger
    {
        partial void InfoLog(string msg) => GD.Print(msg);
        partial void DebugLog(string msg) => GD.Print(msg);
        partial void WarningLog(string msg) => GD.PushWarning(msg);
        partial void ErrorLog(string msg) => GD.PushError(msg);
        partial void FatalLog(string msg) =>  GD.PushError(msg);
    }
}
#endif