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
        /// <summary>
        /// Fire and Forget means that the user will cast the ability and it will be
        /// unleashed into the world without any other interactions
        /// </summary>
        [EnumMember(Value = "fire_and_forget")]
        FireAndForget = 0,
        
        /// <summary>
        /// Concentration means that the actor needs to keep concentrating on the ability
        /// after it has been cast to mantain it into the world. Losing concentration will
        /// interrupt the spell effect.
        /// </summary>
        [EnumMember(Value = "concentration")]
        Concentration = 1,
    }
}