using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Essentials.Audio
{
    [System.Serializable]
    public class AudioChannel
    {
        #region Fields
        public float volume;
        public string channelName;
        public AudioChannel parentChannel;

        private Dictionary<Type, AudioChannel> channelHash;
        #endregion Fields


        #region Constructors
        public AudioChannel(string channelName, float defaultVolume = 1, AudioChannel parent = null)
        {
            this.channelName = channelName;
            this.volume = defaultVolume;
            this.parentChannel = parent;
        }
        #endregion Constructors


        #region Methods

        public float GetVolume()
        {
            return GetVolume(this);
        }

        private float GetVolume(AudioChannel channel)
        {
            if (channel == null)
                return 1;
            else
                return volume*GetVolume(parentChannel);
        }
        #endregion Methods
    }
}