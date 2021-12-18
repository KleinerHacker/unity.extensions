using System;
using System.Linq;
using PcSoft.TrafficLight._90_Scripts._00_Runtime.Assets;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Utils;
using UnityEngine;

namespace PcSoft.TrafficLight._90_Scripts._00_Runtime.Components
{
    [AddComponentMenu(TrafficLightConstants.Root + "/Traffic Light")]
    public sealed class TrafficLight : MonoBehaviour
    {
        private const float DarkEmissionFactor = 10f;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        #region Inspector Data

        [SerializeField]
        internal TrafficLightInitialState initialState = TrafficLightInitialState.OutOfOrder;

        [SerializeField]
        private TrafficLightSwitchAsset trafficLightSwitch;

        [Header("Switch Animation")]
        [SerializeField]
        private AnimationCurve lightSwitchCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private float lightSwitchSpeed = 0.3f;

        [Header("Lights")]
        [SerializeField]
        private TrafficLightItem redLight;

        [SerializeField]
        private TrafficLightItem yellowLight;

        [SerializeField]
        private TrafficLightItem greenLight;

        #endregion

        #region Properties

        public TrafficLightState State { get; private set; } = TrafficLightState.OutOfOrder;

        #endregion

        private float _counter = 0f;
        private TrafficLightBlinkState _blinkState = TrafficLightBlinkState.On;
        private byte _blinkCounter = 0;
        private Action _blinkFinished = null;

        #region Builtin Methods

        private void LateUpdate()
        {
            if ((State != TrafficLightState.GreenFinish && State != TrafficLightState.OutOfOrder) || _blinkState == TrafficLightBlinkState.Switch)
                return;

            _counter += Time.deltaTime;
            if (_counter >= trafficLightSwitch.BlinkDuration)
            {
                Debug.Log("Blink in state " + State + " to " + _blinkState, this);
                switch (_blinkState)
                {
                    case TrafficLightBlinkState.On:
                        _blinkState = TrafficLightBlinkState.Switch;
                        AnimationBuilder.Create(this)
                            .Animate(lightSwitchCurve, lightSwitchSpeed,
                                v => HandleLightItem(State == TrafficLightState.GreenFinish ? greenLight : yellowLight, TrafficLightBehavior.TurnOff, v),
                                () => _blinkState = TrafficLightBlinkState.Off
                            )
                            .Start();

                        _blinkCounter++;
                        if (State == TrafficLightState.GreenFinish && _blinkCounter >= trafficLightSwitch.BlinkGreenOnFinishCount)
                        {
                            DoSwitchRed(() =>
                            {
                                _blinkFinished?.Invoke();
                                _blinkFinished = null;
                                _blinkState = TrafficLightBlinkState.On;
                            });
                        }

                        break;
                    case TrafficLightBlinkState.Off:
                        _blinkState = TrafficLightBlinkState.Switch;
                        AnimationBuilder.Create(this)
                            .Animate(lightSwitchCurve, lightSwitchSpeed,
                                v => HandleLightItem(State == TrafficLightState.GreenFinish ? greenLight : yellowLight, TrafficLightBehavior.TurnOn, v),
                                () => _blinkState = TrafficLightBlinkState.On
                            ).Start();
                        break;
                    case TrafficLightBlinkState.Switch:
                        throw new NotSupportedException();
                    default:
                        throw new NotImplementedException();
                }

                _counter = 0f;
            }
        }

        #endregion

        public void SwitchRed(Action onFinished = null, bool immediately = false)
        {
            if (State == TrafficLightState.Red || State == TrafficLightState.GreenFinish)
            {
                Debug.LogWarning("Traffic Light State already RED or GREEN FINISHED", this);
                return;
            }

            if (State == TrafficLightState.Switching)
            {
                Debug.LogWarning("Traffic Light State is switching, unable to change to RED", this);
                return;
            }

            if (immediately)
            {
                Debug.Log("Switch to red immediately", this);

                SwitchOn(redLight);
                SwitchOff(yellowLight);
                SwitchOff(greenLight);

                State = TrafficLightState.Red;
                onFinished?.Invoke();
            }
            else
            {
                if (trafficLightSwitch.BlinkGreenOnFinish)
                {
                    Debug.Log("Switch to green finished (way to red)", this);
                    DoSwitchGreenFinish(onFinished);
                }
                else
                {
                    Debug.Log("Switch to red", this);
                    DoSwitchRed(onFinished);
                }
            }
        }

