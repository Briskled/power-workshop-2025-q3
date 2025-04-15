using System;
using DG.Tweening;
using Fighting;
using NaughtyAttributes;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class StatShopItem : MonoBehaviour
    {
        [SerializeField] private StatType statType;
        [SerializeField] private Character character;
        [SerializeField] private TextMeshProUGUI currentValueText;
        [SerializeField] private TextMeshProUGUI changeText;
        [SerializeField] private int cost;
        [SerializeField] private CostDisplay costDisplay;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (!TokenWallet.Instance.TryTransaction(cost)) return;

            cost++;
            character.LevelUp(statType);
            UpdateText();
            // todo animation
        }

        private void OnEnable()
        {
            character.onHealthLevelChanged += OnHealthLevelChanged;
            character.onDamageLevelChanged += OnDamageLevelChanged;
        }

        private void OnDisable()
        {
            character.onHealthLevelChanged -= OnHealthLevelChanged;
            character.onDamageLevelChanged -= OnDamageLevelChanged;
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDamageLevelChanged(int newlevel)
        {
            UpdateText();
        }

        private void OnHealthLevelChanged(int newlevel)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            var currentValue = character.GetStat(statType);
            var nextValue = character.GetStatAtLevel(statType, character.GetLevel(statType) + 1);
            var diff = nextValue - currentValue;
            currentValueText.text = currentValue.ToString();
            changeText.text = "+" + diff;
            costDisplay.Cost = cost;
        }
    }
}