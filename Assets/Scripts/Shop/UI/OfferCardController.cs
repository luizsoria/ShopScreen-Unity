using System;
using UnityEngine;
using UnityEngine.UIElements;
using Shop.Data;

namespace Shop.UI
{
    /// <summary>
    /// Controller for individual offer cards (Offers tab).
    /// Handles Star Pass, Starter Pack, and Premium Pack cards.
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

        private OfferItemData _offerData;
        private Action<OfferItemData> _onPurchaseClicked;

        public VisualElement Root => _root;

        public OfferCardController()
        {
            // Build the card structure programmatically
            _root = new VisualElement();
            _root.name = "offer-card";
            _root.AddToClassList("offer-card");

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

            // Footer
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
                // Star Pass shows an image
                _imageArea.style.display = DisplayStyle.Flex;
                _imageArea.AddToClassList("offer-card__image--starpass");
                _itemsList.style.display = DisplayStyle.None;
            }
            else
            {
                // Starter/Premium Pack shows items list
                _imageArea.style.display = DisplayStyle.None;
                _itemsList.style.display = DisplayStyle.Flex;
                PopulateItemsList(data.RewardItems);
            }

            // Set buy button
            _buyButton.text = data.PriceFormatted;
            _buyButton.clicked += OnPurchaseClick;
        }

        /// <summary>
        /// Clean up event handlers.
        /// </summary>
        public void Unbind()
        {
            _buyButton.clicked -= OnPurchaseClick;
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

                var text = new Label($"{reward.FormattedText}");
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
            _onPurchaseClicked?.Invoke(_offerData);
        }
    }
}