        public void SwitchGreen(Action onFinished = null, bool immediately = false)
        {
            if (State == TrafficLightState.Green)
            {
                Debug.LogWarning("Traffic Light State already GREEN", this);
                return;
            }

            if (State == TrafficLightState.Switching)
            {
                Debug.LogWarning("Traffic Light State is switching, unable to change to GREEN", this);
                return;
            }

            if (immediately)
            {
                Debug.Log("Switch to green immediately", this);

                SwitchOff(redLight);
                SwitchOff(yellowLight);
                SwitchOn(greenLight);

                State = TrafficLightState.Green;
                onFinished?.Invoke();
            }
            else
            {
                Debug.Log("Switch to green", this);

                State = TrafficLightState.Switching;
                SwitchDynamic(trafficLightSwitch.SwitchToGreen, () =>
                {
                    State = TrafficLightState.Green;
                    onFinished?.Invoke();
                });
            }
        }

        public void SwitchOutOfOrder(Action onFinished = null, bool immediately = false)
        {
            if (State == TrafficLightState.OutOfOrder)
            {
                Debug.LogWarning("Traffic Light State already OUT OF ORDER", this);
                return;
            }

            if (State == TrafficLightState.Switching)
            {
                Debug.LogWarning("Traffic Light State is switching, unable to change to OUT OF ORDER", this);
                return;
            }

            if (immediately)
            {
                Debug.Log("Switch to out of order immediately", this);

                SwitchOff(redLight);
                SwitchOn(yellowLight);
                SwitchOff(greenLight);

                State = TrafficLightState.OutOfOrder;
                onFinished?.Invoke();
            }
            else
            {
                Debug.Log("Switch to out of order", this);
                AnimationBuilder.Create(this)
                    .Animate(lightSwitchCurve, lightSwitchSpeed, v =>
                    {
                        HandleLightItem(redLight, TrafficLightBehavior.TurnOff, v);
                        HandleLightItem(greenLight, TrafficLightBehavior.TurnOff, v);
                        HandleLightItem(yellowLight, TrafficLightBehavior.TurnOn, v);
                    })
                    .WithFinisher(onFinished)
                    .Start();
            }
        }

        private void DoSwitchRed(Action onFinished)
        {
            State = TrafficLightState.Switching;
            SwitchDynamic(trafficLightSwitch.SwitchToRed, () =>
            {
                State = TrafficLightState.Red;
                onFinished?.Invoke();
            });
        }

        private void DoSwitchGreenFinish(Action onFinished)
        {
            if (State == TrafficLightState.GreenFinish)
            {
                Debug.LogWarning("Traffic Light State already GREEN FINISH", this);
                return;
            }

            if (State == TrafficLightState.Switching)
            {
                Debug.LogWarning("Traffic Light State is switching, unable to change to GREEN FINISH", this);
                return;
            }

            if (State != TrafficLightState.Green)
            {
                Debug.Log("Switch to green (for finishing)", this);

                State = TrafficLightState.Switching;
                SwitchDynamic(trafficLightSwitch.SwitchToGreen, () =>
                {
                    _blinkCounter = 0;
                    _blinkFinished = onFinished;

                    State = TrafficLightState.GreenFinish;
                });
            }
            else
            {
                Debug.Log("Green Finishing set", this);

                _blinkCounter = 0;
                _blinkFinished = onFinished;

                State = TrafficLightState.GreenFinish;
            }
        }

        private void SwitchOff(TrafficLightItem item)
        {
            foreach (var light in item.Lights)
            {
                light.Light.intensity = 0f;
            }

            foreach (var renderer in item.LightRenderers)
            {
                renderer.Renderer.materials[renderer.MaterialIndex].SetColor(EmissionColor, Color.clear);
            }
        }

