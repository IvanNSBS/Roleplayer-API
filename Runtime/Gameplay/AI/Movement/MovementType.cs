using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.Runtime.Serialization;

namespace INUlib.Gameplay.AI.Movement
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MovementType
    {
        [EnumMember(Value = "wander")]
        Wander = 0,
        [EnumMember(Value = "follow")]
        Follow = 1,
        [EnumMember(Value = "flee")]
        Flee = 2,
        [EnumMember(Value = "keep_distance")]
        KeepDistance = 3
    }
}