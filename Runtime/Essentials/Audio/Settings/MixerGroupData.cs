using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Essentials.Audio.Settings
{
    [Serializable]
    public class MixerGroupData 
    {
        #region Fields
        [JsonProperty("parameterName")]
        [SerializeField] private string m_parameterName;
        
        [JsonProperty("mute")]
        [SerializeField] private bool m_mute;
        
        [JsonProperty("volume")]
        [SerializeField, Range(0, 1)] private float m_volume;
        
        [JsonProperty("scaler")]
        [SerializeField, Range(1, 30)] private float m_scaler;
        #endregion Fields

        #region Properties
        public string ParameterName => m_parameterName;
        public float Scaler => m_scaler;

        public float Volume
        {
            get => m_volume;
            set => m_volume = value;
        }

        public bool Mute
        {
            get => m_mute;
            set => m_mute = value;
        }
        #endregion Properties
    }
}