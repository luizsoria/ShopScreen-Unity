using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Common.UI;
using Shop.Data;
using Shop.Services;

namespace Shop.UI
{
    /// <summary>
    /// Main shop screen controller. Extends UIScreenBase to integrate with the
    /// navigation system. Orchestrates all shop UI components including tabs,
    /// item grids, offer cards, and wallet displays.
    ///
    /// Items are loaded procedurally from a ShopCatalog ScriptableObject.
    /// </summary>
    public class ShopScreen : UIScreenBase
    {
        [Header("Shop Configuration")]
        [SerializeField] private ShopCatalog shopCatalog;

        // Services (Dependency Inversion Principle)
        private IShopService _shopService;
        private IWalletService _walletService;

        // UI Element references
        private Label _shopTitle;
        private Button _closeButton;
        private VisualElement _shopSidebar;
        private VisualElement _shopContent;

        // Tab buttons
        private Button _tabOffers;
        private Button _tabMoney;
        private Button _tabCoins;

        // Tab content panels
        private VisualElement _offersContent;
        private VisualElement _moneyContent;
        private VisualElement _coinsContent;

        // Scroll containers
        private VisualElement _offersScroll;
        private ScrollView _moneyScroll;
        private ScrollView _coinsScroll;

        // Navigation arrows
        private Button _offersNavArrow;
        private Button _moneyNavArrow;
        private Button _coinsNavArrow;

        // Wallet displays
        private Label _moneyAmountLabel;
        private Label _coinsAmountLabel;
        private Button _addMoneyBtn;
        private Button _addCoinsBtn;

        // Controllers
        private WalletDisplayController _moneyWalletController;
        private WalletDisplayController _coinsWalletController;
        private readonly List<ShopItemCardController> _moneyCardControllers = new List<ShopItemCardController>();
        private readonly List<ShopItemCardController> _coinsCardControllers = new List<ShopItemCardController>();
        private readonly List<OfferCardController> _offerCardControllers = new List<OfferCardController>();

        // State
        private ShopTabType _currentTab = ShopTabType.Offers;

        protected override void Awake()
        {
            base.Awake();
            InitializeServices();
        }

        /// <summary>
        /// Initialize service dependencies.
        /// In a production app, these would be injected via a DI container.
        /// </summary>
        private void InitializeServices()
        {
            int initialMoney = shopCatalog != null ? shopCatalog.DefaultMoneyBalance : 1230;
            int initialCoins = shopCatalog != null ? shopCatalog.DefaultCoinsBalance : 12000;

            _walletService = new MockWalletService(initialMoney, initialCoins);
            _shopService = new MockShopService(_walletService);
        }

        protected override void InitializeUIElements()
        {
            // Header elements
            _shopTitle = QueryElement<Label>("shop-title");
            _closeButton = QueryElement<Button>("close-btn");

            // Wallet elements
            _moneyAmountLabel = QueryElement<Label>("money-amount");
            _coinsAmountLabel = QueryElement<Label>("coins-amount");
            _addMoneyBtn = QueryElement<Button>("add-money-btn");
            _addCoinsBtn = QueryElement<Button>("add-coins-btn");

            // Tab buttons
            _tabOffers = QueryElement<Button>("tab-offers");
            _tabMoney = QueryElement<Button>("tab-money");
            _tabCoins = QueryElement<Button>("tab-coins");

            // Content panels
            _offersContent = QueryElement<VisualElement>("offers-content");
            _moneyContent = QueryElement<VisualElement>("money-content");
            _coinsContent = QueryElement<VisualElement>("coins-content");

            // Scroll containers
            _offersScroll = QueryElement<VisualElement>("offers-scroll");
            _moneyScroll = QueryElement<ScrollView>("money-scroll");
            _coinsScroll = QueryElement<ScrollView>("coins-scroll");

            // Navigation arrows
            _offersNavArrow = QueryElement<Button>("offers-nav-arrow");
            _moneyNavArrow = QueryElement<Button>("money-nav-arrow");
            _coinsNavArrow = QueryElement<Button>("coins-nav-arrow");
        }

