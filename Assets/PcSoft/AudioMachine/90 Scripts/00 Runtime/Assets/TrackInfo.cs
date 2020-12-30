using System;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets
{
    [Serializable]
    public sealed class TrackInfo
    {
        #region Inspector Data

        [SerializeField]
        private string identifier;

        [SerializeField]
        private AudioClip clip;

        [Header("Misc")]
        [SerializeField]
        private float startDelay;

        #endregion

        #region Properties

        public string Identifier => identifier;

        public AudioClip Clip => clip;

        public float StartDelay => startDelay;

        #endregion
    } 
}