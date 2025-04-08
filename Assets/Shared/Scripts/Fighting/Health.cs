using NaughtyAttributes;
using UnityEngine;

namespace Fighting
{
    public delegate void HealthChangedDelegate(float before, float after);

    public delegate void DeathDelegate();

    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private bool startFullLive = true;

        public bool IsAlive => currentHealth > 0;

        public bool IsDead => !IsAlive;

        /// <summary>
        /// Sets max health and invokes <see cref="onMaxHealthChanged"/>.
        /// The given value will be corrected if it is below 1 to have always at least 1 HP.
        /// If max health is to be set lower than current health, the current health will be set to max health using <see cref="CurrentHealth"/>
        /// </summary>
        public float MaxHealth
        {
            get => maxHealth;
            set
            {
                if (Mathf.Approximately(maxHealth, value))
                    return;
                var maxHealthBefore = maxHealth;
                maxHealth = Mathf.Max(value, 1f);
                onMaxHealthChanged?.Invoke(maxHealthBefore, maxHealth);
                if (maxHealth > currentHealth)
                    CurrentHealth = maxHealth;
            }
        }

        /// <summary>
        /// Sets the current health and invokes <see cref="onHealthChanged"/>.
        /// The given value will be corrected if it is below 0 or larger than <see cref="MaxHealth"/> to always stay in bounds.
        /// If the value is 0 or lower, <see cref="onDeath"/> will be invoked.
        /// </summary>
        public float CurrentHealth
        {
            get => currentHealth;
            set
            {
                if (Mathf.Approximately(currentHealth, value))
                    return;
                var healthBefore = currentHealth;
                currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                onHealthChanged?.Invoke(healthBefore, currentHealth);
                if (currentHealth <= 0) onDeath?.Invoke();
            }
        }

        public void Kill()
        {
            CurrentHealth = 0;
        }

        public void Damage(float damage)
        {
            CurrentHealth -= damage;
        }

        public void Heal(float hp)
        {
            CurrentHealth += hp;
        }

        public void HealFully()
        {
            CurrentHealth = MaxHealth;
        }

        [Button]
        public void TestDamage()
        {
            Damage(20);
        }

        [Button]
        public void TestHeal()
        {
            HealFully();
        }

        private void Start()
        {
            if (startFullLive)
                CurrentHealth = MaxHealth;
        }

        public event HealthChangedDelegate onHealthChanged;
        public event HealthChangedDelegate onMaxHealthChanged;
        public event DeathDelegate onDeath;
    }
}