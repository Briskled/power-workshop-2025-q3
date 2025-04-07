using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fighting;
using UnityEngine;


namespace Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class Character : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1f;

        public Health Health { get; private set; }
        public Fighter Fighter { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Fighter = GetComponent<Fighter>();
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