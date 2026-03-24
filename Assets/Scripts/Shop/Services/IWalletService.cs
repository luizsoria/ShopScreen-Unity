using System;
using Shop.Data;

namespace Shop.Services
{
    /// <summary>
    /// Interface for wallet/balance management operations.
    /// Follows Interface Segregation Principle (ISP) - only wallet-related methods.
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Event fired when any balance changes.
        /// </summary>
        event Action<CurrencyType, int> OnBalanceChanged;

        /// <summary>
        /// Get the current balance for a currency type.
        /// </summary>
        int GetBalance(CurrencyType currencyType);

        /// <summary>
        /// Add currency to the wallet.
        /// </summary>
        void AddBalance(CurrencyType currencyType, int amount);

        /// <summary>
        /// Try to spend currency from the wallet.
        /// </summary>
        /// <returns>True if the player had enough balance.</returns>
        bool TrySpend(CurrencyType currencyType, int amount);
    }
}
