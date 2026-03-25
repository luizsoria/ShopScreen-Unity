using UnityEngine;
using UnityEngine.UIElements;
using Common.UI;

namespace Shop.UI
{
    /// <summary>
    /// Controller for purchase feedback toast/notification.
    /// Shows a brief animated message after a successful purchase.
    /// Auto-hides after a configurable duration.
    /// </summary>
    public class PurchaseFeedbackController
    {
        private readonly VisualElement _toast;
        private readonly VisualElement _iconElement;
        private readonly Label _messageLabel;
        private IVisualElementScheduledItem _hideSchedule;

        private const float SHOW_DURATION_MS = 2500f;
        private const float ANIMATION_DURATION_MS = 300f;

        public VisualElement Root => _toast;

        public PurchaseFeedbackController()
        {
            _toast = new VisualElement();
            _toast.name = "purchase-feedback";
            _toast.AddToClassList("purchase-feedback");
            _toast.style.display = DisplayStyle.None;

            // Icon
            _iconElement = new VisualElement();
            _iconElement.AddToClassList("purchase-feedback__icon");
            _toast.Add(_iconElement);

            // Message
            _messageLabel = new Label("Purchase successful!");
            _messageLabel.AddToClassList("purchase-feedback__message");
            _toast.Add(_messageLabel);
        }

        /// <summary>
        /// Show a success feedback message.
        /// </summary>
        public void ShowSuccess(string message)
        {
            Show(message, FeedbackType.Success);
        }

        /// <summary>
        /// Show an error feedback message.
        /// </summary>
        public void ShowError(string message)
        {
            Show(message, FeedbackType.Error);
        }

        private void Show(string message, FeedbackType type)
        {
            // Cancel any pending hide
            _hideSchedule?.Pause();

            _messageLabel.text = message;

            // Apply type-specific styles
            _toast.RemoveFromClassList("purchase-feedback--success");
            _toast.RemoveFromClassList("purchase-feedback--error");

            _iconElement.RemoveFromClassList("purchase-feedback__icon--success");
            _iconElement.RemoveFromClassList("purchase-feedback__icon--error");

            switch (type)
            {
                case FeedbackType.Success:
                    _toast.AddToClassList("purchase-feedback--success");
                    _iconElement.AddToClassList("purchase-feedback__icon--success");
                    break;
                case FeedbackType.Error:
                    _toast.AddToClassList("purchase-feedback--error");
                    _iconElement.AddToClassList("purchase-feedback__icon--error");
                    break;
            }

            // Animate in
            UIAnimationHelper.SlideIn(_toast, SlideDirection.Down, ANIMATION_DURATION_MS);

            // Schedule auto-hide
            _hideSchedule = _toast.schedule.Execute(() =>
            {
                UIAnimationHelper.FadeOut(_toast, ANIMATION_DURATION_MS);
            }).ExecuteLater((long)SHOW_DURATION_MS);

            Debug.Log($"[PurchaseFeedback] {type}: {message}");
        }

        private enum FeedbackType
        {
            Success,
            Error
        }
    }
}