        protected override void RegisterCallbacks()
        {
            // Tab navigation
            RegisterButtonCallback(_tabOffers, () => SwitchTab(ShopTabType.Offers));
            RegisterButtonCallback(_tabMoney, () => SwitchTab(ShopTabType.Money));
            RegisterButtonCallback(_tabCoins, () => SwitchTab(ShopTabType.Coins));

            // Close button
            RegisterButtonCallback(_closeButton, OnCloseClicked);

            // Navigation arrows
            RegisterButtonCallback(_offersNavArrow, () => OnNavArrowClicked(ShopTabType.Offers));
            RegisterButtonCallback(_moneyNavArrow, () => OnNavArrowClicked(ShopTabType.Money));
            RegisterButtonCallback(_coinsNavArrow, () => OnNavArrowClicked(ShopTabType.Coins));
        }

        protected override void UnregisterCallbacks()
        {
            // Clean up card controllers
            foreach (var controller in _moneyCardControllers)
                controller.Unbind();
            foreach (var controller in _coinsCardControllers)
                controller.Unbind();
            foreach (var controller in _offerCardControllers)
                controller.Unbind();

            // Clean up wallet controllers
            _moneyWalletController?.Dispose();
            _coinsWalletController?.Dispose();
        }

        protected override void OnScreenEnabled()
        {
            // Initialize wallet displays
            SetupWalletDisplays();

            // Load shop items procedurally
            PopulateShopItems();

            // Set initial tab
            SwitchTab(ShopTabType.Offers);

            // Register with navigation controller
            UINavigationController.RegisterScreen(Screens.Shop, root);

            Debug.Log("[ShopScreen] Shop screen initialized and ready.");
        }

        protected override void OnScreenDisabled()
        {
            UINavigationController.UnregisterScreen(Screens.Shop);
        }

        // ===== WALLET SETUP =====

        private void SetupWalletDisplays()
        {
            _moneyWalletController = new WalletDisplayController(
                _moneyAmountLabel, _addMoneyBtn, CurrencyType.Money, _walletService);

            _coinsWalletController = new WalletDisplayController(
                _coinsAmountLabel, _addCoinsBtn, CurrencyType.Coins, _walletService);
        }

        // ===== PROCEDURAL ITEM LOADING =====

        private void PopulateShopItems()
        {
            if (shopCatalog == null)
            {
                Debug.LogWarning("[ShopScreen] ShopCatalog is not assigned. Using default data.");
                PopulateWithDefaultData();
                return;
            }

            // Populate Offers
            PopulateOffers(shopCatalog.Offers);

            // Populate Money items
            PopulateGridItems(shopCatalog.MoneyItems, _moneyScroll, _moneyCardControllers);

            // Populate Coins items
            PopulateGridItems(shopCatalog.CoinItems, _coinsScroll, _coinsCardControllers);
        }

        private void PopulateWithDefaultData()
        {
            // Create default offer cards
            CreateDefaultOfferCards();

            // Create default money items
            CreateDefaultGridItems(CurrencyType.Money, _moneyScroll, _moneyCardControllers, 10);

            // Create default coins items
            CreateDefaultGridItems(CurrencyType.Coins, _coinsScroll, _coinsCardControllers, 10);
        }

        private void PopulateOffers(OfferItemData[] offers)
        {
            if (_offersScroll == null) return;
            _offersScroll.Clear();

            if (offers == null || offers.Length == 0)
            {
                CreateDefaultOfferCards();
                return;
            }

            foreach (var offer in offers)
            {
                var controller = new OfferCardController();
                controller.Bind(offer, OnOfferPurchaseClicked);
                _offersScroll.Add(controller.Root);
                _offerCardControllers.Add(controller);
            }
        }

