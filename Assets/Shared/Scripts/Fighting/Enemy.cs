using System;
using NaughtyAttributes;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Fighting
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] [MinValue(0)] private int level = 1;
        public SquareParams healthParams;
        public SquareParams attackParams;

        public int MaxHealth => Mathf.RoundToInt(healthParams.Sample(level));
        public int Damage => Mathf.RoundToInt(attackParams.Sample(level));

        public int Level
        {
            get => level;
            set
            {
                level = value;
                Health.MaxHealth = MaxHealth;
                Fighter.Damage = Damage;
            }
        }

        public Health Health { get; private set; }
        public Fighter Fighter { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Fighter = GetComponent<Fighter>();
        }
    }
}