using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Music;
using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Music.Base;
using PcSoft.AudioMachine._90_Scripts._00_Runtime.Utils;
using UnityAnimation.Runtime.animation.Scripts.Types;
using UnityAnimation.Runtime.animation.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Components.Music
{
    [AddComponentMenu(AudioMachineConstants.Menus.Components.MusicMenu + "/Music Machine")]
    public sealed class MusicMachine : MonoBehaviour
    {
        #region Static Area

        public static MusicMachine Singleton => Resources.FindObjectsOfTypeAll<MusicMachine>()[0];

        #endregion

        #region Inspector Data

        [SerializeField]
        private AudioMixerGroup audioMixerGroup;

        [FormerlySerializedAs("overlapCurve")]
        [Header("Transition")]
        [SerializeField]
        private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [FormerlySerializedAs("overlapSpeed")]
        [SerializeField]
        private float transitionSpeed = 1f;

        #endregion

        private readonly IDictionary<string, AudioSource> _currentAudioSources = new Dictionary<string, AudioSource>();
        private CollectionAudioHelper<DirectMusicSourceAsset> _extendedAudioHelper;
        private CollectionAudioHelper<AudioClip> _simpleAudioHelper;

        #region Builtin Methods

        private void LateUpdate()
        {
            if (_currentAudioSources.Any(x => !x.Value.isPlaying))
            {
                if (_extendedAudioHelper != null)
                {
                    Debug.Log("Play next music");

                    var randomSource = _extendedAudioHelper.Next();

                    DoStopAsync();
                    UpdateAudioSources(randomSource, false);
                    DoStartAsync();
                }
                else if (_simpleAudioHelper != null)
                {
                    Debug.Log("Play next music");

                    var randomClip = _simpleAudioHelper.Next();

                    DoStop();

                    var newAudioSource = CreateAudioSource(false);
                    newAudioSource.clip = randomClip;
                    newAudioSource.Play();

                    _currentAudioSources.Add("Default", newAudioSource);
                    DoStart();
                }
            }
        }

        #endregion

        public void Play(MusicSourceAsset musicSource)
        {
            Stop();

            UpdateAudioSources(musicSource);
            DoStartAsync();
        }

        public void Stop()
        {
            DoStopAsync();
            _currentAudioSources.Clear();
        }

        private void UpdateAudioSources(MusicSourceAsset musicSource, bool loop = true)
        {
            if (musicSource == null)
                throw new ArgumentException("music Source == null");

            Debug.Log("Play new music");

            _extendedAudioHelper = null;
            _simpleAudioHelper = null;
            if (musicSource is SimpleMusicSourceAsset simpleMusicSource)
            {
                var newAudioSource = CreateAudioSource(loop);
                newAudioSource.clip = simpleMusicSource.Clip;
                newAudioSource.Play();

                _currentAudioSources.Add("Default", newAudioSource);
            }
            else if (musicSource is MultiTrackMusicSourceAsset multiTrackMusicSource)
            {
                foreach (var track in multiTrackMusicSource.Tracks)
                {
                    var newAudioSource = CreateAudioSource(loop);
                    newAudioSource.clip = track.Clip;
                    newAudioSource.PlayDelayed(track.StartDelay);

                    _currentAudioSources.Add(track.Identifier, newAudioSource);
                }
            }
            else if (musicSource is ExtendedCollectionMusicSourceAsset extendedCollectionMusicSource)
            {
                _extendedAudioHelper = new CollectionAudioHelper<DirectMusicSourceAsset>(extendedCollectionMusicSource.PlayBehavior, extendedCollectionMusicSource.MusicSources);
                var randomSource = _extendedAudioHelper.Next();

                UpdateAudioSources(randomSource, false);
            }
            else if (musicSource is SimpleCollectionMusicSourceAsset simpleCollectionMusicSource)
            {
                _simpleAudioHelper = new CollectionAudioHelper<AudioClip>(simpleCollectionMusicSource.PlayBehavior, simpleCollectionMusicSource.Clips);
                var randomClip = _simpleAudioHelper.Next();

                var newAudioSource = CreateAudioSource(false);
                newAudioSource.clip = randomClip;
                newAudioSource.Play();

                _currentAudioSources.Add("Default", newAudioSource);
            }
            else
                throw new NotImplementedException();
        }

        private void DoStartAsync()
        {
            foreach (var audioSource in _currentAudioSources.ToArray())
            {
                AnimationBuilder.Create(this, AnimationType.Unscaled)
                    .Animate(transitionCurve, transitionSpeed, v => audioSource.Value.volume = v)
                    .Start();
            }
        }

        private void DoStopAsync()
        {
            var audioSources = _currentAudioSources.Values.ToArray();

            foreach (var audioSource in audioSources)
            {
                AnimationBuilder.Create(this, AnimationType.Unscaled)
                    .Animate(transitionCurve, transitionSpeed, v => audioSource.volume = 1f - v, () => Destroy(audioSource))
                    .Start();
            }

            _currentAudioSources.Clear();
        }

        private void DoStart()
        {
            foreach (var audioSource in _currentAudioSources.ToArray())
            {
                audioSource.Value.volume = 1f;
            }
        }

        private void DoStop()
        {
            var audioSources = _currentAudioSources.Values.ToArray();

            foreach (var audioSource in audioSources)
            {
                Destroy(audioSource);
            }

            _currentAudioSources.Clear();
        }

        private AudioSource CreateAudioSource(bool loop)
        {
            var newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            newAudioSource.volume = 0f;
            newAudioSource.loop = loop;
            newAudioSource.outputAudioMixerGroup = audioMixerGroup;

            return newAudioSource;
        }
    }
}