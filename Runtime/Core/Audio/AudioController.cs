using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Random = System.Random;

namespace INUlib.Core.Audio
{
    /// <summary>
    /// AudioController is a class responsible to instantiate and manage all audio for a game made with the RPG API.
    /// </summary>
    public class AudioController
    {
        #region Fields
        private IReadOnlyDictionary<string, AudioData> m_bgmHash;
        private IReadOnlyDictionary<string, AudioData> m_collectionsHash;
        private GameObject m_soundsContainer;
        private AudioSource m_currentBgm;
        #endregion Fields

        #region Constructor
        public AudioController(GameTracks gameTracks)
        {
            InitializeAudioHash(gameTracks);
        }
        #endregion Constructor
        
        
        #region Methods
        public void PlayBackgroundMusic(string id, float fadeTime = 0f)
        {
            var bgmData = m_bgmHash[id];

            if (fadeTime > 0.01f && m_currentBgm)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append( DOTween.To(() => m_currentBgm.volume, x => m_currentBgm.volume = x, 0, fadeTime) );
                sequence.AppendCallback(() =>
                {
                    MonoBehaviour.Destroy(m_currentBgm.gameObject);
                    m_currentBgm = CreateSound(bgmData);
                    float targetVolume = m_currentBgm.volume;
                    
                    m_currentBgm.spatialize = false;
                    m_currentBgm.spatialBlend = 0f;
                    m_currentBgm.volume = 0;
                    m_currentBgm.Play();
                    sequence.Append( DOTween.To(() => m_currentBgm.volume, x => m_currentBgm.volume = x, targetVolume, fadeTime) );
                });
                return;
            }
            
            m_currentBgm = CreateSound(bgmData);
            m_currentBgm.spatialize = false;
            m_currentBgm.spatialBlend = 0f;
            m_currentBgm.Play();
        }
        
        /// <summary>
        /// Creates and plays a sound at a given location. The position has no effect
        /// if audio data is not set to 3D
        /// </summary>
        /// <param name="id">The audio ID</param>
        /// <param name="position">The Wold Position to spawn the audio</param>
        public void PlaySoundAtLocation(string id, Vector3 position)
        {
            var sound = CreateSound(m_collectionsHash[id]);
            sound.transform.position = position;
            sound.Play();
        }
        
        /// <summary>
        /// Chooses a random sound given an array of IDs and plays it at a given location.
        /// The position has no effect if audio data is not set to 3D
        /// </summary>
        /// <param name="ids">List of AudioData IDs</param>
        /// <param name="position">The World Position to spawn the audio</param>
        public void PlaySoundAtLocation(string[] ids, Vector3 position)
        {
            Random rand = new Random();
            int idx = rand.Next(0, ids.Length);
            
            PlaySoundAtLocation(ids[idx], position);
        }

        /// <summary>
        /// Plays a sound and removes spatialization, even if the sound is set to 3D.
        /// </summary>
        /// <param name="id">The AudioData Id</param>
        public void PlaySound(string id)
        {
            var sound = CreateSound(m_collectionsHash[id]);
            sound.spatialize = false;
            sound.Play();
        }
        
        /// <summary>
        /// Chooses a random audio to play it. Removes spatialization, even if sound is
        /// set to 3D.
        /// </summary>
        /// <param name="ids">An Array of AudioData IDs</param>
        public void PlaySound(string[] ids)
        {
            Random rand = new Random();
            int idx = rand.Next(0, ids.Length);
            
            PlaySound(ids[idx]);
        }
        #endregion Methods
        
        
        #region Helper Methods
        private void InitializeAudioHash(GameTracks gameTracks)
        {
            m_bgmHash = gameTracks.BackgroundMusics.Audios.ToDictionary(x => x.Id, x => x);
            
            List<Dictionary<string, AudioData>> results = new List<Dictionary<string, AudioData>>();
            
            foreach (var collection in gameTracks.Collections)
                results.Add( collection.Audios.ToDictionary(x => x.Id, x => x) );

            m_collectionsHash = results.SelectMany(x => x)
                .ToDictionary(x => x.Key, y => y.Value);
        }
        
        protected virtual AudioSource CreateSound(AudioData data)
        {
            if (!m_soundsContainer)
            {
                m_soundsContainer = new GameObject("Sounds Container");
                m_soundsContainer.transform.position = Vector3.zero;
            }
            
            var soundGO = new GameObject($"Sound_{data.Id}");
            soundGO.transform.parent = m_soundsContainer.transform;
            var source = soundGO.AddComponent<AudioSource>();
            source.playOnAwake = false;
            
            source.volume = data.Volume;
            source.outputAudioMixerGroup = data.MixerGroup;
            source.clip = data.GetRandomClip();
            source.pitch = Mathf.Clamp(data.Pitch + RandomRange(data.RandomPitch), -3f, 3f);
            source.loop = data.Loop;
            
            if (data.Is3d)
            {
                source.spatialize = true;
                source.spatialBlend = 1;

                source.minDistance = data.MinDistance;
                source.maxDistance = data.MaxDistance;
            }

            if (!data.Loop)
                MonoBehaviour.Destroy(soundGO, source.clip.length);
            
            return source;
        }

        private float RandomRange(Vector2 range)
        {
            float min = range.x;
            float max = range.y;
            return (float)new System.Random().NextDouble() * (max - min) + min;
        }
        #endregion Helper Methods
    }
}