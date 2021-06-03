using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Essentials.Audio.Settings
{
    public class AudioMixerController
    {
        #region Fields
        private AudioChannels m_channels;
        private IReadOnlyDictionary<string, MixerGroupData> m_hash;
        #endregion Fields
        
        #region Constructor
        public AudioMixerController(AudioChannels channels)
        {
            m_channels = channels;
            m_hash = channels.GroupsData.ToDictionary(x => x.ParameterName, x => x);

            foreach (var channel in m_hash)
            {
                SetVolume(channel.Key, channel.Value.Volume);
                SetMute(channel.Key, channel.Value.Mute);
            }
        }
        #endregion Constructor
        
        
        #region Methods
        public MixerGroupData GetGroupData(string groupName)
        {
            return m_hash[groupName];
        }
        
        public bool SetVolume(string groupName, float volume)
        {
            MixerGroupData data = m_hash[groupName];
            data.Volume = volume;
            
            if(!data.Mute)
                return m_channels.Mixer.SetFloat(data.ParameterName, GetScaledVolume(data.Volume, data.Scaler));
            
            return false;
        }

        public bool SetMute(string groupName, bool value)
        {
            MixerGroupData data = m_hash[groupName];
            data.Mute = value;
            
            float muteValue = GetScaledVolume(0, data.Scaler);
            float unmutedValue = GetScaledVolume(data.Volume, data.Scaler);
            
            return m_channels.Mixer.SetFloat(data.ParameterName, value ? muteValue : unmutedValue);
        }
        #endregion Methods
        
        
        #region Helper Methods
        private float GetScaledVolume(float value, float scaler)
        {
            float clampedValue = Mathf.Clamp(value, 0.001f, value);
            return Mathf.Log10(clampedValue) * scaler;
        }
        #endregion Helper Methods
    }
}