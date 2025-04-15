using System;
using JetBrains.Annotations;
using UnityEngine;

public class TokenWallet : MonoBehaviour
{
    private static TokenWallet _instance;

    public static TokenWallet Instance
    {
        get
        {
            if (_instance == null) _instance = new GameObject("TokenWallet").AddComponent<TokenWallet>();
            return _instance;
        }
        private set => _instance = value;
    }

    private int _amount;

    public int Amount
    {
        get => _amount;
        set
        {
            var amountBefore = _amount;
            _amount = value;
            GlobalEvents.onWalletTokenCountChanged?.Invoke(new WalletTokenCountChangedEvent
            {
                amountBefore = amountBefore,
                amountNow = _amount
            });
        }
    }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GlobalEvents.onTodoCompleted += OnTodoCompleted;
    }

    private void OnDisable()
    {
        GlobalEvents.onTodoCompleted -= OnTodoCompleted;
    }

    private void OnTodoCompleted(TodoCompletedEvent obj)
    {
        Amount++;
    }

    public bool TryTransaction(int cost, Action onSuccess = null, Action onFail = null)
    {
        if (cost > _amount)
        {
            onFail?.Invoke();
            return false;
        }

        Amount -= cost;
        onSuccess?.Invoke();
        return true;
    }
}