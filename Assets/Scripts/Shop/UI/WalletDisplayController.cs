using System;
using UnityEngine;
using UnityEngine.UIElements;
using Shop.Data;
using Shop.Services;

namespace Shop.UI
{
    /// <summary>
    /// Controller for the wallet display in the shop header.
    /// Manages the visual representation of currency balances and the "+" button.
    /// </summary>
    public class WalletDisplayController
    {
        private readonly Label _amountLabel;
        private readonly Button _addButton;
        private readonly CurrencyType _currencyType;
        private readonly IWalletService _walletService;

        public WalletDisplayController(
            Label amountLabel,
            Button addButton,
            CurrencyType currencyType,
            IWalletService walletService)
        {
            _amountLabel = amountLabel;
            _addButton = addButton;
            _currencyType = currencyType;
            _walletService = walletService;

            // Set initial balance
            UpdateDisplay(_walletService.GetBalance(_currencyType));

            // Listen for balance changes
            _walletService.OnBalanceChanged += OnBalanceChanged;

            // Register add button callback
            if (_addButton != null)
            {
                _addButton.clicked += OnAddButtonClicked;
            }
        }

        /// <summary>
        /// Clean up event subscriptions.
        /// </summary>
        public void Dispose()
        {
            _walletService.OnBalanceChanged -= OnBalanceChanged;

            if (_addButton != null)
            {
                _addButton.clicked -= OnAddButtonClicked;
            }
        }

        private void OnBalanceChanged(CurrencyType type, int newBalance)
        {
            if (type == _currencyType)
            {
                UpdateDisplay(newBalance);
            }
        }

        private void UpdateDisplay(int balance)
        {
            if (_amountLabel != null)
            {
                _amountLabel.text = FormatNumber(balance);
            }
        }

        private void OnAddButtonClicked()
        {
            Debug.Log($"[WalletDisplay] Add {_currencyType} button clicked. " +
                      $"Navigate to {_currencyType} purchase tab.");
        }

        private string FormatNumber(int number)
        {
            return number.ToString("N0").Replace(",", ".");
        }
    }
}
