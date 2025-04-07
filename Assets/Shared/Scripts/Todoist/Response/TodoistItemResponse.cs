using System;

namespace Todoist
{
    [Serializable]
    public class TodoistItemResponse
    {
        public string content;
        public string added_at;
        public string completed_at;
    }
}