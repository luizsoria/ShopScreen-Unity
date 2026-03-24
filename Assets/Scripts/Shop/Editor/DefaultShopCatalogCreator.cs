#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Shop.Data;

namespace Shop.Editor
{
    /// <summary>
    /// Editor utility to create a default ShopCatalog with sample data.
    /// Access via menu: Tools > Shop > Create Default Catalog.
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
            var starterPack = CreateOfferItem("StarterPack", "STARTER PACK", OfferType.StarterPack, 19.99f);
            var premiumPack = CreateOfferItem("PremiumPack", "PREMIUM PACK", OfferType.PremiumPack, 19.99f);

            // Create money items (10 items, last in first row is watch ad)
            var moneyItems = new ShopItemData[10];
            for (int i = 0; i < 10; i++)
            {
                bool isWatchAd = (i == 4);
                moneyItems[i] = CreateShopItem(
                    $"MoneyPack_{i + 1}",
                    $"Money Pack {i + 1}",
                    2500,
                    CurrencyType.Money,
                    19.99f,
                    isWatchAd
                );
            }

            // Create coin items (10 items, last in first row is watch ad)
            var coinItems = new ShopItemData[10];
            for (int i = 0; i < 10; i++)
            {
                bool isWatchAd = (i == 4);
                coinItems[i] = CreateShopItem(
                    $"CoinPack_{i + 1}",
                    $"Coin Pack {i + 1}",
                    2500,
                    CurrencyType.Coins,
                    19.99f,
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
            typeof(ShopItemData).GetField("priceFormatted", flags)?.SetValue(item, $"R$ {price:F2}".Replace(".", ","));
            typeof(ShopItemData).GetField("isWatchAd", flags)?.SetValue(item, isWatchAd);

            string assetPath = $"Assets/Resources/ShopData/{id}.asset";
            AssetDatabase.CreateAsset(item, assetPath);

            return item;
        }
    }
}
#endif
