using System;
using UnityEngine;
using UnityEngine.UIElements;
using Common.Events;
using Common.UI;
using Shop.Data;
using Shop.Events;

namespace Shop.UI
{
    /// <summary>
    /// Controller for the purchase confirmation popup.
    /// Shows item details and asks the user to confirm or cancel the purchase.
    /// Supports both regular items and special offers.
    /// </summary>
    public class ConfirmationPopupController
    {
        private readonly VisualElement _overlay;
        private readonly VisualElement _popup;
        private readonly Label _titleLabel;
        private readonly Label _descriptionLabel;
        private readonly Label _priceLabel;
        private readonly VisualElement _itemIcon;
        private readonly Button _confirmButton;
        private readonly Button _cancelButton;

        // Current pending purchase data
        private ShopItemData _pendingItem;
        private OfferItemData _pendingOffer;
        private bool _isOffer;
        private bool _isWatchAd;

        public VisualElement Root => _overlay;

        public ConfirmationPopupController()
        {
            // Overlay (darkened background)
            _overlay = new VisualElement();
            _overlay.name = "confirmation-overlay";
            _overlay.AddToClassList("confirmation-overlay");
            _overlay.style.display = DisplayStyle.None;

            // Popup container
            _popup = new VisualElement();
            _popup.name = "confirmation-popup";
            _popup.AddToClassList("confirmation-popup");

            // Title
            _titleLabel = new Label("CONFIRM PURCHASE");
            _titleLabel.AddToClassList("confirmation-popup__title");
            _popup.Add(_titleLabel);

            // Separator
            var separator = new VisualElement();
            separator.AddToClassList("confirmation-popup__separator");
            _popup.Add(separator);

            // Content area
            var content = new VisualElement();
            content.AddToClassList("confirmation-popup__content");

            // Item icon
            _itemIcon = new VisualElement();
            _itemIcon.AddToClassList("confirmation-popup__icon");
            content.Add(_itemIcon);

            // Info area
            var infoArea = new VisualElement();
            infoArea.AddToClassList("confirmation-popup__info");

            _descriptionLabel = new Label("Do you want to purchase this item?");
            _descriptionLabel.AddToClassList("confirmation-popup__description");
            infoArea.Add(_descriptionLabel);

            _priceLabel = new Label("R$ 0,00");
            _priceLabel.AddToClassList("confirmation-popup__price");
            infoArea.Add(_priceLabel);

            content.Add(infoArea);
            _popup.Add(content);

            // Buttons area
            var buttonsArea = new VisualElement();
            buttonsArea.AddToClassList("confirmation-popup__buttons");

            _cancelButton = new Button();
            _cancelButton.text = "CANCEL";
            _cancelButton.AddToClassList("confirmation-popup__button");
            _cancelButton.AddToClassList("confirmation-popup__button--cancel");
            _cancelButton.clicked += OnCancelClicked;
            buttonsArea.Add(_cancelButton);

            _confirmButton = new Button();
            _confirmButton.text = "CONFIRM";
            _confirmButton.AddToClassList("confirmation-popup__button");
            _confirmButton.AddToClassList("confirmation-popup__button--confirm");
            _confirmButton.clicked += OnConfirmClicked;
            buttonsArea.Add(_confirmButton);

            _popup.Add(buttonsArea);
            _overlay.Add(_popup);

            // Click overlay to cancel
            _overlay.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target == _overlay)
                    OnCancelClicked();
            });
        }

        /// <summary>
        /// Show the popup for a regular shop item purchase.
        /// </summary>
        public void ShowForItem(ShopItemData item, bool isWatchAd = false)
        {
            _pendingItem = item;
            _pendingOffer = null;
            _isOffer = false;
            _isWatchAd = isWatchAd;

            _titleLabel.text = isWatchAd ? "WATCH AD FOR REWARD" : "CONFIRM PURCHASE";

            string currencyName = item.CurrencyType == CurrencyType.Money ? "Money" : "Coins";
            _descriptionLabel.text = $"Get {item.Amount:N0} {currencyName}?".Replace(",", ".");

            if (isWatchAd)
            {
                _priceLabel.text = "FREE (Watch Ad)";
                _confirmButton.text = "WATCH";
            }
            else
            {
                _priceLabel.text = item.PriceFormatted;
                _confirmButton.text = "BUY NOW";
            }

            // Set icon style
            _itemIcon.ClearClassList();
            _itemIcon.AddToClassList("confirmation-popup__icon");
            if (item.CurrencyType == CurrencyType.Money)
                _itemIcon.AddToClassList("confirmation-popup__icon--money");
            else
                _itemIcon.AddToClassList("confirmation-popup__icon--coins");

            Show();
        }

        /// <summary>
        /// Show the popup for a special offer purchase.
        /// </summary>
        public void ShowForOffer(OfferItemData offer)
        {
            _pendingItem = null;
            _pendingOffer = offer;
            _isOffer = true;
            _isWatchAd = false;

            _titleLabel.text = "CONFIRM PURCHASE";
            _descriptionLabel.text = $"Purchase {offer.OfferName}?";
            _priceLabel.text = offer.PriceFormatted;
            _confirmButton.text = "BUY NOW";

            // Set icon style based on offer type
            _itemIcon.ClearClassList();
            _itemIcon.AddToClassList("confirmation-popup__icon");
            switch (offer.OfferType)
            {
                case OfferType.StarPass:
                    _itemIcon.AddToClassList("confirmation-popup__icon--starpass");
                    break;
                case OfferType.StarterPack:
                    _itemIcon.AddToClassList("confirmation-popup__icon--starter");
                    break;
                case OfferType.PremiumPack:
                    _itemIcon.AddToClassList("confirmation-popup__icon--premium");
                    break;
            }

            Show();
        }

        private void Show()
        {
            UIAnimationHelper.FadeIn(_overlay, 200f);
            UIAnimationHelper.ScaleBounce(_popup, 1.05f, 200f);
            Debug.Log("[ConfirmationPopup] Popup shown.");
        }

        public void Hide()
        {
            UIAnimationHelper.FadeOut(_overlay, 200f);
            Debug.Log("[ConfirmationPopup] Popup hidden.");
        }

        private void OnConfirmClicked()
        {
            Debug.Log("[ConfirmationPopup] Purchase confirmed!");

            EventBus.Publish(new PurchaseConfirmedEvent
            {
                Item = _pendingItem,
                Offer = _pendingOffer,
                IsOffer = _isOffer,
                IsWatchAd = _isWatchAd
            });

            Hide();
        }

        private void OnCancelClicked()
        {
            Debug.Log("[ConfirmationPopup] Purchase cancelled.");
            EventBus.Publish(new PurchaseCancelledEvent());
            Hide();
        }

        public void Dispose()
        {
            _confirmButton.clicked -= OnConfirmClicked;
            _cancelButton.clicked -= OnCancelClicked;
        }
    }
}
