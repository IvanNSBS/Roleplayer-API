using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Essentials.Audio
{
    /// <summary>
    /// AudioData contains all the data necessary to play an AudioClip in unity, as well as data for
    /// the specific RPG API functionalities
    /// </summary>
    [Serializable]
    public class AudioData
    {
        #region Fields
        [SerializeField] private string m_id;
        
        [Header("Sound Data")]
        [SerializeField] private bool m_loop;
        [SerializeField] [Range(0, 1)] private float m_volume = 1f;
        [SerializeField] private AudioMixerGroup m_mixerGroup;
        [SerializeField] private AudioClip m_clips;
        
        [Space(15)]
        
        [Header("Pitch Data")]
        [SerializeField] [Range(-3, 3)] private float m_pitch = 1;
        [SerializeField] [MinMax(-1, 1)] private Vector2 m_randomPitch;

        [Space(15)]
        [Header("3D Data")] 
        [SerializeField] private bool m_is3d;
        [SerializeField] [Range(0.1f, 30f)] private float m_minDistance;
        [SerializeField] [Range(0.1f, 30f)] private float m_maxDistance;
        #endregion Fields

        
        #region Properties
        public string Id => m_id;
        public AudioClip Clips => m_clips;
        public AudioMixerGroup MixerGroup => m_mixerGroup;
        public float Volume => m_volume;
        public float Pitch => m_pitch;
        public Vector2 RandomPitch => m_randomPitch;
        public bool Is3d => m_is3d;
        public float MinDistance => m_minDistance;
        public float MaxDistance => m_maxDistance;
        public bool Loop => m_loop;
        #endregion Properties
        
        
        #region Methods
        public AudioClip GetRandomClip()
        {
            return m_clips;
        }
        #endregion Methods
    }
}