using System;
using Todoist;
using Unity.VisualScripting;
using UnityEngine;

public static class GlobalEvents
{
    public static Action<TodoCompletedEvent> onTodoCompleted;
}

public class TodoCompletedEvent
{
    public TodoistItemResponse todoResponse;
}