using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Audio
{
    [CreateAssetMenu(fileName = "Audio Collection", menuName = "Zynith/Audio/Game Tracks", order = 0)]
    public class GameTracks : ScriptableObject
    {
        #region Inspector Fields
        [SerializeField] private AudioTrack m_backgroundMusics;
        [SerializeField] private List<AudioTrack> m_collections;
        #endregion Inspector Fields
        
        #region Properties
        public AudioTrack BackgroundMusics => m_backgroundMusics;
        public IReadOnlyList<AudioTrack> Collections => m_collections;
        #endregion Properties
    }
}