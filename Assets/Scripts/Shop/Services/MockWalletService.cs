using System;
using System.Collections.Generic;
using UnityEngine;
using Common.Events;
using Shop.Data;
using Shop.Events;

namespace Shop.Services
{
    /// <summary>
    /// Mock implementation of IWalletService for testing purposes.
    /// Manages in-memory currency balances and publishes balance change events
    /// via both the legacy event and the EventBus.
    /// </summary>
    public class MockWalletService : IWalletService
    {
        public event Action<CurrencyType, int> OnBalanceChanged;

        private readonly Dictionary<CurrencyType, int> _balances;

        public MockWalletService(int initialMoney = 0, int initialCoins = 0)
        {
            _balances = new Dictionary<CurrencyType, int>
            {
                { CurrencyType.Money, initialMoney },
                { CurrencyType.Coins, initialCoins }
            };
        }

        public int GetBalance(CurrencyType currencyType)
        {
            return _balances.TryGetValue(currencyType, out int balance) ? balance : 0;
        }

        public void AddBalance(CurrencyType currencyType, int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[MockWalletService] Cannot add non-positive amount: {amount}");
                return;
            }

            if (!_balances.ContainsKey(currencyType))
                _balances[currencyType] = 0;

            int previousBalance = _balances[currencyType];
            _balances[currencyType] += amount;
            int newBalance = _balances[currencyType];

            Debug.Log($"[MockWalletService] Added {amount} {currencyType}. " +
                      $"Balance: {previousBalance} -> {newBalance}");

            // Notify via legacy event
            OnBalanceChanged?.Invoke(currencyType, newBalance);

            // Notify via EventBus
            EventBus.Publish(new WalletBalanceChangedEvent
            {
                CurrencyType = currencyType,
                NewBalance = newBalance,
                PreviousBalance = previousBalance
            });
        }

        public bool TrySpend(CurrencyType currencyType, int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[MockWalletService] Cannot spend non-positive amount: {amount}");
                return false;
            }

            int currentBalance = GetBalance(currencyType);

            if (currentBalance < amount)
            {
                Debug.Log($"[MockWalletService] Insufficient {currencyType}. " +
                          $"Required: {amount}, Available: {currentBalance}");
                return false;
            }

            int previousBalance = _balances[currencyType];
            _balances[currencyType] -= amount;
            int newBalance = _balances[currencyType];

            Debug.Log($"[MockWalletService] Spent {amount} {currencyType}. " +
                      $"Balance: {previousBalance} -> {newBalance}");

            OnBalanceChanged?.Invoke(currencyType, newBalance);

            EventBus.Publish(new WalletBalanceChangedEvent
            {
                CurrencyType = currencyType,
                NewBalance = newBalance,
                PreviousBalance = previousBalance
            });

            return true;
        }
    }
}
