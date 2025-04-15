using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fighting
{
    public class HealthBar : MonoBehaviour
    {
        [Header("References")] [SerializeField] [Required]
        private Health health;

        [SerializeField] [Required] private Slider barSlider;
        [SerializeField] [Required] private Slider impactSlider;
        [SerializeField] [Required] private TextMeshProUGUI healthText;
        [SerializeField] [Required] private Image flashOverlay;

        [Header("Positioning")]
        [SerializeField]
        [Tooltip("The health bar's position relative to the target health object")]
        private Vector3 offset = Vector3.up * 1;

        [Header("Timing")] [SerializeField] private float barLerpTime = .3f;
        [SerializeField] private float impactDelay = 1f;
        [SerializeField] private float impactLerpTime = .5f;

        [Header("Flash")] [Space] [SerializeField]
        private bool doFlash = true;

        [SerializeField] [ShowIf(nameof(doFlash))]
        private float flashTime = .1f;

        [SerializeField] [ShowIf(nameof(doFlash))] [Range(0, 1)]
        private float flashIntensity = .6f;

        [Header("Shake")] [Space] [SerializeField]
        private bool doShake = true;

        [SerializeField] [ShowIf(nameof(doShake))]
        private float shakeTime = .35f;

        [SerializeField] [ShowIf(nameof(doShake))]
        private float shakeIntensity = 12;

        [SerializeField] [ShowIf(nameof(doShake))] [MinValue(0)]
        private int shakeSpeed = 25;

        private Tweener _impactTweener;

        private void OnEnable()
        {
            health.onHealthChanged += OnHealthChanged;
            health.onMaxHealthChanged += OnMaxHealthChanged;
            health.onDeath += OnDeath;
        }

        private void OnDisable()
        {
            health.onHealthChanged -= OnHealthChanged;
            health.onMaxHealthChanged -= OnMaxHealthChanged;
            health.onDeath -= OnDeath;
        }

        private void OnDeath()
        {
            Destroy(gameObject);
        }

        private void Start()
        {
            UpdateBar(health.CurrentHealth, health.MaxHealth, true, true);
        }

        private void OnHealthChanged(float before, float after)
        {
            UpdateBar(after, health.MaxHealth);
        }

        private void OnMaxHealthChanged(float before, float after)
        {
            UpdateBar(health.CurrentHealth, after);
        }

        public void UpdateBar(float currentHealth, float maxHealth, bool bypassFlash = false, bool bypassShake = false)
        {
            barSlider.maxValue = maxHealth;
            impactSlider.maxValue = maxHealth;
            healthText.text = $"{currentHealth:0} / {maxHealth:0}";

            DOTween.To(
                    () => barSlider.value,
                    x => barSlider.value = x,
                    currentHealth,
                    barLerpTime)
                .SetEase(Ease.OutCirc);

            if (currentHealth > impactSlider.value)
            {
                // heal
                impactSlider.value = currentHealth;
            }
            else
            {
                // damage
                if (_impactTweener?.IsPlaying() == true)
                    _impactTweener.Kill();
                _impactTweener = DOTween.To(
                        () => impactSlider.value,
                        x => impactSlider.value = x,
                        currentHealth,
                        impactLerpTime)
                    .SetEase(Ease.InCirc)
                    .SetDelay(impactDelay);

                if (doFlash && !bypassFlash)
                    flashOverlay.DOFade(flashIntensity, flashTime * .5f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => flashOverlay.color = new Color(flashOverlay.color.r, flashOverlay.color.g,
                            flashOverlay.color.b, 0))
                        .SetLoops(2, LoopType.Yoyo);

                if (doShake && !bypassShake)
                    transform.DOShakeRotation(shakeTime, Vector3.one * shakeIntensity, shakeSpeed)
                        .OnComplete(() => transform.rotation = Quaternion.identity);
            }
        }

        private void LateUpdate()
        {
            transform.position = health.transform.position + offset;
        }

        private void Reset()
        {
            health = GetComponentInParent<Health>();
        }
    }
}