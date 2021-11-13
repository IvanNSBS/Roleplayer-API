using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Audio
{
    /// <summary>
    /// Game tracks is the container of all the game AudioTracks, aswell as the specific audiotrack for
    /// background musics 
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Collection", menuName = "INU lib/Audio/Game Tracks", order = 0)]
    public class GameTracks : ScriptableObject
    {
        #region Inspector Fields
        [SerializeField] private AudioTrack m_backgroundMusics;
        [SerializeField] private List<AudioTrack> m_collections;
        #endregion Inspector Fields
        
        #region Properties
        public virtual AudioTrack BackgroundMusics => m_backgroundMusics;
        public virtual IReadOnlyList<AudioTrack> Collections => m_collections;
        #endregion Properties
    }
}