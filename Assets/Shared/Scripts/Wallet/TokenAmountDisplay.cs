using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shared.Scripts.Wallet
{
    public class TokenAmountDisplay : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private bool debugMode;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (debugMode)
                TokenWallet.Instance.Amount++;
        }
    }
}