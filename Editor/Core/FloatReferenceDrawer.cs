using UnityEditor;
using INUlib.BackendToolkit.ScriptableValues.DefaultReferences;

namespace INUlib.UEditor.Core
{
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : ScriptableReferenceDrawer { }

    [CustomPropertyDrawer(typeof(Vec3Reference))]
    public class Vec3ReferenceDrawer : ScriptableReferenceDrawer { }
}