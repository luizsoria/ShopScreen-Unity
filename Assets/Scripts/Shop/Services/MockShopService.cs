using UnityEngine;
using Shop.Data;

namespace Shop.Services
{
    /// <summary>
    /// Mock implementation of IShopService for testing purposes.
    /// All purchases are simulated with Debug.Log messages.
    /// In a real implementation, this would integrate with platform-specific IAP systems.
    /// </summary>
    public class MockShopService : IShopService
    {
        private readonly IWalletService _walletService;

        public MockShopService(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public bool PurchaseItem(ShopItemData item)
        {
            Debug.Log($"[MockShopService] Purchase requested: {item.ItemName} " +
                      $"({item.Amount} {item.CurrencyType}) for {item.PriceFormatted}");

            // Simulate successful purchase
            _walletService.AddBalance(item.CurrencyType, item.Amount);

            Debug.Log($"[MockShopService] Purchase successful! " +
                      $"Added {item.Amount} {item.CurrencyType} to wallet. " +
                      $"New balance: {_walletService.GetBalance(item.CurrencyType)}");

            return true;
        }

        public bool PurchaseOffer(OfferItemData offer)
        {
            Debug.Log($"[MockShopService] Offer purchase requested: {offer.OfferName} " +
                      $"({offer.OfferType}) for {offer.PriceFormatted}");

            if (offer.RewardItems != null)
            {
                foreach (var reward in offer.RewardItems)
                {
                    Debug.Log($"[MockShopService] Reward: {reward.FormattedText} {reward.Description}");
                }
            }

            Debug.Log($"[MockShopService] Offer purchase successful: {offer.OfferName}");
            return true;
        }

        public bool WatchAdForReward(ShopItemData item)
        {
            Debug.Log($"[MockShopService] Watch ad requested for: {item.ItemName} " +
                      $"({item.Amount} {item.CurrencyType})");

            // Simulate ad watched successfully
            _walletService.AddBalance(item.CurrencyType, item.Amount);

            Debug.Log($"[MockShopService] Ad watched! " +
                      $"Rewarded {item.Amount} {item.CurrencyType}. " +
                      $"New balance: {_walletService.GetBalance(item.CurrencyType)}");

            return true;
        }
    }
}
