using Shop.Data;

namespace Shop.Events
{
    /// <summary>
    /// Event fired when a shop item purchase is requested.
    /// </summary>
    public struct ShopItemPurchaseRequestedEvent
    {
        public ShopItemData Item;
        public bool IsWatchAd;
    }

    /// <summary>
    /// Event fired when an offer purchase is requested.
    /// </summary>
    public struct OfferPurchaseRequestedEvent
    {
        public OfferItemData Offer;
    }

    /// <summary>
    /// Event fired when a purchase is completed (success or failure).
    /// </summary>
    public struct PurchaseCompletedEvent
    {
        public bool Success;
        public string ItemName;
        public string Message;
        public CurrencyType? CurrencyType;
        public int AmountAdded;
    }

    /// <summary>
    /// Event fired when the user confirms a purchase in the confirmation popup.
    /// </summary>
    public struct PurchaseConfirmedEvent
    {
        public ShopItemData Item;
        public OfferItemData Offer;
        public bool IsOffer;
        public bool IsWatchAd;
    }

    /// <summary>
    /// Event fired when the user cancels a purchase in the confirmation popup.
    /// </summary>
    public struct PurchaseCancelledEvent { }

    /// <summary>
    /// Event fired when a tab switch is requested.
    /// </summary>
    public struct TabSwitchRequestedEvent
    {
        public ShopTabType Tab;
    }

    /// <summary>
    /// Event fired when the shop close is requested.
    /// </summary>
    public struct ShopCloseRequestedEvent { }

    /// <summary>
    /// Event fired when wallet balance changes.
    /// </summary>
    public struct WalletBalanceChangedEvent
    {
        public CurrencyType CurrencyType;
        public int NewBalance;
        public int PreviousBalance;
    }
}
