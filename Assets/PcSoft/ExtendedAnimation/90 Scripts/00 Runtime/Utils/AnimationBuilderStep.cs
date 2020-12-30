using System;
using UnityEngine;

namespace PcSoft.ExtendedAnimation._90_Scripts._00_Runtime.Utils
{
    public sealed partial class AnimationBuilder
    {
        private abstract class AnimationStep
        {
            public Action OnFinished { get; }

            protected AnimationStep(Action onFinished)
            {
                OnFinished = onFinished;
            }
        }

        private sealed class AnimateConstantAnimationStep : AnimationStep
        {
            public float Time { get; }
            public Action<float> Handler { get; }

            public AnimateConstantAnimationStep(float time, Action<float> handler, Action onFinished) : base(onFinished)
            {
                Time = time;
                Handler = handler;
            }
        }

        private sealed class AnimateAnimationStep : AnimationStep
        {
            public AnimationCurve Curve { get; }
            public float Speed { get; }
            public Action<float> Handler { get; }

            public AnimateAnimationStep(AnimationCurve curve, float speed, Action<float> handler, Action onFinished) : base(onFinished)
            {
                Curve = curve;
                Speed = speed;
                Handler = handler;
            }
        }

        private sealed class WaitSecondsAnimationStep : AnimationStep
        {
            public float Seconds { get; }

            public WaitSecondsAnimationStep(float seconds, Action onFinished) : base(onFinished)
            {
                Seconds = seconds;
            }
        }
        
        private sealed class WaitFramesAnimationStep : AnimationStep
        {
            public uint Frames { get; }

            public WaitFramesAnimationStep(uint frames, Action onFinished) : base(onFinished)
            {
                Frames = frames;
            }
        }

        private sealed class RunAllSecondsAnimationStep : AnimationStep
        {
            public float Seconds { get; }
            public Action[] Actions { get; }

            public RunAllSecondsAnimationStep(float seconds, Action[] actions, Action onFinished) : base(onFinished)
            {
                Seconds = seconds;
                Actions = actions;
            }
        }
        
        private sealed class RunAllFramesAnimationStep : AnimationStep
        {
            public uint Frames { get; }
            public Action[] Actions { get; }

            public RunAllFramesAnimationStep(uint frames, Action[] actions, Action onFinished) : base(onFinished)
            {
                Frames = frames;
                Actions = actions;
            }
        }
        
        private sealed class RunRepeatSecondsAnimationStep : AnimationStep
        {
            public float Seconds { get; }
            public uint RepeatCount { get; }
            public Action<int> Action { get; }

            public RunRepeatSecondsAnimationStep(float seconds, uint repeatCount, Action<int> action, Action onFinished) : base(onFinished)
            {
                Seconds = seconds;
                RepeatCount = repeatCount;
                Action = action;
            }
        }
        
        private sealed class RunRepeatFramesAnimationStep : AnimationStep
        {
            public uint Frames { get; }
            public uint RepeatCount { get; }
            public Action<int> Action { get; }

            public RunRepeatFramesAnimationStep(uint frames, uint repeatCount, Action<int> action, Action onFinished) : base(onFinished)
            {
                Frames = frames;
                RepeatCount = repeatCount;
                Action = action;
            }
        }

        private sealed class ParallelAnimationStep : AnimationStep
        {
            public Func<AnimationBuilder, AnimationBuilder> BuildAction { get; }

            public ParallelAnimationStep(Func<AnimationBuilder, AnimationBuilder> buildAction, Action onFinished) : base(onFinished)
            {
                BuildAction = buildAction;
            }
        }
    }
}