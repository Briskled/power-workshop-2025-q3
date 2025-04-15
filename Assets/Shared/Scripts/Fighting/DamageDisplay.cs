using System;
using TMPro;
using UnityEngine;

namespace Fighting
{
    public class DamageDisplay : MonoBehaviour
    {
        [SerializeField] private Fighter fighter;
        [SerializeField] private TextMeshProUGUI damageText;

        private void Start()
        {
            UpdateText();
        }

        private void OnEnable()
        {
            fighter.onDamageChanged += OnDamageChanged;
        }

        private void OnDisable()
        {
            fighter.onDamageChanged -= OnDamageChanged;
        }

        private void OnDamageChanged(int damagebefore, int damagenow)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            damageText.text = fighter.Damage.ToString();
        }
    }
}