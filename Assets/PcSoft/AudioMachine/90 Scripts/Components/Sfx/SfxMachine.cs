using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.AudioMachine._90_Scripts.Assets;
using PcSoft.AudioMachine._90_Scripts.Assets.Sfx;
using PcSoft.AudioMachine._90_Scripts.Assets.Sfx.Base;
using PcSoft.AudioMachine._90_Scripts.Utils;
using PcSoft.AudioMachine._90_Scripts.Utils.Extensions;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace PcSoft.AudioMachine._90_Scripts.Components.Sfx
{
    [AddComponentMenu(AudioMachineConstants.Menus.Components.SfxMenu + "/Sfx Machine")]
    [RequireComponent(typeof(AudioSource))]
    public sealed class SfxMachine : MonoBehaviour
    {
        #region Static Area

        public static SfxMachine Singleton => Resources.FindObjectsOfTypeAll<SfxMachine>()[0];

        #endregion

        #region Inspector Data

        [SerializeField]
        private AudioMixerGroup audioMixerGroup;

        #endregion

        private CollectionAudioHelper<DirectSfxSourceAsset> _extendedAudioHelper;
        private CollectionAudioHelper<AudioClip> _simpleAudioHelper;
        private AudioSource _audioSource;
        private readonly IDictionary<SfxSourceAsset, AudioSourceHolder> _audioSourceMap = new Dictionary<SfxSourceAsset, AudioSourceHolder>();

        #region Builtin Methods

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        private void LateUpdate()
        {
            foreach (var holder in _audioSourceMap)
            {
                if (holder.Value.AudioSources.Any(x => !x.Value.isPlaying))
                {
                    if (_extendedAudioHelper != null)
                    {
                        var randomSource = _extendedAudioHelper.Next();

                        foreach (var audioSource in holder.Value.AudioSources.Values)
                        {
                            Destroy(audioSource);
                        }
                        holder.Value.AudioSources.Clear();
                        
                        var dict = BuildAudioSources(randomSource, false);
                        holder.Value.AudioSources.AddRange(dict);
                    }
                    else if (_simpleAudioHelper != null)
                    {
                        var randomClip = _simpleAudioHelper.Next();
                    
                        foreach (var audioSource in holder.Value.AudioSources.Values)
                        {
                            Destroy(audioSource);
                        }
                        holder.Value.AudioSources.Clear();
                    
                        var newAudioSource = CreateAudioSource(false);
                        newAudioSource.clip = randomClip;
                        newAudioSource.Play();
                    
                        holder.Value.AudioSources.Add("Default", newAudioSource);
                    }
                }                
            }
        }

        #endregion

        public void Play(SfxSourceAsset sfxSource)
        {
            if (sfxSource == null)
                throw new ArgumentException("sfxSource == null");
            
            if (sfxSource is SimpleSfxSourceAsset simpleSfxSource)
            {
                _audioSource.PlayOneShot(simpleSfxSource.Clip);
            }
            else if (sfxSource is MultiTrackSfxSourceAsset multiTrackAfxSource)
            {
                foreach (var track in multiTrackAfxSource.Tracks)
                {
                    if (track.StartDelay <= 0f)
                    {
                        _audioSource.PlayOneShot(track.Clip);
                    }
                    else
                    {
                        StartCoroutine(AnimationUtils.WaitAndRun(track.StartDelay, () => _audioSource.PlayOneShot(track.Clip)));
                    }
                }
            }
            else if (sfxSource is ExtendedCollectionSfxSourceAsset extendedCollectionSfxSource)
            {
                var audioHelper = new CollectionAudioHelper<DirectSfxSourceAsset>(CollectionAudioBehavior.PlayRandom, extendedCollectionSfxSource.SfxSources);
                var randomSource = audioHelper.Next();
                
                Play(randomSource);
            }
            else if (sfxSource is SimpleCollectionSfxSourceAsset simpleCollectionSfxSource)
            {
                var audioHelper = new CollectionAudioHelper<AudioClip>(CollectionAudioBehavior.PlayRandom, simpleCollectionSfxSource.Clips);
                var randomClip = audioHelper.Next();
                
                _audioSource.PlayOneShot(randomClip);
            }
        }

        public void PlayLoop(SfxSourceAsset sfxSource)
        {
            if (sfxSource == null)
                throw new ArgumentException("sfxSource == null");
            
            if (_audioSourceMap.ContainsKey(sfxSource))
            {
                var holder = _audioSourceMap[sfxSource];
                holder.Add();

                return;
            }

            _extendedAudioHelper = null;
            _simpleAudioHelper = null;
            
            var audioSources = BuildAudioSources(sfxSource);
            _audioSourceMap.Add(sfxSource, new AudioSourceHolder(audioSources));
        }

        public void Stop()
        {
            _audioSource.Stop();
        }
        
        public void StopAllLoops()
        {
            foreach (var clip in new List<SfxSourceAsset>(_audioSourceMap.Keys))
            {
                StopLoop(clip);
            }
        }

        public void StopLoop(SfxSourceAsset sfxSource)
        {
            if (!_audioSourceMap.ContainsKey(sfxSource))
            {
                Debug.LogWarning("[SFX] Clip not found in loop map");
                return;
            }

            var holder = _audioSourceMap[sfxSource];
            if (holder.Remove())
            {
                foreach (var audioSource in holder.AudioSources.Values)
                {
                    Destroy(audioSource);
                }
                _audioSourceMap.Remove(sfxSource);
            }
        }

        private Dictionary<string, AudioSource> BuildAudioSources(SfxSourceAsset sfxSource, bool loop = true)
        {
            var audioSources = new Dictionary<string, AudioSource>();
            if (sfxSource is SimpleSfxSourceAsset simpleSfxSource)
            {
                var newAudioSource = CreateAudioSource(loop);
                newAudioSource.clip = simpleSfxSource.Clip;
                newAudioSource.Play();

                audioSources.Add("default", newAudioSource);
            }
            else if (sfxSource is MultiTrackSfxSourceAsset multiTrackSfxSource)
            {
                foreach (var track in multiTrackSfxSource.Tracks)
                {
                    var newAudioSource = CreateAudioSource(loop);
                    newAudioSource.clip = track.Clip;
                    newAudioSource.PlayDelayed(track.StartDelay);

                    audioSources.Add(track.Identifier, newAudioSource);
                }
            }
            else if (sfxSource is ExtendedCollectionSfxSourceAsset extendedCollectionSfxSource)
            {
                _extendedAudioHelper = new CollectionAudioHelper<DirectSfxSourceAsset>(extendedCollectionSfxSource.PlayBehavior, extendedCollectionSfxSource.SfxSources);
                var randomSource = _extendedAudioHelper.Next();

                return BuildAudioSources(randomSource, false);
            }
            else if (sfxSource is SimpleCollectionSfxSourceAsset simpleCollectionSfxSource)
            {
                _simpleAudioHelper = new CollectionAudioHelper<AudioClip>(simpleCollectionSfxSource.PlayBehavior, simpleCollectionSfxSource.Clips);
                var randomClip = _simpleAudioHelper.Next();
                
                var newAudioSource = CreateAudioSource(false);
                newAudioSource.clip = randomClip;
                newAudioSource.Play();
                
                audioSources.Add("default", newAudioSource);
            }
            else
                throw new NotImplementedException();

            return audioSources;
        }

        private AudioSource CreateAudioSource(bool loop)
        {
            var newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            newAudioSource.loop = loop;
            newAudioSource.outputAudioMixerGroup = audioMixerGroup;
            
            return newAudioSource;
        }

        private sealed class AudioSourceHolder
        {
            public IDictionary<string, AudioSource> AudioSources { get; }
            public int Counter { get; private set; } = 1;

            public AudioSourceHolder(IDictionary<string, AudioSource> audioSources)
            {
                AudioSources = audioSources;
            }

            public void Add()
            {
                Counter++;
                foreach (var audioSource in AudioSources.Values.Where(x => !x.isPlaying))
                {
                    audioSource.Play();
                }
            }

            public bool Remove()
            {
                Counter--;
                if (Counter <= 0)
                {
                    foreach (var audioSource in AudioSources.Values)
                    {
                        audioSource.Stop();
                    }
                    return true;
                }

                return false;
            }
        }
    }
}