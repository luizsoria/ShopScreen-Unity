using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shop.UI
{
    /// <summary>
    /// Controller for offer countdown timers.
    /// Displays a countdown timer on offer cards to create urgency.
    /// Updates every second and fires an event when the timer expires.
    /// </summary>
    public class OfferTimerController
    {
        private readonly Label _timerLabel;
        private readonly VisualElement _timerContainer;
        private IVisualElementScheduledItem _timerSchedule;
        private DateTime _expirationTime;
        private Action _onExpired;

        public VisualElement Root => _timerContainer;

        public OfferTimerController()
        {
            _timerContainer = new VisualElement();
            _timerContainer.name = "offer-timer";
            _timerContainer.AddToClassList("offer-timer");

            var clockIcon = new VisualElement();
            clockIcon.AddToClassList("offer-timer__icon");
            _timerContainer.Add(clockIcon);

            _timerLabel = new Label("00:00:00");
            _timerLabel.AddToClassList("offer-timer__text");
            _timerContainer.Add(_timerLabel);
        }

        /// <summary>
        /// Start the countdown timer.
        /// </summary>
        /// <param name="duration">Duration of the offer.</param>
        /// <param name="onExpired">Callback when timer reaches zero.</param>
        public void StartTimer(TimeSpan duration, Action onExpired = null)
        {
            _expirationTime = DateTime.Now + duration;
            _onExpired = onExpired;

            // Update immediately
            UpdateTimerDisplay();

            // Schedule updates every second
            _timerSchedule = _timerContainer.schedule.Execute(UpdateTimerDisplay).Every(1000);
        }

        /// <summary>
        /// Start with a specific expiration time.
        /// </summary>
        public void StartTimer(DateTime expirationTime, Action onExpired = null)
        {
            _expirationTime = expirationTime;
            _onExpired = onExpired;

            UpdateTimerDisplay();
            _timerSchedule = _timerContainer.schedule.Execute(UpdateTimerDisplay).Every(1000);
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void StopTimer()
        {
            _timerSchedule?.Pause();
        }

        private void UpdateTimerDisplay()
        {
            var remaining = _expirationTime - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                _timerLabel.text = "EXPIRED";
                _timerContainer.AddToClassList("offer-timer--expired");
                _timerSchedule?.Pause();
                _onExpired?.Invoke();
                return;
            }

            if (remaining.TotalHours >= 24)
            {
                int days = (int)remaining.TotalDays;
                _timerLabel.text = $"{days}d {remaining.Hours:D2}h";
            }
            else if (remaining.TotalHours >= 1)
            {
                _timerLabel.text = $"{(int)remaining.TotalHours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }
            else
            {
                _timerLabel.text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                // Add urgency style when less than 5 minutes
                if (remaining.TotalMinutes < 5)
                {
                    _timerContainer.AddToClassList("offer-timer--urgent");
                }
            }
        }
    }
}
