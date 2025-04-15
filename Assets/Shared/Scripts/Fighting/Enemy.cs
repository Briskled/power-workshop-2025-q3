using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using Unity.Cinemachine;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;

namespace Fighting
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class Enemy : MonoBehaviour
    {
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");
        [SerializeField] [MinValue(0)] private int level = 1;
        [SerializeField] private float dissolveTime = 1;
        [SerializeField] private float dissolveDelay = 1;
        private List<Material> _materials;
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

            _materials = new List<Material>();
            _materials.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>().Select(it => it.material));
        }

        private void OnEnable()
        {
            Health.onDeath += OnDeath;
        }

        private void OnDisable()
        {
            Health.onDeath -= OnDeath;
        }

        private void OnDeath()
        {
            var dissolve = 0f;
            DOTween.To(
                    () => dissolve,
                    x =>
                    {
                        dissolve = x;
                        _materials.ForEach(mat => mat.SetFloat(Dissolve, x));
                    },
                    1,
                    dissolveTime)
                .SetDelay(dissolveDelay)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}