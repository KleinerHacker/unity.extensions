using System;
using System.Linq;
using UnityEngine;

namespace PcSoft.TrafficLight._90_Scripts._00_Runtime.Components
{
    [AddComponentMenu(TrafficLightConstants.Root + "/Traffic Light Controller")]
    [DisallowMultipleComponent]
    public sealed class TrafficLightController : MonoBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private TrafficLightSwitchSystemItem[] switchItems;

        #endregion

        private int _index = 0;
        private float _counter = 0f;
        private bool _inSwitchState = false;

        #region Builtin Methods

        private void OnEnable()
        {
            _index = 0;
            _counter = 0f;
            
            foreach (var trafficLight in switchItems.SelectMany(x => x.TrafficLights).Distinct())
            {
                trafficLight.Initialize();
            }

            RunSwitch();
        }

        private void LateUpdate()
        {
            if (_inSwitchState)
                return;

            _counter += Time.deltaTime;
            if (_counter >= switchItems[_index].DelayTime)
            {
                _index++;
                if (_index >= switchItems.Length)
                {
                    _index = 0;
                }
                
                RunSwitch();

                _counter = 0f;
            }
        }

        #endregion

        private void RunSwitch()
        {
            Debug.Log("Run Switch for " + _index, this);
            
            _inSwitchState = true;
            foreach (var trafficLight in switchItems[_index].TrafficLights)
            {
                switch (switchItems[_index].SwitchType)
                {
                    case TrafficLightSwitchType.SwitchToRed:
                        trafficLight.SwitchRed(() => _inSwitchState = false);
                        break;
                    case TrafficLightSwitchType.SwitchToGreen:
                        trafficLight.SwitchGreen(() => _inSwitchState = false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }

    [Serializable]
    public sealed class TrafficLightSwitchSystemItem
    {
        #region Inspector Data

        [SerializeField]
        private TrafficLight[] trafficLights;

        [SerializeField]
        private TrafficLightSwitchType switchType;

        [SerializeField]
        private float delayTime = 1f;

        #endregion

        #region Properties

        public TrafficLight[] TrafficLights => trafficLights;

        public TrafficLightSwitchType SwitchType => switchType;

        public float DelayTime => delayTime;

        #endregion
    }

    public enum TrafficLightSwitchType
    {
        SwitchToGreen,
        SwitchToRed
    }

    public enum TrafficLightInitialState
    {
        Red,
        Green,
        OutOfOrder
    }
}