using Shop.Data;

namespace Shop.Services
{
    /// <summary>
    /// Interface for shop purchase operations.
    /// Follows Interface Segregation Principle (ISP) - only purchase-related methods.
    /// </summary>
    public interface IShopService
    {
        /// <summary>
        /// Attempt to purchase a shop item (money or coins).
        /// </summary>
        /// <param name="item">The item data to purchase.</param>
        /// <returns>True if purchase was successful, false otherwise.</returns>
        bool PurchaseItem(ShopItemData item);

        /// <summary>
        /// Attempt to purchase a special offer.
        /// </summary>
        /// <param name="offer">The offer data to purchase.</param>
        /// <returns>True if purchase was successful, false otherwise.</returns>
        bool PurchaseOffer(OfferItemData offer);

        /// <summary>
        /// Simulate watching an ad for a free reward.
        /// </summary>
        /// <param name="item">The item to receive after watching.</param>
        /// <returns>True if the ad was watched successfully.</returns>
        bool WatchAdForReward(ShopItemData item);
    }
}
