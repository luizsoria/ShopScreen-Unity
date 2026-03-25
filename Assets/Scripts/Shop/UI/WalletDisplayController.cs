using System;
using UnityEngine;
using UnityEngine.UIElements;
using Common.UI;
using Shop.Data;
using Shop.Services;

namespace Shop.UI
{
    /// <summary>
    /// Controller for the wallet display in the shop header.
    /// Manages the visual representation of currency balances with animated
    /// number transitions and the "+" button for quick navigation.
    /// </summary>
    public class WalletDisplayController
    {
        private readonly Label _amountLabel;
        private readonly Button _addButton;
        private readonly VisualElement _container;
        private readonly CurrencyType _currencyType;
        private readonly IWalletService _walletService;

        private int _displayedBalance;

        public WalletDisplayController(
            VisualElement container,
            Label amountLabel,
            Button addButton,
            CurrencyType currencyType,
            IWalletService walletService)
        {
            _container = container;
            _amountLabel = amountLabel;
            _addButton = addButton;
            _currencyType = currencyType;
            _walletService = walletService;

            // Set initial balance
            _displayedBalance = _walletService.GetBalance(_currencyType);
            UpdateDisplay(_displayedBalance);

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
                int previousBalance = _displayedBalance;
                _displayedBalance = newBalance;

                // Animate the number change
                UIAnimationHelper.AnimateNumber(_amountLabel, previousBalance, newBalance, 600f);

                // Bounce the container for visual feedback
                if (_container != null)
                {
                    UIAnimationHelper.ScaleBounce(_container, 1.1f, 200f);
                }
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
            Debug.Log($"[WalletDisplay] Add {_currencyType} button clicked.");

            // Publish tab switch event
            Common.Events.EventBus.Publish(new Events.TabSwitchRequestedEvent
            {
                Tab = _currencyType == CurrencyType.Money
                    ? Data.ShopTabType.Money
                    : Data.ShopTabType.Coins
            });
        }

        private string FormatNumber(int number)
        {
            return number.ToString("N0").Replace(",", ".");
        }
    }
}
