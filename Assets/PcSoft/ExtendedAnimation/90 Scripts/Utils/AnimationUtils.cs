using System;
using System.Collections;
using UnityEngine;

namespace PcSoft.ExtendedAnimation._90_Scripts.Utils
{
    public static class AnimationUtils
    {
        public static IEnumerator RunAnimation(float preDelay, AnimationCurve curve, float speed, Action<float> action, Action finishAction = null)
        {
            return DoRunAnimation(preDelay, curve, speed, action, () => Time.deltaTime, finishAction);
        }
        
        public static IEnumerator RunAnimation(AnimationCurve curve, float speed, Action<float> action, Action finishAction = null)
        {
            return DoRunAnimation(0f, curve, speed, action, () => Time.deltaTime, finishAction);
        }
        
        public static IEnumerator RunAnimationUnscaled(float preDelay, AnimationCurve curve, float speed, Action<float> action, Action finishAction = null)
        {
            return DoRunAnimation(preDelay, curve, speed, action, () => Time.unscaledDeltaTime, finishAction);
        }
        
        public static IEnumerator RunAnimationUnscaled(AnimationCurve curve, float speed, Action<float> action, Action finishAction = null)
        {
            return DoRunAnimation(0f, curve, speed, action, () => Time.unscaledDeltaTime, finishAction);
        }
        
        private static IEnumerator DoRunAnimation(float preDelay, AnimationCurve curve, float speed, Action<float> action, Func<float> deltaTime, Action finishAction)
        {
            if (preDelay > 0)
                yield return new WaitForSeconds(preDelay);
            
            for (var i = 0f; i < speed; i += deltaTime.Invoke())
            {
                var value = curve.Evaluate(i / speed);
                action(value);

                yield return null;
            }

            action(curve.Evaluate(1f));
            finishAction?.Invoke();
        }

        public static IEnumerator WaitAndRun(float preDelay, float postDelay, Action preAction, Action postAction)
        {
            yield return new WaitForSeconds(preDelay);
            preAction.Invoke();
            
            yield return new WaitForSeconds(postDelay);
            postAction.Invoke();
        }
        
        public static IEnumerator WaitAndRun(float delay, Action onFinished)
        {
            yield return new WaitForSeconds(delay);
            onFinished.Invoke();
        }

        public static IEnumerator WaitAndRun(Action onFinished)
        {
            yield return null;
            onFinished.Invoke();
        }

        public static IEnumerator Delegate(Func<IEnumerator> func)
        {
            return func.Invoke();
        }

        public static IEnumerator RunAll(float delay, params Action[] actions)
        {
            foreach (var action in actions)
            {
                action.Invoke();
                yield return new WaitForSeconds(delay);
            }
        }

        public static IEnumerator RunAll(float delay, uint repeat, Action action, Action onFinished = null)
        {
            for (var i = 0; i < repeat; i++)
            {
                action.Invoke();
                yield return new WaitForSeconds(delay);
            }
            
            onFinished?.Invoke();
        }
    }
}