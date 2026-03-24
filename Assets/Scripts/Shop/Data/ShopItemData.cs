using UnityEngine;

namespace Shop.Data
{
    /// <summary>
    /// Represents a single purchasable item in the shop.
    /// Used for both Money and Coins tab items.
    /// </summary>
    [CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item Data")]
    public class ShopItemData : ScriptableObject
    {
        [Header("Item Info")]
        [SerializeField] private string itemId;
        [SerializeField] private string itemName;
        [SerializeField] private int amount;
        [SerializeField] private CurrencyType currencyType;

        [Header("Pricing")]
        [SerializeField] private float price;
        [SerializeField] private string priceFormatted;
        [SerializeField] private bool isWatchAd;

        [Header("Visuals")]
        [SerializeField] private Sprite icon;

        public string ItemId => itemId;
        public string ItemName => itemName;
        public int Amount => amount;
        public CurrencyType CurrencyType => currencyType;
        public float Price => price;
        public string PriceFormatted => string.IsNullOrEmpty(priceFormatted) ? $"R$ {price:F2}" : priceFormatted;
        public bool IsWatchAd => isWatchAd;
        public Sprite Icon => icon;
    }
}
