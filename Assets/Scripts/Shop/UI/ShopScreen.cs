using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Common.DI;
using Common.Events;
using Common.UI;
using Shop.Data;
using Shop.Events;
using Shop.Services;

namespace Shop.UI
{
    /// <summary>
    /// Main shop screen controller. Extends UIScreenBase to integrate with the
    /// navigation system. Orchestrates all shop UI components including tabs,
    /// item grids, offer cards, wallet displays, confirmation popup, and
    /// purchase feedback.
    ///
    /// Items are loaded procedurally from a ShopCatalog ScriptableObject.
    /// Uses EventBus for decoupled communication and ServiceLocator for DI.
    /// </summary>
    public class ShopScreen : UIScreenBase
    {
        [Header("Shop Configuration")]
        [SerializeField] private ShopCatalog shopCatalog;

        // Services (resolved via ServiceLocator)
        private IShopService _shopService;
        private IWalletService _walletService;

        // UI Element references
        private Label _shopTitle;
        private Button _closeButton;

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

        // Wallet elements
        private VisualElement _moneyWalletContainer;
        private VisualElement _coinsWalletContainer;
        private Label _moneyAmountLabel;
        private Label _coinsAmountLabel;
        private Button _addMoneyBtn;
        private Button _addCoinsBtn;

        // Controllers
        private WalletDisplayController _moneyWalletController;
        private WalletDisplayController _coinsWalletController;
        private ConfirmationPopupController _confirmationPopup;
        private PurchaseFeedbackController _purchaseFeedback;
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
        /// Initialize service dependencies using ServiceLocator.
        /// </summary>
        private void InitializeServices()
        {
            int initialMoney = shopCatalog != null ? shopCatalog.DefaultMoneyBalance : 1230;
            int initialCoins = shopCatalog != null ? shopCatalog.DefaultCoinsBalance : 12000;

            _walletService = new MockWalletService(initialMoney, initialCoins);
            _shopService = new MockShopService(_walletService);

            // Register services for other components to use
            ServiceLocator.Register<IWalletService>(_walletService);
            ServiceLocator.Register<IShopService>(_shopService);
        }

        protected override void InitializeUIElements()
        {
            // Header elements
            _shopTitle = QueryElement<Label>("shop-title");
            _closeButton = QueryElement<Button>("close-btn");

            // Wallet elements
            _moneyWalletContainer = QueryElement<VisualElement>("money-wallet");
            _coinsWalletContainer = QueryElement<VisualElement>("coins-wallet");
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

            // EventBus subscriptions
            EventBus.Subscribe<PurchaseConfirmedEvent>(OnPurchaseConfirmed);
            EventBus.Subscribe<PurchaseCompletedEvent>(OnPurchaseCompleted);
            EventBus.Subscribe<TabSwitchRequestedEvent>(OnTabSwitchRequested);
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

            // Clean up popup
            _confirmationPopup?.Dispose();

            // EventBus unsubscriptions
            EventBus.Unsubscribe<PurchaseConfirmedEvent>(OnPurchaseConfirmed);
            EventBus.Unsubscribe<PurchaseCompletedEvent>(OnPurchaseCompleted);
            EventBus.Unsubscribe<TabSwitchRequestedEvent>(OnTabSwitchRequested);
        }

        protected override void OnScreenEnabled()
        {
            // Initialize wallet displays
            SetupWalletDisplays();

            // Initialize confirmation popup
            SetupConfirmationPopup();

            // Initialize purchase feedback
            SetupPurchaseFeedback();

            // Load shop items procedurally
            PopulateShopItems();

            // Set initial tab with animation
            SwitchTab(ShopTabType.Offers);

            // Register with navigation controller
            UINavigationController.RegisterScreen(Screens.Shop, root);

            Debug.Log("[ShopScreen] Shop screen initialized and ready.");
        }

        protected override void OnScreenDisabled()
        {
            UINavigationController.UnregisterScreen(Screens.Shop);
            ServiceLocator.Clear();
            EventBus.Clear();
        }

        // ===== SETUP METHODS =====

