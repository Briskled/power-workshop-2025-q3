using System;
using Fighting;
using Todoist;
using Unity.VisualScripting;
using UnityEngine;

public static class GlobalEvents
{
    public static Action<TodoCompletedEvent> onTodoCompleted;
    public static Action<GameStateChangedEvent> onGameStateChanged;
    public static Action<WalletTokenCountChangedEvent> onWalletTokenCountChanged;
}

public class TodoCompletedEvent
{
    public TodoistItemResponse todoResponse;
}

public class GameStateChangedEvent
{
    public GameState stateBefore;
    public GameState stateNow;
}

public class WalletTokenCountChangedEvent
{
    public int amountBefore;
    public int amountNow;
}