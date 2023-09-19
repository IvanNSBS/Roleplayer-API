using System;
using INUlib.BackendToolkit.ScriptableValues.DefaultValues;

namespace INUlib.BackendToolkit.ScriptableValues.DefaultReferences
{
    [Serializable]
    public class IntReference : ScriptableReference<int, IntValue> { }

    [Serializable]
    public class StringReference : ScriptableReference<string, StringValue> { }

    [Serializable]
    public class UIntReference : ScriptableReference<uint, UIntValue> { }

    [Serializable]
    public class BoolReference : ScriptableReference<bool, BoolValue> { }
}