        private void SetupWalletDisplays()
        {
            _moneyWalletController = new WalletDisplayController(
                _moneyWalletContainer, _moneyAmountLabel, _addMoneyBtn,
                CurrencyType.Money, _walletService);

            _coinsWalletController = new WalletDisplayController(
                _coinsWalletContainer, _coinsAmountLabel, _addCoinsBtn,
                CurrencyType.Coins, _walletService);
        }

        private void SetupConfirmationPopup()
        {
            _confirmationPopup = new ConfirmationPopupController();
            root.Add(_confirmationPopup.Root);
        }

        private void SetupPurchaseFeedback()
        {
            _purchaseFeedback = new PurchaseFeedbackController();
            root.Add(_purchaseFeedback.Root);
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

            PopulateOffers(shopCatalog.Offers);
            PopulateGridItems(shopCatalog.MoneyItems, _moneyScroll, _moneyCardControllers);
            PopulateGridItems(shopCatalog.CoinItems, _coinsScroll, _coinsCardControllers);
        }

        private void PopulateWithDefaultData()
        {
            CreateDefaultOfferCards();
            CreateDefaultGridItems(CurrencyType.Money, _moneyScroll, _moneyCardControllers, 10);
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

            int index = 0;
            foreach (var offer in offers)
            {
                var controller = new OfferCardController();
                controller.Bind(offer, OnOfferPurchaseClicked);

                // Stagger slide-in animation
                var cardRoot = controller.Root;
                cardRoot.style.opacity = 0f;
                int delay = index * 100;
                cardRoot.schedule.Execute(() =>
                {
                    UIAnimationHelper.SlideIn(cardRoot, SlideDirection.Up, 400f);
                }).ExecuteLater(delay);

                _offersScroll.Add(cardRoot);
                _offerCardControllers.Add(controller);
                index++;
            }
        }

        private void CreateDefaultOfferCards()
        {
            if (_offersScroll == null) return;

            var defaultOffers = new[]
            {
                ("star_pass", "STAR PASS", OfferType.StarPass, 19.99f),
                ("starter_pack", "STARTER PACK", OfferType.StarterPack, 19.99f),
                ("premium_pack", "PREMIUM PACK", OfferType.PremiumPack, 49.99f)
            };

            int index = 0;
            foreach (var (id, name, type, price) in defaultOffers)
            {
                var controller = new OfferCardController();
                var data = CreateDefaultOfferData(id, name, type, price);
                controller.Bind(data, OnOfferPurchaseClicked);

                // Stagger slide-in animation
                var cardRoot = controller.Root;
                cardRoot.style.opacity = 0f;
                int delay = index * 150;
                cardRoot.schedule.Execute(() =>
                {
                    UIAnimationHelper.SlideIn(cardRoot, SlideDirection.Up, 400f);
                }).ExecuteLater(delay);

                _offersScroll.Add(cardRoot);
                _offerCardControllers.Add(controller);
                index++;
            }
        }

