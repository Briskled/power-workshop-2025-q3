using System;
using Fighting;
using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        levelText.text = enemy.Level.ToString();
    }
}