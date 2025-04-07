using System;
using TMPro;
using UnityEngine;

namespace Shared.Scripts.Wallet
{
    public class TokenAmountDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            UpdateText(TokenWallet.Instance.Amount);
        }

        private void UpdateText(int instanceAmount)
        {
            text.text = instanceAmount.ToString();
        }

        private void OnEnable()
        {
            GlobalEvents.onWalletTokenCountChanged += OnTokenCountChanged;
        }

        private void OnDisable()
        {
            GlobalEvents.onWalletTokenCountChanged -= OnTokenCountChanged;
        }

        private void OnTokenCountChanged(WalletTokenCountChangedEvent obj)
        {
            UpdateText(obj.amountNow);
        }
    }
}