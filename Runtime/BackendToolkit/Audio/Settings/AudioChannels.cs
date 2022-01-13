using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace INUlib.BackendToolkit.Audio.Settings
{
    /// <summary>
    /// Unity AudioMixer configuration file for the RPG API
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Settings", menuName = "INU lib/Audio/Settings", order = 0)]
    public class AudioChannels : ScriptableObject
    {
        #region Inspector Fields
        [SerializeField] private AudioMixer m_mixer;
        [SerializeField] private List<MixerGroupData> m_groupsData;
        #endregion Inspector Fields
        
        #region Properties
        public AudioMixer Mixer => m_mixer;
        public IReadOnlyList<MixerGroupData> GroupsData => m_groupsData;
        #endregion Properties
    }
}