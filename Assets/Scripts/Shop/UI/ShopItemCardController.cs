using System;
using UnityEngine;
using UnityEngine.UIElements;
using Common.UI;
using Shop.Data;

namespace Shop.UI
{
    /// <summary>
    /// Controller for individual shop item cards (Money/Coins tabs).
    /// Responsible for setting up the visual elements, handling click events,
    /// and providing visual feedback on interactions.
    /// Each instance manages one card in the grid.
    /// </summary>
    public class ShopItemCardController
    {
        private readonly VisualElement _root;
        private readonly VisualElement _cardImage;
        private readonly VisualElement _currencyIcon;
        private readonly Label _amountText;
        private readonly Button _buyButton;
        private readonly VisualElement _watchIcon;

        private ShopItemData _itemData;
        private Action<ShopItemData> _onPurchaseClicked;
        private Action<ShopItemData> _onWatchAdClicked;

        public VisualElement Root => _root;

        public ShopItemCardController()
        {
            // Build the card structure programmatically
            _root = new VisualElement();
            _root.name = "shop-item-card";
            _root.AddToClassList("shop-item-card");

            // Image area
            _cardImage = new VisualElement();
            _cardImage.name = "card-image";
            _cardImage.AddToClassList("shop-item-card__image");
            _root.Add(_cardImage);

            // Info area
            var infoContainer = new VisualElement();
            infoContainer.AddToClassList("shop-item-card__info");

            // Amount row
            var amountRow = new VisualElement();
            amountRow.AddToClassList("shop-item-card__amount-row");

            _currencyIcon = new VisualElement();
            _currencyIcon.AddToClassList("shop-item-card__currency-icon");
            amountRow.Add(_currencyIcon);

            _amountText = new Label("0");
            _amountText.AddToClassList("shop-item-card__amount-text");
            amountRow.Add(_amountText);

            infoContainer.Add(amountRow);

            // Buy button
            _buyButton = new Button();
            _buyButton.name = "buy-button";
            _buyButton.AddToClassList("shop-item-card__buy-button");

            // Watch icon (for ad-based items)
            _watchIcon = new VisualElement();
            _watchIcon.AddToClassList("shop-item-card__watch-icon");
            _watchIcon.style.display = DisplayStyle.None;

            infoContainer.Add(_buyButton);
            _root.Add(infoContainer);
        }

        /// <summary>
        /// Bind data to this card and set up event handlers.
        /// </summary>
        public void Bind(ShopItemData data, Action<ShopItemData> onPurchase, Action<ShopItemData> onWatchAd)
        {
            _itemData = data;
            _onPurchaseClicked = onPurchase;
            _onWatchAdClicked = onWatchAd;

            // Clear any previous type-specific classes
            _cardImage.RemoveFromClassList("shop-item-card__image--money");
            _cardImage.RemoveFromClassList("shop-item-card__image--coins");
            _currencyIcon.RemoveFromClassList("shop-item-card__currency-icon--money");
            _currencyIcon.RemoveFromClassList("shop-item-card__currency-icon--coins");
            _buyButton.RemoveFromClassList("shop-item-card__buy-button--watch");

            // Set currency-specific styles
            if (data.CurrencyType == CurrencyType.Money)
            {
                _cardImage.AddToClassList("shop-item-card__image--money");
                _currencyIcon.AddToClassList("shop-item-card__currency-icon--money");
            }
            else
            {
                _cardImage.AddToClassList("shop-item-card__image--coins");
                _currencyIcon.AddToClassList("shop-item-card__currency-icon--coins");
            }

            // Set amount text
            _amountText.text = FormatNumber(data.Amount);

            // Set button text and behavior
            if (data.IsWatchAd)
            {
                _buyButton.text = "WATCH AD";
                _buyButton.AddToClassList("shop-item-card__buy-button--watch");
                _watchIcon.style.display = DisplayStyle.Flex;
                _buyButton.clicked += OnWatchAdClick;
            }
            else
            {
                _buyButton.text = data.PriceFormatted;
                _buyButton.clicked += OnPurchaseClick;
            }

            // Set icon from data if available
            if (data.Icon != null)
            {
                _cardImage.style.backgroundImage = new StyleBackground(data.Icon);
            }
        }

        /// <summary>
        /// Clean up event handlers.
        /// </summary>
        public void Unbind()
        {
            _buyButton.clicked -= OnPurchaseClick;
            _buyButton.clicked -= OnWatchAdClick;
        }

        private void OnPurchaseClick()
        {
            // Visual feedback
            UIAnimationHelper.ScaleBounce(_root, 0.95f, 100f);
            _onPurchaseClicked?.Invoke(_itemData);
        }

        private void OnWatchAdClick()
        {
            UIAnimationHelper.ScaleBounce(_root, 0.95f, 100f);
            _onWatchAdClicked?.Invoke(_itemData);
        }

        private string FormatNumber(int number)
        {
            return number.ToString("N0").Replace(",", ".");
        }
    }
}
