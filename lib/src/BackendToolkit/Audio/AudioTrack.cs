using UnityEngine;
using System.Collections.Generic;

namespace INUlib.BackendToolkit.Audio
{
    /// <summary>
    /// AudioTrack is a container of a AudioData. It is supposed to be a container for
    /// audio of a given category
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Collection", menuName = "INU lib/Audio/Audio Collection", order = 0)]
    public class AudioTrack : ScriptableObject
    {
        #region Inspector Fields
        [SerializeField] private List<AudioData> m_audios = new List<AudioData>();
        #endregion Inspector Fields
        
        #region Properties
        public virtual IReadOnlyList<AudioData> Audios => m_audios;
        #endregion Properties
    }
} 