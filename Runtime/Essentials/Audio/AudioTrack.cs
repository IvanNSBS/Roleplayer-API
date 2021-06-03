using UnityEngine;
using System.Collections.Generic;

namespace Essentials.Audio
{
    [CreateAssetMenu(fileName = "Audio Collection", menuName = "Zynith/Audio/Audio Collection", order = 0)]
    public class AudioTrack : ScriptableObject
    {
        #region Inspector Fields
        [SerializeField] private List<AudioData> m_audios;
        #endregion Inspector Fields
        
        #region Properties
        public List<AudioData> Audios => m_audios;
        #endregion Properties
    }
} 