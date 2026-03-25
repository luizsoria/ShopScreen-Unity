#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Shop.Data;

namespace Shop.Editor
{
    /// <summary>
    /// Editor utility to create a default ShopCatalog with sample data.
    /// Access via menu: Tools > Shop > Create Default Catalog.
    /// Generates varied, realistic shop items for demonstration.
    /// </summary>
    public static class DefaultShopCatalogCreator
    {
        [MenuItem("Tools/Shop/Create Default Catalog")]
        public static void CreateDefaultCatalog()
        {
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/ShopData"))
                AssetDatabase.CreateFolder("Assets/Resources", "ShopData");

            // Create offer items
            var starPass = CreateOfferItem("StarPass", "STAR PASS", OfferType.StarPass, 19.99f);
            var starterPack = CreateOfferItem("StarterPack", "STARTER PACK", OfferType.StarterPack, 9.99f);
            var premiumPack = CreateOfferItem("PremiumPack", "PREMIUM PACK", OfferType.PremiumPack, 49.99f);

            // Create money items with varied amounts and prices
            int[] moneyAmounts = { 100, 500, 1000, 2500, 50, 5000, 10000, 25000, 50000, 100000 };
            float[] moneyPrices = { 1.99f, 4.99f, 9.99f, 19.99f, 0f, 29.99f, 49.99f, 99.99f, 149.99f, 249.99f };
            string[] moneyNames = {
                "Pocket Money", "Small Stack", "Money Bag", "Treasure Chest", "Free Money",
                "Gold Vault", "Money Mountain", "Fortune", "Jackpot", "Money Empire"
            };

            var moneyItems = new ShopItemData[10];
            for (int i = 0; i < 10; i++)
            {
                bool isWatchAd = (i == 4);
                moneyItems[i] = CreateShopItem(
                    $"MoneyPack_{i + 1}",
                    moneyNames[i],
                    moneyAmounts[i],
                    CurrencyType.Money,
                    moneyPrices[i],
                    isWatchAd
                );
            }

            // Create coin items with varied amounts and prices
            int[] coinAmounts = { 500, 2500, 5000, 10000, 200, 25000, 50000, 100000, 250000, 500000 };
            float[] coinPrices = { 1.99f, 4.99f, 9.99f, 19.99f, 0f, 29.99f, 49.99f, 99.99f, 149.99f, 249.99f };
            string[] coinNames = {
                "Coin Pouch", "Coin Sack", "Coin Chest", "Coin Hoard", "Free Coins",
                "Coin Treasury", "Coin Vault", "Coin Mountain", "Coin Kingdom", "Coin Empire"
            };

            var coinItems = new ShopItemData[10];
            for (int i = 0; i < 10; i++)
            {
                bool isWatchAd = (i == 4);
                coinItems[i] = CreateShopItem(
                    $"CoinPack_{i + 1}",
                    coinNames[i],
                    coinAmounts[i],
                    CurrencyType.Coins,
                    coinPrices[i],
                    isWatchAd
                );
            }

            // Create the catalog
            var catalog = ScriptableObject.CreateInstance<ShopCatalog>();

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(ShopCatalog).GetField("offers", flags)?.SetValue(catalog,
                new OfferItemData[] { starPass, starterPack, premiumPack });
            typeof(ShopCatalog).GetField("moneyItems", flags)?.SetValue(catalog, moneyItems);
            typeof(ShopCatalog).GetField("coinItems", flags)?.SetValue(catalog, coinItems);
            typeof(ShopCatalog).GetField("defaultMoneyBalance", flags)?.SetValue(catalog, 1230);
            typeof(ShopCatalog).GetField("defaultCoinsBalance", flags)?.SetValue(catalog, 12000);

            AssetDatabase.CreateAsset(catalog, "Assets/Resources/ShopData/ShopCatalog.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[DefaultShopCatalogCreator] Default ShopCatalog created at " +
                      "Assets/Resources/ShopData/ShopCatalog.asset");

            Selection.activeObject = catalog;
        }

        private static OfferItemData CreateOfferItem(string id, string name, OfferType type, float price)
        {
            var item = ScriptableObject.CreateInstance<OfferItemData>();

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(OfferItemData).GetField("offerId", flags)?.SetValue(item, id);
            typeof(OfferItemData).GetField("offerName", flags)?.SetValue(item, name);
            typeof(OfferItemData).GetField("offerType", flags)?.SetValue(item, type);
            typeof(OfferItemData).GetField("price", flags)?.SetValue(item, price);
            typeof(OfferItemData).GetField("priceFormatted", flags)?.SetValue(item, $"R$ {price:F2}".Replace(".", ","));

            string assetPath = $"Assets/Resources/ShopData/{id}.asset";
            AssetDatabase.CreateAsset(item, assetPath);

            return item;
        }

        private static ShopItemData CreateShopItem(
            string id, string name, int amount,
            CurrencyType currencyType, float price, bool isWatchAd)
        {
            var item = ScriptableObject.CreateInstance<ShopItemData>();

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(ShopItemData).GetField("itemId", flags)?.SetValue(item, id);
            typeof(ShopItemData).GetField("itemName", flags)?.SetValue(item, name);
            typeof(ShopItemData).GetField("amount", flags)?.SetValue(item, amount);
            typeof(ShopItemData).GetField("currencyType", flags)?.SetValue(item, currencyType);
            typeof(ShopItemData).GetField("price", flags)?.SetValue(item, price);
            typeof(ShopItemData).GetField("priceFormatted", flags)?.SetValue(item,
                isWatchAd ? "FREE" : $"R$ {price:F2}".Replace(".", ","));
            typeof(ShopItemData).GetField("isWatchAd", flags)?.SetValue(item, isWatchAd);

            string assetPath = $"Assets/Resources/ShopData/{id}.asset";
            AssetDatabase.CreateAsset(item, assetPath);

            return item;
        }
    }
}
#endif
