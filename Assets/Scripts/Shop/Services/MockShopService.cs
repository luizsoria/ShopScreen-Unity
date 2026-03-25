using UnityEngine;
using Common.Events;
using Shop.Data;
using Shop.Events;

namespace Shop.Services
{
    /// <summary>
    /// Mock implementation of IShopService for testing purposes.
    /// All purchases are simulated with Debug.Log messages and EventBus notifications.
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

            // Simulate IAP processing delay would go here in production
            // For mock, immediately succeed
            _walletService.AddBalance(item.CurrencyType, item.Amount);

            string currencyName = item.CurrencyType == CurrencyType.Money ? "Money" : "Coins";
            Debug.Log($"[MockShopService] Purchase successful! " +
                      $"Added {item.Amount} {item.CurrencyType} to wallet.");

            EventBus.Publish(new PurchaseCompletedEvent
            {
                Success = true,
                ItemName = item.ItemName,
                Message = $"+{item.Amount:N0} {currencyName}!".Replace(",", "."),
                CurrencyType = item.CurrencyType,
                AmountAdded = item.Amount
            });

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

            EventBus.Publish(new PurchaseCompletedEvent
            {
                Success = true,
                ItemName = offer.OfferName,
                Message = $"{offer.OfferName} purchased successfully!",
                CurrencyType = null,
                AmountAdded = 0
            });

            return true;
        }

        public bool WatchAdForReward(ShopItemData item)
        {
            Debug.Log($"[MockShopService] Watch ad requested for: {item.ItemName} " +
                      $"({item.Amount} {item.CurrencyType})");

            // Simulate ad watched successfully
            _walletService.AddBalance(item.CurrencyType, item.Amount);

            string currencyName = item.CurrencyType == CurrencyType.Money ? "Money" : "Coins";
            Debug.Log($"[MockShopService] Ad watched! " +
                      $"Rewarded {item.Amount} {item.CurrencyType}.");

            EventBus.Publish(new PurchaseCompletedEvent
            {
                Success = true,
                ItemName = item.ItemName,
                Message = $"Ad reward: +{item.Amount:N0} {currencyName}!".Replace(",", "."),
                CurrencyType = item.CurrencyType,
                AmountAdded = item.Amount
            });

            return true;
        }
    }
}