        private void SwitchOn(TrafficLightItem item)
        {
            foreach (var light in item.Lights)
            {
                light.Light.intensity = light.TargetIntensity;
            }

            foreach (var renderer in item.LightRenderers)
            {
                renderer.Renderer.materials[renderer.MaterialIndex].SetColor(EmissionColor, renderer.TargetColor);
            }
        }

        private void SwitchDynamic(TrafficLightSwitch @switch, Action onFinished)
        {
            var builder = AnimationBuilder.Create(this);
            foreach (var step in @switch.Steps)
            {
                var lightList = step.Items.Select(x => (GetLight(x.Type), x.Behavior)).ToList();
                builder
                    .Animate(lightSwitchCurve, lightSwitchSpeed, v =>
                    {
                        foreach (var (item, behavior) in lightList)
                        {
                            HandleLightItem(item, behavior, v);
                        }
                    })
                    .Wait(step.ShowTime);
            }

            builder
                .WithFinisher(onFinished)
                .Start();
        }

        private void HandleLightItem(TrafficLightItem item, TrafficLightBehavior behavior, float v)
        {
            float value;
            switch (behavior)
            {
                case TrafficLightBehavior.TurnOn:
                    value = v;
                    break;
                case TrafficLightBehavior.TurnOff:
                    value = 1f - v;
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var light in item.Lights)
            {
                light.Light.intensity = light.TargetIntensity * value;
            }

            foreach (var renderer in item.LightRenderers)
            {
                renderer.Renderer.materials[renderer.MaterialIndex].SetColor(EmissionColor,
                    Color.Lerp(Color.clear, renderer.TargetColor, value));
            }
        }

        private TrafficLightItem GetLight(TrafficLightType type)
        {
            switch (type)
            {
                case TrafficLightType.Red:
                    return redLight;
                case TrafficLightType.Yellow:
                    return yellowLight;
                case TrafficLightType.Green:
                    return greenLight;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void Initialize()
        {
            switch (initialState)
            {
                case TrafficLightInitialState.Red:
                    SwitchRed(immediately: true);
                    break;
                case TrafficLightInitialState.Green:
                    SwitchGreen(immediately: true);
                    break;
                case TrafficLightInitialState.OutOfOrder:
                    SwitchOutOfOrder(immediately: true);
                    break;
                default:
                    throw new NotImplementedException();
            }

            _counter = 0f;
            _blinkCounter = 0;
            _blinkState = TrafficLightBlinkState.On;
        }
    }

    [Serializable]
    public sealed class TrafficLightItem
    {
        #region Inspector Data

        [SerializeField]
        private TrafficLightRendererItem[] lightRenderers;

        [SerializeField]
        private TrafficLightLightItem[] lights;

        #endregion

        #region Properties

        public TrafficLightRendererItem[] LightRenderers => lightRenderers;

        public TrafficLightLightItem[] Lights => lights;

        #endregion
    }

    [Serializable]
    public sealed class TrafficLightRendererItem
    {
        #region Inspector Data

        [SerializeField]
        private MeshRenderer renderer;

        [SerializeField]
        private byte materialIndex = 0;

        [ColorUsage(false, true)]
        [SerializeField]
        private Color targetColor;

        #endregion

        #region Properties

        public MeshRenderer Renderer => renderer;

        public byte MaterialIndex => materialIndex;

        public Color TargetColor => targetColor;

        #endregion
    }

    [Serializable]
    public sealed class TrafficLightLightItem
    {
        #region Inspector Data

        [SerializeField]
        private Light light;

        [SerializeField]
        private float targetIntensity = 1f;

        #endregion

        #region Properties

        public Light Light => light;

        public float TargetIntensity => targetIntensity;

        #endregion
    }

    public enum TrafficLightState
    {
        Red,
        Green,
        GreenFinish,
        Switching,
        OutOfOrder
    }

    public enum TrafficLightBlinkState
    {
        On,
        Off,
        Switch
    }
}