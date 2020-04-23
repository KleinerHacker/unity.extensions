using System;
using System.Collections;
using UnityEditor.MemoryProfiler;

namespace PcSoft.ExtendedAnimation._90_Scripts.Utils
{
    public sealed partial class AnimationBuilder
    {
        public bool IsRunning { get; private set; }
        
        public AnimationRunner Start(float delayed, Action onFinished = null)
        {
            if (_steps.Count <= 0)
                throw new InvalidOperationException("No animation inside builder");
            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            IsRunning = true;
            Run(AnimationUtils.WaitAndRun(delayed, () =>
            {
                onFinished?.Invoke();
                StartNext(0);
            }));
            
            return new AnimationRunner(_behaviour);
        }

        public AnimationRunner Start()
        {
            if (_steps.Count <= 0)
                throw new InvalidOperationException("No animation inside builder");
            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            IsRunning = true;
            StartNext(0);
            return new AnimationRunner(_behaviour);
        }

        private void StartNext(int stepIndex)
        {
            if (stepIndex >= _steps.Count)
            {
                _onFinished?.Invoke();
                IsRunning = false;
                
                return;
            }

            var step = _steps[stepIndex];
            if (step is AnimateAnimationStep animStep)
            {
                Run(AnimationUtils.RunAnimation(_type, animStep.Curve, animStep.Speed, animStep.Handler, () =>
                {
                    animStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }));
            }
            else if (step is WaitSecondsAnimationStep waitSecStep)
            {
                Run(AnimationUtils.WaitAndRun(_type, waitSecStep.Seconds, () =>
                {
                    waitSecStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }));
            }
            else if (step is WaitFramesAnimationStep waitFrameStep)
            {
                Run(AnimationUtils.WaitAndRun(waitFrameStep.Frames, () =>
                {
                    waitFrameStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }));
            }
            else if (step is RunAllSecondsAnimationStep runAllSecStep)
            {
                Run(AnimationUtils.RunAll(_type, runAllSecStep.Seconds, () =>
                {
                    runAllSecStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }, runAllSecStep.Actions));
            }
            else if (step is RunAllFramesAnimationStep runAllFramesStep)
            {
                Run(AnimationUtils.RunAll(_type, runAllFramesStep.Frames, () =>
                {
                    runAllFramesStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }, runAllFramesStep.Actions));
            }
            else if (step is RunRepeatSecondsAnimationStep runRepeatSecStep)
            {
                Run(AnimationUtils.RunAll(runRepeatSecStep.Seconds, runRepeatSecStep.RepeatCount, runRepeatSecStep.Action, _type, () =>
                {
                    runRepeatSecStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }));
            }
            else if (step is RunRepeatFramesAnimationStep runRepeatFramesStep)
            {
                Run(AnimationUtils.RunAll(runRepeatFramesStep.Frames, runRepeatFramesStep.RepeatCount, runRepeatFramesStep.Action, _type, () =>
                {
                    runRepeatFramesStep.OnFinished?.Invoke();
                    StartNext(stepIndex + 1);
                }));
            }
            else if (step is ParallelAnimationStep parallelStep)
            {
                var builder = Create(_behaviour, _type);
                builder = parallelStep.BuildAction.Invoke(builder);

                if (!builder.IsRunning)
                {
                    builder.Start();
                }
            }
            else
                throw new NotImplementedException(step.GetType().FullName);
        }

        private void Run(IEnumerator animation)
        {
            _behaviour.StartCoroutine(animation);
        }
    }
}