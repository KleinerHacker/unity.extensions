using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityExtensions;

namespace PcSoft.TrafficLight._90_Scripts._00_Runtime.Assets
{
    [CreateAssetMenu(menuName = TrafficLightConstants.Root + "/Traffic Light Switch")]
    public sealed class TrafficLightSwitchAsset : ScriptableObject
    {
        #region Inspector Data

        [SerializeField]
        private TrafficLightSwitch switchToGreen;
        
        [SerializeField]
        private TrafficLightSwitch switchToRed;

        [FormerlySerializedAs("blinkGreenOnFinishDuration")]
        [Header("Blinking")]
        [SerializeField]
        private float blinkDuration = 1f;

        [SerializeField]
        private bool blinkGreenOnFinish = false;

        [SerializeField]
        [Range(1, 100)]
        private byte blinkGreenOnFinishCount = 5;

        #endregion

        #region Properties

        public TrafficLightSwitch SwitchToGreen => switchToGreen;

        public TrafficLightSwitch SwitchToRed => switchToRed;

        public bool BlinkGreenOnFinish => blinkGreenOnFinish;

        public float BlinkDuration => blinkDuration;

        public byte BlinkGreenOnFinishCount => blinkGreenOnFinishCount;

        #endregion
    }

    [Serializable]
    public sealed class TrafficLightSwitch
    {
        #region Inspector Data

        [ReorderableList(elementsAreSubassets = true, hideFooterButtons = true)]
        [SerializeField]
        private TrafficLightSwitchStep[] steps;

        #endregion

        #region Properties

        public TrafficLightSwitchStep[] Steps => steps;

        #endregion
    }

    [Serializable]
    public sealed class TrafficLightSwitchStep
    {
        #region Inspector Data

        [ReorderableList(elementsAreSubassets = true, hideFooterButtons = true)]
        [SerializeField]
        private TrafficLightSwitchItem[] items;

        [SerializeField]
        private float showTime = 1f;

        #endregion

        #region Properties

        public TrafficLightSwitchItem[] Items => items;

        public float ShowTime => showTime;

        #endregion
    }

    [Serializable]
    public sealed class TrafficLightSwitchItem
    {
        #region Inspector Data

        [SerializeField]
        private TrafficLightType type;

        [SerializeField]
        private TrafficLightBehavior behavior;

        #endregion

        #region Properties

        public TrafficLightType Type => type;

        public TrafficLightBehavior Behavior => behavior;

        #endregion
    }

    public enum TrafficLightType
    {
        Red,
        Yellow,
        Green
    }

    public enum TrafficLightBehavior
    {
        TurnOn,
        TurnOff,
    }
}