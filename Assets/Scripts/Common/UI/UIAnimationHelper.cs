using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common.UI
{
    /// <summary>
    /// Utility class providing reusable UI Toolkit animation helpers.
    /// Uses USS transitions and inline style manipulation for smooth animations.
    /// </summary>
    public static class UIAnimationHelper
    {
        /// <summary>
        /// Fade in a visual element from transparent to opaque.
        /// </summary>
        public static void FadeIn(VisualElement element, float durationMs = 300f)
        {
            if (element == null) return;

            element.style.opacity = 0f;
            element.style.display = DisplayStyle.Flex;
            element.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> { new TimeValue(durationMs, TimeUnit.Millisecond) });
            element.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("opacity") });

            // Schedule the opacity change for next frame to trigger transition
            element.schedule.Execute(() =>
            {
                element.style.opacity = 1f;
            }).ExecuteLater(16);
        }

        /// <summary>
        /// Fade out a visual element from opaque to transparent.
        /// </summary>
        public static void FadeOut(VisualElement element, float durationMs = 300f, Action onComplete = null)
        {
            if (element == null) return;

            element.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> { new TimeValue(durationMs, TimeUnit.Millisecond) });
            element.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("opacity") });

            element.style.opacity = 0f;

            element.schedule.Execute(() =>
            {
                element.style.display = DisplayStyle.None;
                onComplete?.Invoke();
            }).ExecuteLater((long)durationMs + 50);
        }

        /// <summary>
        /// Scale bounce effect (scale up then back to normal).
        /// </summary>
        public static void ScaleBounce(VisualElement element, float scaleUp = 1.15f, float durationMs = 150f)
        {
            if (element == null) return;

            element.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> { new TimeValue(durationMs, TimeUnit.Millisecond) });
            element.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("scale") });

            element.style.scale = new StyleScale(new Scale(new Vector3(scaleUp, scaleUp, 1f)));

            element.schedule.Execute(() =>
            {
                element.style.scale = new StyleScale(new Scale(Vector3.one));
            }).ExecuteLater((long)durationMs);
        }

        /// <summary>
        /// Slide in from a direction.
        /// </summary>
        public static void SlideIn(VisualElement element, SlideDirection direction = SlideDirection.Right, float durationMs = 400f)
        {
            if (element == null) return;

            float startX = 0, startY = 0;
            switch (direction)
            {
                case SlideDirection.Left: startX = -100f; break;
                case SlideDirection.Right: startX = 100f; break;
                case SlideDirection.Up: startY = -100f; break;
                case SlideDirection.Down: startY = 100f; break;
            }

            element.style.translate = new StyleTranslate(new Translate(startX, startY));
            element.style.opacity = 0f;
            element.style.display = DisplayStyle.Flex;

            element.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue>
                {
                    new TimeValue(durationMs, TimeUnit.Millisecond),
                    new TimeValue(durationMs, TimeUnit.Millisecond)
                });
            element.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName>
                {
                    new StylePropertyName("translate"),
                    new StylePropertyName("opacity")
                });
            element.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new System.Collections.Generic.List<EasingFunction>
                {
                    new EasingFunction(EasingMode.EaseOutCubic),
                    new EasingFunction(EasingMode.EaseOutCubic)
                });

            element.schedule.Execute(() =>
            {
                element.style.translate = new StyleTranslate(new Translate(0, 0));
                element.style.opacity = 1f;
            }).ExecuteLater(16);
        }

        /// <summary>
        /// Pulse animation (subtle scale oscillation for attention).
        /// </summary>
        public static IVisualElementScheduledItem Pulse(VisualElement element, float minScale = 0.97f, float maxScale = 1.03f, float intervalMs = 1000f)
        {
            if (element == null) return null;

            bool scaleUp = true;

            element.style.transitionDuration = new StyleList<TimeValue>(
                new System.Collections.Generic.List<TimeValue> { new TimeValue(intervalMs / 2f, TimeUnit.Millisecond) });
            element.style.transitionProperty = new StyleList<StylePropertyName>(
                new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("scale") });
            element.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new System.Collections.Generic.List<EasingFunction> { new EasingFunction(EasingMode.EaseInOutSine) });

            return element.schedule.Execute(() =>
            {
                float targetScale = scaleUp ? maxScale : minScale;
                element.style.scale = new StyleScale(new Scale(new Vector3(targetScale, targetScale, 1f)));
                scaleUp = !scaleUp;
            }).Every((long)(intervalMs / 2f));
        }

        /// <summary>
        /// Animate a label's numeric value from one number to another.
        /// </summary>
        public static void AnimateNumber(Label label, int from, int to, float durationMs = 500f, string format = "N0")
        {
            if (label == null) return;

            int steps = Mathf.Max(1, (int)(durationMs / 30f));
            float stepDuration = durationMs / steps;
            float stepValue = (to - from) / (float)steps;

            int currentStep = 0;
            label.schedule.Execute(() =>
            {
                currentStep++;
                int currentValue = (currentStep >= steps) ? to : (int)(from + stepValue * currentStep);
                label.text = currentValue.ToString(format).Replace(",", ".");
            }).Every((long)stepDuration).Until(() => currentStep >= steps);
        }
    }

    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }
}
