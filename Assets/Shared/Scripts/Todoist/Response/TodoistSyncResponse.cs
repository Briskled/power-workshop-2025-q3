using System;
using System.Collections.Generic;

namespace Todoist
{
    [Serializable]
    public class TodoistSyncResponse
    {
        public bool full_sync;
        public string sync_token;
        public List<TodoistItemResponse> items;
    }
}