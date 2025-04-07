using UnityEngine;

namespace Todoist
{
    [CreateAssetMenu(menuName = "Power/Todoist Integration Settings")]
    public class TodoistIntegrationSettings : ScriptableObject
    {
        public string todoistBaseUri;
        public string todoistSyncPath;
        [Password] public string apiKey;

        public string TodoistSyncUri => todoistBaseUri + todoistSyncPath;
    }
}