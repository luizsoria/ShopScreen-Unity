using System;
using System.Collections.Generic;
using UnityEngine;
using Shop.Data;

namespace Shop.Services
{
    /// <summary>
    /// Mock implementation of IWalletService for testing purposes.
    /// Manages in-memory currency balances.
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

            _balances[currencyType] += amount;

            Debug.Log($"[MockWalletService] Added {amount} {currencyType}. " +
                      $"New balance: {_balances[currencyType]}");

            OnBalanceChanged?.Invoke(currencyType, _balances[currencyType]);
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

            _balances[currencyType] -= amount;

            Debug.Log($"[MockWalletService] Spent {amount} {currencyType}. " +
                      $"New balance: {_balances[currencyType]}");

            OnBalanceChanged?.Invoke(currencyType, _balances[currencyType]);
            return true;
        }
    }
}
