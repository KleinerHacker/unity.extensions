using System;
using System.Collections;
using PcSoft.ExtendedAnimation._90_Scripts.Types;
using UnityEngine;

namespace PcSoft.ExtendedAnimation._90_Scripts.Utils
{
    public static class AnimationUtils
    {
        public static IEnumerator RunAnimation(AnimationCurve curve, float speed, Action<float> handler, Action onFinished = null)
        {
            return RunAnimation(AnimationType.Scaled, curve, speed, handler, onFinished);
        }
        
        public static IEnumerator RunAnimation(float preDelay, AnimationCurve curve, float speed, Action<float> handler, Action onFinished)
        {
            return RunAnimation(AnimationType.Scaled, preDelay, curve, speed, handler, onFinished);
        }
        
        public static IEnumerator RunAnimation(AnimationType type, AnimationCurve curve, float speed, Action<float> handler, Action onFinished = null)
        {
            return RunAnimation(type, 0f, curve, speed, handler, onFinished);
        }
        
        public static IEnumerator RunAnimation(AnimationType type, float preDelay, AnimationCurve curve, float speed, Action<float> handler, Action onFinished)
        {
            if (preDelay > 0f)
            {
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(preDelay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(preDelay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            for (var i = 0f; i < speed; i += GetDelta(type))
            {
                var value = curve.Evaluate(i / speed);
                handler(value);

                yield return null;
            }

            handler(curve.Evaluate(1f));
            onFinished?.Invoke();
        }
        
        public static IEnumerator RunAnimationConstant(float time, Action<float> handler, Action onFinished)
        {
            return RunAnimationConstant(AnimationType.Scaled, 0f, time, handler, onFinished);
        }

        public static IEnumerator RunAnimationConstant(float preDelay, float time, Action<float> handler, Action onFinished)
        {
            return RunAnimationConstant(AnimationType.Scaled, preDelay, time, handler, onFinished);
        }

        public static IEnumerator RunAnimationConstant(AnimationType type, float time, Action<float> handler, Action onFinished)
        {
            return RunAnimationConstant(type, 0f, time, handler, onFinished);
        }

        public static IEnumerator RunAnimationConstant(AnimationType type, float preDelay, float time, Action<float> handler, Action onFinished)
        {
            if (preDelay > 0f)
            {
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(preDelay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(preDelay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            for (var i = 0f; i < time; i += GetDelta(type))
            {
                var value = i / time;
                handler(value);

                yield return null;
            }

            onFinished?.Invoke();
        }

        public static IEnumerator WaitAndRun(float preDelay, float postDelay, Action preAction, Action postAction)
        {
            return WaitAndRun(preDelay, postDelay, AnimationType.Scaled, preAction, postAction);
        }

        public static IEnumerator WaitAndRun(float preDelay, float postDelay, AnimationType type, Action preAction, Action postAction)
        {
            switch (type)
            {
                case AnimationType.Scaled:
                    yield return new WaitForSeconds(preDelay);
                    break;
                case AnimationType.Unscaled:
                    yield return new WaitForSecondsRealtime(preDelay);
                    break;
                default:
                    throw new NotImplementedException();
            }
            preAction.Invoke();
            
            switch (type)
            {
                case AnimationType.Scaled:
                    yield return new WaitForSeconds(postDelay);
                    break;
                case AnimationType.Unscaled:
                    yield return new WaitForSecondsRealtime(postDelay);
                    break;
                default:
                    throw new NotImplementedException();
            }
            postAction.Invoke();
        }

        public static IEnumerator WaitAndRun(float delay, Action onFinished)
        {
            return WaitAndRun(AnimationType.Scaled, delay, onFinished);
        }
        
        public static IEnumerator WaitAndRun(AnimationType type, float delay, Action onFinished)
        {
            switch (type)
            {
                case AnimationType.Scaled:
                    yield return new WaitForSeconds(delay);
                    break;
                case AnimationType.Unscaled:
                    yield return new WaitForSecondsRealtime(delay);
                    break;
                default:
                    throw new NotImplementedException();
            }
            onFinished.Invoke();
        }

        public static IEnumerator WaitAndRun(uint frames, Action onFinished)
        {
            for (var i = 0; i < frames; i++)
            {
                yield return null;                
            }
            onFinished.Invoke();
        }

        public static IEnumerator Delegate(Func<IEnumerator> func)
        {
            return func.Invoke();
        }

        public static IEnumerator RunAll(float delay, params Action[] actions)
        {
            return RunAll(AnimationType.Scaled, delay, null, actions);
        }
        
        public static IEnumerator RunAll(AnimationType type, float delay, Action onFinished, params Action[] actions)
        {
            foreach (var action in actions)
            {
                action.Invoke();
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(delay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(delay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            
            onFinished?.Invoke();
        }
        
        public static IEnumerator RunAll(uint frames, params Action[] actions)
        {
            return RunAll(frames, null, actions);
        }
        
        public static IEnumerator RunAll(uint frames, Action onFinished, params Action[] actions)
        {
            foreach (var action in actions)
            {
                action.Invoke();
                for (var i = 0; i < frames; i++)
                {
                    yield return null;
                }
            }
            
            onFinished?.Invoke();
        }

        public static IEnumerator RunAll(float delay, uint repeat, Action<int> handler, AnimationType type = AnimationType.Scaled, Action onFinished = null)
        {
            for (var i = 0; i < repeat; i++)
            {
                handler.Invoke(i);
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(delay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(delay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            
            onFinished?.Invoke();
        }
        
        public static IEnumerator RunAll(uint frames, uint repeat, Action<int> handler, Action onFinished = null)
        {
            for (var i = 0; i < repeat; i++)
            {
                handler.Invoke(i);
                for (var j = 0; j < frames; j++)
                {
                    yield return null;
                }
            }
            
            onFinished?.Invoke();
        }

        private static float GetDelta(AnimationType type)
        {
            switch (type)
            {
                case AnimationType.Scaled:
                    return Time.deltaTime;
                case AnimationType.Unscaled:
                    return Time.unscaledDeltaTime;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}