        private void CreateDefaultOfferCards()
        {
            if (_offersScroll == null) return;

            // Star Pass
            var starPassController = new OfferCardController();
            var starPassData = CreateDefaultOfferData("star_pass", "STAR PASS", OfferType.StarPass, 19.99f);
            starPassController.Bind(starPassData, OnOfferPurchaseClicked);
            _offersScroll.Add(starPassController.Root);
            _offerCardControllers.Add(starPassController);

            // Starter Pack
            var starterController = new OfferCardController();
            var starterData = CreateDefaultOfferData("starter_pack", "STARTER PACK", OfferType.StarterPack, 19.99f);
            starterController.Bind(starterData, OnOfferPurchaseClicked);
            _offersScroll.Add(starterController.Root);
            _offerCardControllers.Add(starterController);

            // Premium Pack
            var premiumController = new OfferCardController();
            var premiumData = CreateDefaultOfferData("premium_pack", "PREMIUM PACK", OfferType.PremiumPack, 19.99f);
            premiumController.Bind(premiumData, OnOfferPurchaseClicked);
            _offersScroll.Add(premiumController.Root);
            _offerCardControllers.Add(premiumController);
        }

        private OfferItemData CreateDefaultOfferData(string id, string name, OfferType type, float price)
        {
            var data = ScriptableObject.CreateInstance<OfferItemData>();

            // Use reflection to set private serialized fields for runtime-created instances
            var idField = typeof(OfferItemData).GetField("offerId",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var nameField = typeof(OfferItemData).GetField("offerName",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var typeField = typeof(OfferItemData).GetField("offerType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var priceField = typeof(OfferItemData).GetField("price",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var priceFormattedField = typeof(OfferItemData).GetField("priceFormatted",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            idField?.SetValue(data, id);
            nameField?.SetValue(data, name);
            typeField?.SetValue(data, type);
            priceField?.SetValue(data, price);
            priceFormattedField?.SetValue(data, $"R$ {price:F2}".Replace(".", ","));

            return data;
        }

        private void PopulateGridItems(
            ShopItemData[] items,
            ScrollView scrollContainer,
            List<ShopItemCardController> controllers)
        {
            if (scrollContainer == null) return;

            var contentContainer = scrollContainer.contentContainer;
            contentContainer.Clear();

            if (items == null || items.Length == 0) return;

            // Create a wrapper for the grid layout
            var gridWrapper = new VisualElement();
            gridWrapper.style.flexDirection = FlexDirection.Row;
            gridWrapper.style.flexWrap = Wrap.Wrap;
            gridWrapper.style.alignContent = Align.FlexStart;
            gridWrapper.style.flexGrow = 1;

            foreach (var item in items)
            {
                var controller = new ShopItemCardController();
                controller.Bind(item, OnItemPurchaseClicked, OnWatchAdClicked);
                gridWrapper.Add(controller.Root);
                controllers.Add(controller);
            }

            contentContainer.Add(gridWrapper);
        }

        private void CreateDefaultGridItems(
            CurrencyType currencyType,
            ScrollView scrollContainer,
            List<ShopItemCardController> controllers,
            int count)
        {
            if (scrollContainer == null) return;

            var contentContainer = scrollContainer.contentContainer;
            contentContainer.Clear();

            var gridWrapper = new VisualElement();
            gridWrapper.style.flexDirection = FlexDirection.Row;
            gridWrapper.style.flexWrap = Wrap.Wrap;
            gridWrapper.style.alignContent = Align.FlexStart;
            gridWrapper.style.flexGrow = 1;

            for (int i = 0; i < count; i++)
            {
                bool isLastInFirstRow = (i == 4); // 5th item is "watch ad"
                var itemData = CreateDefaultShopItemData(
                    $"{currencyType.ToString().ToLower()}_{i}",
                    $"{currencyType} Pack {i + 1}",
                    2500,
                    currencyType,
                    19.99f,
                    isLastInFirstRow
                );

                var controller = new ShopItemCardController();
                controller.Bind(itemData, OnItemPurchaseClicked, OnWatchAdClicked);
                gridWrapper.Add(controller.Root);
                controllers.Add(controller);
            }

            contentContainer.Add(gridWrapper);
        }

        private ShopItemData CreateDefaultShopItemData(
            string id, string name, int amount,
            CurrencyType currencyType, float price, bool isWatchAd)
        {
            var data = ScriptableObject.CreateInstance<ShopItemData>();

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(ShopItemData).GetField("itemId", flags)?.SetValue(data, id);
            typeof(ShopItemData).GetField("itemName", flags)?.SetValue(data, name);
            typeof(ShopItemData).GetField("amount", flags)?.SetValue(data, amount);
            typeof(ShopItemData).GetField("currencyType", flags)?.SetValue(data, currencyType);
            typeof(ShopItemData).GetField("price", flags)?.SetValue(data, price);
            typeof(ShopItemData).GetField("priceFormatted", flags)?.SetValue(data, $"R$ {price:F2}".Replace(".", ","));
            typeof(ShopItemData).GetField("isWatchAd", flags)?.SetValue(data, isWatchAd);

            return data;
        }

        // ===== TAB NAVIGATION =====

        private void SwitchTab(ShopTabType tab)
        {
            _currentTab = tab;

            // Update tab button styles
            UpdateTabButtonStyles(tab);

            // Show/hide content panels
            SetContentVisibility(_offersContent, tab == ShopTabType.Offers);
            SetContentVisibility(_moneyContent, tab == ShopTabType.Money);
            SetContentVisibility(_coinsContent, tab == ShopTabType.Coins);

            Debug.Log($"[ShopScreen] Switched to tab: {tab}");
        }

        private void UpdateTabButtonStyles(ShopTabType activeTab)
        {
            SetTabActive(_tabOffers, activeTab == ShopTabType.Offers);
            SetTabActive(_tabMoney, activeTab == ShopTabType.Money);
            SetTabActive(_tabCoins, activeTab == ShopTabType.Coins);
        }

        private void SetTabActive(Button tabButton, bool isActive)
        {
            if (tabButton == null) return;

            if (isActive)
                tabButton.AddToClassList("tab-button--active");
            else
                tabButton.RemoveFromClassList("tab-button--active");
        }

        private void SetContentVisibility(VisualElement content, bool visible)
        {
            if (content == null) return;

            if (visible)
            {
                content.RemoveFromClassList("shop-tab-content");
                content.AddToClassList("shop-tab-content");
                content.AddToClassList("shop-tab-content--active");
            }
            else
            {
                content.RemoveFromClassList("shop-tab-content--active");
            }
        }

        // ===== EVENT HANDLERS =====

        private void OnItemPurchaseClicked(ShopItemData item)
        {
            Debug.Log($"[ShopScreen] Purchase clicked: {item.ItemName} for {item.PriceFormatted}");
            _shopService.PurchaseItem(item);
        }

        private void OnOfferPurchaseClicked(OfferItemData offer)
        {
            Debug.Log($"[ShopScreen] Offer purchase clicked: {offer.OfferName} for {offer.PriceFormatted}");
            _shopService.PurchaseOffer(offer);
        }

        private void OnWatchAdClicked(ShopItemData item)
        {
            Debug.Log($"[ShopScreen] Watch ad clicked for: {item.ItemName}");
            _shopService.WatchAdForReward(item);
        }

        private void OnCloseClicked()
        {
            Debug.Log("[ShopScreen] Close button clicked. Navigating back.");
            UINavigationController.GoBack();
        }

        private void OnNavArrowClicked(ShopTabType tab)
        {
            Debug.Log($"[ShopScreen] Navigation arrow clicked on tab: {tab}");

            // Scroll the content to the right
            switch (tab)
            {
                case ShopTabType.Money:
                    if (_moneyScroll != null)
                        _moneyScroll.scrollOffset += new Vector2(300, 0);
                    break;
                case ShopTabType.Coins:
                    if (_coinsScroll != null)
                        _coinsScroll.scrollOffset += new Vector2(300, 0);
                    break;
            }
        }
    }
}
