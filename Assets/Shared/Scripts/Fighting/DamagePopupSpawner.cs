using System;
using System.Collections;
using System.Collections.Generic;
using Fighting;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Health))]
public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private Vector3 spawnPositionOffset;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.onHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _health.onHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float before, float after)
    {
        if (before < after)
            return;

        var popupObject = Instantiate(damagePopupPrefab, transform.position + spawnPositionOffset, Quaternion.identity);
        var popup = popupObject.GetComponent<DamagePopup>();

        var diff = after - before;
        popup.Value = Mathf.RoundToInt(diff);
    }
}