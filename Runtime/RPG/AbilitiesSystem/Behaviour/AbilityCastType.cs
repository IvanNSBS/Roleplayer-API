using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace INUlib.RPG.AbilitiesSystem
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AbilityCastType
    {
        [EnumMember(Value = "fire_and_forget")]
        FireAndForget = 0,
        [EnumMember(Value = "concentration")]
        Concentration = 1,
    }
}