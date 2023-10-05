using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace INUlib.RPG.AbilitiesSystem
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CooldownUpdateType
    {
        /// <summary>
        /// Cooldown will be updated every frame with the Update function
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto = 0,
        
        /// <summary>
        /// Cooldowns will never be updated and the user must manually call the
        /// UpdateCooldowns function of the AbilitiesController 
        /// </summary>
        [EnumMember(Value = "manual")]
        Manual = 1
    }
}