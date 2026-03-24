using UnityEngine;

namespace Shop.Data
{
    /// <summary>
    /// Represents a single reward item within an offer pack.
    /// For example: "X2 Hammers", "X20 Coins", etc.
    /// </summary>
    [System.Serializable]
    public class OfferRewardItem
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private string description;
        [SerializeField] private int multiplier;

        public Sprite Icon => icon;
        public string Description => description;
        public int Multiplier => multiplier;
        public string FormattedText => $"X{multiplier}";
    }

    /// <summary>
    /// Represents a special offer/pack in the shop (Star Pass, Starter Pack, Premium Pack).
    /// </summary>
    [CreateAssetMenu(fileName = "NewOfferItem", menuName = "Shop/Offer Item Data")]
    public class OfferItemData : ScriptableObject
    {
        [Header("Offer Info")]
        [SerializeField] private string offerId;
        [SerializeField] private string offerName;
        [SerializeField] private OfferType offerType;

        [Header("Pricing")]
        [SerializeField] private float price;
        [SerializeField] private string priceFormatted;

        [Header("Content")]
        [SerializeField] private OfferRewardItem[] rewardItems;
        [SerializeField] private Sprite backgroundImage;

        public string OfferId => offerId;
        public string OfferName => offerName;
        public OfferType OfferType => offerType;
        public float Price => price;
        public string PriceFormatted => string.IsNullOrEmpty(priceFormatted) ? $"R$ {price:F2}" : priceFormatted;
        public OfferRewardItem[] RewardItems => rewardItems;
        public Sprite BackgroundImage => backgroundImage;
    }
}