        private OfferItemData CreateDefaultOfferData(string id, string name, OfferType type, float price)
        {
            var data = ScriptableObject.CreateInstance<OfferItemData>();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            typeof(OfferItemData).GetField("offerId", flags)?.SetValue(data, id);
            typeof(OfferItemData).GetField("offerName", flags)?.SetValue(data, name);
            typeof(OfferItemData).GetField("offerType", flags)?.SetValue(data, type);
            typeof(OfferItemData).GetField("price", flags)?.SetValue(data, price);
            typeof(OfferItemData).GetField("priceFormatted", flags)?.SetValue(data, $"R$ {price:F2}".Replace(".", ","));

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

            var gridWrapper = new VisualElement();
            gridWrapper.AddToClassList("shop-grid-wrapper");

            int index = 0;
            foreach (var item in items)
            {
                var controller = new ShopItemCardController();
                controller.Bind(item, OnItemPurchaseClicked, OnWatchAdClicked);
                gridWrapper.Add(controller.Root);
                controllers.Add(controller);
                index++;
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
            gridWrapper.AddToClassList("shop-grid-wrapper");

            // Varied amounts for more realistic look
            int[] amounts = currencyType == CurrencyType.Money
                ? new[] { 100, 500, 1000, 2500, 50, 5000, 10000, 25000, 50000, 100000 }
                : new[] { 500, 2500, 5000, 10000, 200, 25000, 50000, 100000, 250000, 500000 };

            float[] prices = { 1.99f, 4.99f, 9.99f, 19.99f, 0f, 29.99f, 49.99f, 99.99f, 149.99f, 249.99f };

            for (int i = 0; i < count; i++)
            {
                int amount = i < amounts.Length ? amounts[i] : 2500;
                float price = i < prices.Length ? prices[i] : 19.99f;
                bool isWatchAd = (i == 4);

                var itemData = CreateDefaultShopItemData(
                    $"{currencyType.ToString().ToLower()}_{i}",
                    $"{currencyType} Pack {i + 1}",
                    amount,
                    currencyType,
                    price,
                    isWatchAd
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
            typeof(ShopItemData).GetField("priceFormatted", flags)?.SetValue(data,
                isWatchAd ? "FREE" : $"R$ {price:F2}".Replace(".", ","));
            typeof(ShopItemData).GetField("isWatchAd", flags)?.SetValue(data, isWatchAd);

            return data;
        }

        // ===== TAB NAVIGATION =====

        private void SwitchTab(ShopTabType tab)
        {
            _currentTab = tab;
            UpdateTabButtonStyles(tab);

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
            {
                tabButton.AddToClassList("tab-button--active");
                UIAnimationHelper.ScaleBounce(tabButton, 1.05f, 150f);
            }
            else
            {
                tabButton.RemoveFromClassList("tab-button--active");
            }
        }

        private void SetContentVisibility(VisualElement content, bool visible)
        {
            if (content == null) return;

            content.RemoveFromClassList("shop-tab-content--active");

            if (visible)
            {
                content.AddToClassList("shop-tab-content--active");
            }
        }

        // ===== EVENT HANDLERS =====

        private void OnItemPurchaseClicked(ShopItemData item)
        {
            Debug.Log($"[ShopScreen] Purchase clicked: {item.ItemName} for {item.PriceFormatted}");
            _confirmationPopup.ShowForItem(item, false);
        }

        private void OnOfferPurchaseClicked(OfferItemData offer)
        {
            Debug.Log($"[ShopScreen] Offer purchase clicked: {offer.OfferName} for {offer.PriceFormatted}");
            _confirmationPopup.ShowForOffer(offer);
        }

        private void OnWatchAdClicked(ShopItemData item)
        {
            Debug.Log($"[ShopScreen] Watch ad clicked for: {item.ItemName}");
            _confirmationPopup.ShowForItem(item, true);
        }

        private void OnPurchaseConfirmed(PurchaseConfirmedEvent evt)
        {
            if (evt.IsOffer && evt.Offer != null)
            {
                _shopService.PurchaseOffer(evt.Offer);
            }
            else if (evt.Item != null)
            {
                if (evt.IsWatchAd)
                    _shopService.WatchAdForReward(evt.Item);
                else
                    _shopService.PurchaseItem(evt.Item);
            }
        }

        private void OnPurchaseCompleted(PurchaseCompletedEvent evt)
        {
            if (evt.Success)
            {
                _purchaseFeedback.ShowSuccess(evt.Message);
            }
            else
            {
                _purchaseFeedback.ShowError(evt.Message);
            }
        }

        private void OnTabSwitchRequested(TabSwitchRequestedEvent evt)
        {
            SwitchTab(evt.Tab);
        }

        private void OnCloseClicked()
        {
            Debug.Log("[ShopScreen] Close button clicked. Navigating back.");
            UINavigationController.GoBack();
        }

        private void OnNavArrowClicked(ShopTabType tab)
        {
            Debug.Log($"[ShopScreen] Navigation arrow clicked on tab: {tab}");

            switch (tab)
            {
                case ShopTabType.Money:
                    if (_moneyScroll != null)
                        _moneyScroll.scrollOffset += new Vector2(0, 200);
                    break;
                case ShopTabType.Coins:
                    if (_coinsScroll != null)
                        _coinsScroll.scrollOffset += new Vector2(0, 200);
                    break;
            }
        }
    }
}
