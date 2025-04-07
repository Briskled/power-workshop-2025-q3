using System;
using DG.Tweening;
using UnityEngine;

namespace Shared.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CanvasGroup restingView;
        [SerializeField] private float restingViewFadeDuration;
        [SerializeField] private CanvasGroup shopView;
        [SerializeField] private float shopViewFadeDuration;

        private void Start()
        {
            SetViewVisible(restingView, 0, false, false);
            SetViewVisible(shopView, 0, false, false);
        }

        private void OnEnable()
        {
            GlobalEvents.onGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            GlobalEvents.onGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameStateChangedEvent obj)
        {
            if (obj.stateNow == GameState.Resting) SetRestingViewVisible(true);
            if (obj.stateBefore == GameState.Resting) SetRestingViewVisible(false);
            if (obj.stateNow == GameState.Shopping) SetShopViewVisible(true);
            if (obj.stateBefore == GameState.Shopping) SetShopViewVisible(false);
        }

        public void SetRestingViewVisible(bool visible)
        {
            SetViewVisible(restingView, restingViewFadeDuration, visible);
        }

        public void SetShopViewVisible(bool visible)
        {
            SetViewVisible(shopView, shopViewFadeDuration, visible);
        }

        private void SetViewVisible(CanvasGroup canvasGroup, float fadeDuration, bool visible, bool animate = true)
        {
            if (visible)
            {
                if (animate)
                {
                    canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
                    {
                        canvasGroup.interactable = true;
                        canvasGroup.blocksRaycasts = true;
                    });
                }
                else
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
            else
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                if (animate)
                    canvasGroup.DOFade(0, fadeDuration);
                else
                    canvasGroup.alpha = 0;
            }
        }
    }
}