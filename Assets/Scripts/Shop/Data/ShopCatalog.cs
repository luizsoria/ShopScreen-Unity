using UnityEngine;

namespace Shop.Data
{
    /// <summary>
    /// Central catalog that holds all shop data organized by category.
    /// This is the main data source for the shop screen.
    /// </summary>
    [CreateAssetMenu(fileName = "ShopCatalog", menuName = "Shop/Shop Catalog")]
    public class ShopCatalog : ScriptableObject
    {
        [Header("Offers")]
        [SerializeField] private OfferItemData[] offers;

        [Header("Money Items")]
        [SerializeField] private ShopItemData[] moneyItems;

        [Header("Coins Items")]
        [SerializeField] private ShopItemData[] coinItems;

        [Header("Wallet Defaults")]
        [SerializeField] private int defaultMoneyBalance = 1230;
        [SerializeField] private int defaultCoinsBalance = 12000;

        public OfferItemData[] Offers => offers;
        public ShopItemData[] MoneyItems => moneyItems;
        public ShopItemData[] CoinItems => coinItems;
        public int DefaultMoneyBalance => defaultMoneyBalance;
        public int DefaultCoinsBalance => defaultCoinsBalance;
    }
}
