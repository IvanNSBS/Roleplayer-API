using UnityEditor;
using INUlib.BackendToolkit.ScriptableValues.DefaultReferences;

namespace INUlib.UEditor.Core
{
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : ScriptableReferenceDrawer { }
    [CustomPropertyDrawer(typeof(Vec3Reference))]
    public class Vec3ReferenceDrawer : ScriptableReferenceDrawer { }
    [CustomPropertyDrawer(typeof(Vec2Reference))]
    public class Vec2ReferenceDrawer : ScriptableReferenceDrawer { }
    [CustomPropertyDrawer(typeof(IntReference))]
    public class IntReferenceDrawer : ScriptableReferenceDrawer { }
    [CustomPropertyDrawer(typeof(UIntReference))]
    public class UIntReferenceDrawer : ScriptableReferenceDrawer { }
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : ScriptableReferenceDrawer { }
    
    [CustomPropertyDrawer(typeof(BoolReference))]
    public class BoolReferenceDrawer : ScriptableReferenceDrawer { }
}