using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CostDisplay : MonoBehaviour
{
    [SerializeField] private Color colorCanAfford = Color.white;
    [SerializeField] private Color colorCanNotAfford = Color.red;

    public bool CanAfford => _cost <= TokenWallet.Instance.Amount;

    public int Cost
    {
        get => _cost;
        set
        {
            _cost = value;
            UpdateText();
        }
    }

    private int _cost;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = _cost.ToString();
        _text.color = GetTextColor();
    }

    private Color GetTextColor()
    {
        return CanAfford ? colorCanAfford : colorCanNotAfford;
    }
}