using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fighting;
using UnityEngine;


namespace Player
{
    public delegate void LevelChangedDelegate(int newLevel);


    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class Character : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private SquareParams healthParams;
        [SerializeField] private SquareParams damageParams;
        [SerializeField] private int healthLevel = 1;
        [SerializeField] private int damageLevel = 1;


        public int MaxHealth => GetStatAtLevel(StatType.Health, healthLevel);
        public int Damage => GetStatAtLevel(StatType.Damage, damageLevel);

        public event LevelChangedDelegate onHealthLevelChanged;
        public event LevelChangedDelegate onDamageLevelChanged;

        public int HealthLevel
        {
            get => healthLevel;
            set
            {
                healthLevel = value;
                Health.MaxHealth = MaxHealth;
                onHealthLevelChanged?.Invoke(healthLevel);
            }
        }

        public int DamageLevel
        {
            get => damageLevel;
            set
            {
                damageLevel = value;
                Fighter.Damage = Damage;
                onDamageLevelChanged?.Invoke(damageLevel);
            }
        }

        public int GetStat(StatType statType)
        {
            return statType == StatType.Damage ? Damage : MaxHealth;
        }

        public void LevelUp(StatType statType)
        {
            if (statType == StatType.Damage)
                DamageLevel++;
            else
                HealthLevel++;
        }

        public int GetStatAtLevel(StatType statType, int level)
        {
            return statType == StatType.Damage
                ? Mathf.RoundToInt(damageParams.Sample(level))
                : Mathf.RoundToInt(healthParams.Sample(level));
        }

        public int GetLevel(StatType statType)
        {
            return statType == StatType.Damage ? damageLevel : healthLevel;
        }

        public Health Health { get; private set; }
        public Fighter Fighter { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Fighter = GetComponent<Fighter>();
        }

        private void Start()
        {
            Health.MaxHealth = MaxHealth;
            Fighter.Damage = Damage;
        }

        public async UniTask SlideTo(float targetPositionX, float duration)
        {
            var ct = this.GetCancellationTokenOnDestroy();
            await transform.DOMoveX(targetPositionX, duration)
                .SetEase(Ease.InOutBack)
                .WithCancellation(ct);
        }

        public async UniTask WalkTo(float restingPositionX)
        {
            var difference = restingPositionX - transform.position.x;
            var time = difference / movementSpeed;
            await transform.DOMoveX(restingPositionX, time)
                .SetEase(Ease.InOutQuad)
                .WithCancellation(this.GetCancellationTokenOnDestroy());
        }
    }
}