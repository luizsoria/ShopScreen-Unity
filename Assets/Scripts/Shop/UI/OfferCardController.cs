using System;
using UnityEngine;
using UnityEngine.UIElements;
using Common.UI;
using Shop.Data;

namespace Shop.UI
{
    /// <summary>
    /// Controller for individual offer cards (Offers tab).
    /// Handles Star Pass, Starter Pack, and Premium Pack cards with
    /// countdown timers, animations, and ribbon badges.
    /// Each instance manages one offer card.
    /// </summary>
    public class OfferCardController
    {
        private readonly VisualElement _root;
        private readonly VisualElement _header;
        private readonly Label _headerText;
        private readonly VisualElement _body;
        private readonly VisualElement _imageArea;
        private readonly VisualElement _itemsList;
        private readonly Button _buyButton;
        private readonly VisualElement _ribbon;
        private readonly Label _ribbonText;

        private OfferItemData _offerData;
        private Action<OfferItemData> _onPurchaseClicked;
        private OfferTimerController _timerController;
        private IVisualElementScheduledItem _pulseAnimation;

        public VisualElement Root => _root;

        public OfferCardController()
        {
            // Build the card structure programmatically
            _root = new VisualElement();
            _root.name = "offer-card";
            _root.AddToClassList("offer-card");

            // Ribbon badge (e.g., "BEST VALUE", "LIMITED")
            _ribbon = new VisualElement();
            _ribbon.AddToClassList("offer-card__ribbon");
            _ribbon.style.display = DisplayStyle.None;

            _ribbonText = new Label("BEST VALUE");
            _ribbonText.AddToClassList("offer-card__ribbon-text");
            _ribbon.Add(_ribbonText);
            _root.Add(_ribbon);

            // Header
            _header = new VisualElement();
            _header.AddToClassList("offer-card__header");

            _headerText = new Label("OFFER");
            _headerText.AddToClassList("offer-card__header-text");
            _header.Add(_headerText);
            _root.Add(_header);

            // Body
            _body = new VisualElement();
            _body.AddToClassList("offer-card__body");

            _imageArea = new VisualElement();
            _imageArea.AddToClassList("offer-card__image");
            _body.Add(_imageArea);

            _itemsList = new VisualElement();
            _itemsList.AddToClassList("offer-card__items-list");
            _itemsList.style.display = DisplayStyle.None;
            _body.Add(_itemsList);

            _root.Add(_body);

            // Footer with timer + buy button
            var footer = new VisualElement();
            footer.AddToClassList("offer-card__footer");

            _buyButton = new Button();
            _buyButton.AddToClassList("offer-card__buy-button");
            footer.Add(_buyButton);

            _root.Add(footer);
        }

        /// <summary>
        /// Bind offer data to this card and set up event handlers.
        /// </summary>
        public void Bind(OfferItemData data, Action<OfferItemData> onPurchase)
        {
            _offerData = data;
            _onPurchaseClicked = onPurchase;

            // Set header text and color
            _headerText.text = data.OfferName.ToUpper();
            ApplyHeaderStyle(data.OfferType);

            // Set content based on offer type
            if (data.OfferType == OfferType.StarPass)
            {
                _imageArea.style.display = DisplayStyle.Flex;
                _imageArea.AddToClassList("offer-card__image--starpass");
                _itemsList.style.display = DisplayStyle.None;
            }
            else
            {
                _imageArea.style.display = DisplayStyle.None;
                _itemsList.style.display = DisplayStyle.Flex;
                PopulateItemsList(data.RewardItems);
            }

            // Set buy button
            _buyButton.text = data.PriceFormatted;
            _buyButton.clicked += OnPurchaseClick;

            // Show ribbon for premium pack
            if (data.OfferType == OfferType.PremiumPack)
            {
                _ribbon.style.display = DisplayStyle.Flex;
                _ribbonText.text = "BEST VALUE";
                _ribbon.AddToClassList("offer-card__ribbon--premium");
            }
            else if (data.OfferType == OfferType.StarterPack)
            {
                _ribbon.style.display = DisplayStyle.Flex;
                _ribbonText.text = "POPULAR";
                _ribbon.AddToClassList("offer-card__ribbon--starter");
            }

            // Add countdown timer
            AddTimer(data.OfferType);

            // Add pulse animation to buy button for attention
            _pulseAnimation = UIAnimationHelper.Pulse(_buyButton, 0.98f, 1.02f, 2000f);
        }

        /// <summary>
        /// Clean up event handlers and animations.
        /// </summary>
        public void Unbind()
        {
            _buyButton.clicked -= OnPurchaseClick;
            _pulseAnimation?.Pause();
            _timerController?.StopTimer();
        }

        private void ApplyHeaderStyle(OfferType offerType)
        {
            switch (offerType)
            {
                case OfferType.StarPass:
                    _header.AddToClassList("offer-card__header--starpass");
                    break;
                case OfferType.StarterPack:
                    _header.AddToClassList("offer-card__header--starter");
                    break;
                case OfferType.PremiumPack:
                    _header.AddToClassList("offer-card__header--premium");
                    break;
            }
        }

        private void AddTimer(OfferType offerType)
        {
            _timerController = new OfferTimerController();

            // Different durations based on offer type
            TimeSpan duration;
            switch (offerType)
            {
                case OfferType.StarPass:
                    duration = TimeSpan.FromDays(7);
                    break;
                case OfferType.StarterPack:
                    duration = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));
                    break;
                case OfferType.PremiumPack:
                    duration = TimeSpan.FromDays(3);
                    break;
                default:
                    duration = TimeSpan.FromDays(1);
                    break;
            }

            _timerController.StartTimer(duration, () =>
            {
                Debug.Log($"[OfferCard] Offer expired: {_offerData?.OfferName}");
                _buyButton.SetEnabled(false);
                _buyButton.text = "EXPIRED";
            });

            // Insert timer before footer
            var footer = _root.Q(className: "offer-card__footer");
            if (footer != null)
            {
                int footerIndex = _root.IndexOf(footer);
                _root.Insert(footerIndex, _timerController.Root);
            }
            else
            {
                _root.Add(_timerController.Root);
            }
        }

        private void PopulateItemsList(OfferRewardItem[] rewards)
        {
            _itemsList.Clear();

            if (rewards == null) return;

            foreach (var reward in rewards)
            {
                var row = new VisualElement();
                row.AddToClassList("offer-card__item-row");

                var icon = new VisualElement();
                icon.AddToClassList("offer-card__item-icon");
                if (reward.Icon != null)
                {
                    icon.style.backgroundImage = new StyleBackground(reward.Icon);
                }
                row.Add(icon);

                var text = new Label(reward.FormattedText);
                text.AddToClassList("offer-card__item-text");

                if (!string.IsNullOrEmpty(reward.Description))
                {
                    text.text = $"{reward.FormattedText}  {reward.Description}";
                }

                row.Add(text);
                _itemsList.Add(row);
            }
        }

        private void OnPurchaseClick()
        {
            // Bounce animation on click
            UIAnimationHelper.ScaleBounce(_root, 0.95f, 100f);
            _onPurchaseClicked?.Invoke(_offerData);
        }
    }
}
