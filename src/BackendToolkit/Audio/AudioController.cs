using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Random = System.Random;

namespace INUlib.BackendToolkit.Audio
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
        private GameObject m_persistentSoundsContainer;
        private AudioSource m_currentBgm;
        #endregion Fields

        #region Constructor
        public AudioController(GameTracks gameTracks)
        {
            InitializeAudioHash(gameTracks);
        }
        #endregion Constructor
        
        
        #region Methods
        public void PlayBackgroundMusic(string id, float fadeTime = 0f, bool persist=true)
        {
            var bgmData = m_bgmHash[id];

            if (fadeTime > 0.01f && m_currentBgm)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append( DOTween.To(() => m_currentBgm.volume, x => m_currentBgm.volume = x, 0, fadeTime) );
                sequence.AppendCallback(() =>
                {
                    MonoBehaviour.Destroy(m_currentBgm.gameObject);
                    m_currentBgm = CreateSound(bgmData, persist);
                    float targetVolume = m_currentBgm.volume;
                    
                    m_currentBgm.spatialize = false;
                    m_currentBgm.spatialBlend = 0f;
                    m_currentBgm.volume = 0;
                    m_currentBgm.Play();
                    sequence.Append( DOTween.To(() => m_currentBgm.volume, x => m_currentBgm.volume = x, targetVolume, fadeTime) );
                });
                return;
            }
            else if(m_currentBgm)
            {
                MonoBehaviour.Destroy(m_currentBgm.gameObject);
                m_currentBgm = null;
            }
            
            m_currentBgm = CreateSound(bgmData, persist);
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
        /// <param name="persist">Whether or not the sound should persist when loading a new scene</param>
        /// <returns>The Instantiated Audio Source</returns>
        public AudioSource PlaySoundAtLocation(string id, Vector3 position, bool persist=false)
        {
            var sound = CreateSound(m_collectionsHash[id], persist);
            sound.transform.position = position;
            sound.Play();

            return sound;
        }
        
        /// <summary>
        /// Chooses a random sound given an array of IDs and plays it at a given location.
        /// The position has no effect if audio data is not set to 3D
        /// </summary>
        /// <param name="ids">List of AudioData IDs</param>
        /// <param name="position">The World Position to spawn the audio</param>
        /// <param name="persist">Whether or not the sound should persist when loading a new scene</param>
        /// <returns>The Instantiated Audio Source</returns>
        public AudioSource PlaySoundAtLocation(string[] ids, Vector3 position, bool persist=false)
        {
            Random rand = new Random();
            int idx = rand.Next(0, ids.Length);
            
            return PlaySoundAtLocation(ids[idx], position, persist);
        }

        /// <summary>
        /// Plays a sound and removes spatialization, even if the sound is set to 3D.
        /// </summary>
        /// <param name="id">The AudioData Id</param>
        /// <param name="persist">Whether or not the sound should persist when loading a new scene</param>
        /// <returns>The Instantiated Audio Source</returns>
        public AudioSource PlaySound(string id, bool persist=false)
        {
            var sound = CreateSound(m_collectionsHash[id], persist);
            sound.spatialize = false;
            sound.Play();

            return sound;
        }
        
        /// <summary>
        /// Chooses a random audio to play it. Removes spatialization, even if sound is
        /// set to 3D.
        /// </summary>
        /// <param name="ids">An Array of AudioData IDs</param>
        /// <param name="persist">Whether or not the sound should persist when loading a new scene</param>
        /// <returns>The Instantiated Audio Source</returns>
        public AudioSource PlaySound(string[] ids, bool persist=false)
        {
            Random rand = new Random();
            int idx = rand.Next(0, ids.Length);
            
            return PlaySound(ids[idx], persist);
        }

        /// <summary>
        /// Destroys an audio over time, fading the audio, or instantly, 
        /// if destroy time is less than or equal to 0.
        /// </summary>
        /// <param name="destroyTime"></param>
        /// <param name="easing"></param>
        public void DestroyAudio(AudioSource source, float destroyTime, Ease easing=Ease.OutQuad)
        {
            if(destroyTime > 0)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(
                    DOTween.To(
                        () => m_currentBgm.volume, x=>m_currentBgm.volume=x, 0, destroyTime
                    ).SetEase(easing)
                );
                sequence.AppendCallback(() => {
                    if(source.gameObject)
                        MonoBehaviour.Destroy(source.gameObject);
                });

                sequence.onComplete += () => {
                    sequence.Kill();
                    sequence = null;
                };

                sequence.Play();
            }
            else
            {
                MonoBehaviour.Destroy(source);
            }
        }

        /// <summary>
        /// Checks if a sound exists in the audio collection
        /// </summary>
        /// <param name="id">The id of the sound</param>
        /// <returns>True if exists. False otherwise</returns>
        public bool SoundsExists(string id) => m_collectionsHash.ContainsKey(id);

        /// <summary>
        /// Checks if a Background Music exists
        /// </summary>
        /// <param name="id">The id of the BGM</param>
        /// <returns></returns>
        public bool BGMExists(string id) => m_bgmHash.ContainsKey(id);
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
        
        protected virtual AudioSource CreateSound(AudioData data, bool persist=false)
        {
            if (!m_soundsContainer)
            {
                m_soundsContainer = new GameObject("Sounds Container");
                m_soundsContainer.transform.position = Vector3.zero;
            }
            if(persist && ! m_persistentSoundsContainer)
            {
                m_persistentSoundsContainer = new GameObject("Sounds Container");
                m_persistentSoundsContainer.transform.position = Vector3.zero;
                MonoBehaviour.DontDestroyOnLoad(m_persistentSoundsContainer);
            }
            
            var soundGO = new GameObject($"Sound_{data.Id}");
            if(persist)
                soundGO.transform.parent = m_persistentSoundsContainer.transform;
            else
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