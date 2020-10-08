using System;
using UnityEngine;
using System.Collections.Generic;

namespace RPGCore.Audio
{
    [ExecuteInEditMode]
    public class AudioManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private AudioChannel masterChannel;
        [SerializeField] private List<AudioChannel> secondaryChannels;
        #endregion Fields
        
        #region MonoBehaviour Methods

        private void Awake()
        {
            
        }

        #endregion MonoBehaviour Methods
        
        
        #region Utility Methods

        private void InitializeChannels()
        {
            // TODO: Load default volue and name from files
            masterChannel = new AudioChannel("Master Volume", 1f);
            foreach (var channel in secondaryChannels)
            {
                channel.parentChannel = masterChannel;
            }
        }
        #endregion Utility Methods
        
        #region Methods
        public void PlaySoundAtLocation(Vector3 position, string channel)
        {
            
        }

        public void PlaySound()
        {
            
        }
        #endregion Methods
    }
}