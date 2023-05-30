using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace INUlib.RPG.AbilitiesSystem
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StartCooldownPolicy
    {
        [EnumMember(Value = "after_channeling")]
        AfterChanneling = 0,
    
        [EnumMember(Value = "after_casting")]
        AfterCasting = 1,
        
        [EnumMember(Value = "after_concentrating")]
        AfterConcentrating = 2
    }
}
