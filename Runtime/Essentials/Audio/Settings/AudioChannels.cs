using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Essentials.Audio.Settings
{
    [CreateAssetMenu(fileName = "Audio Settings", menuName = "Zynith/Audio/Settings", order = 0)]
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