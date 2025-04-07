using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;

namespace Todoist
{
    public class TodoistIntegrator : MonoBehaviour
    {
        private const string KeySyncToken = "syncToken";

        [SerializeField] private TodoistIntegrationSettings settings;
        [SerializeField] private bool startAllOver;
        [SerializeField] [MinValue(0.5f)] private float pollingDelaySec = 1f;

        private string SyncToken
        {
            get => PlayerPrefs.GetString(KeySyncToken, "*");
            set => PlayerPrefs.SetString(KeySyncToken, value);
        }

        private Coroutine _syncCoroutine;

        private void OnEnable()
        {
            _syncCoroutine = StartCoroutine(RunSyncLoop());
        }

        private void OnDisable()
        {
            if (_syncCoroutine == null) return;

            StopCoroutine(_syncCoroutine);
            _syncCoroutine = null;
        }

        private IEnumerator RunSyncLoop()
        {
            // By default, sync token will be persistent during. If we want to start all over, let's reset it
            if (startAllOver)
                SyncToken = "*";

            while (isActiveAndEnabled)
            {
                yield return RequestUpdates(SyncToken, response =>
                {
                    var finishedItems = response.items
                        .Where(it => it.completed_at is { Length: > 0 })
                        .ToList();

                    finishedItems.ForEach(it =>
                        GlobalEvents.onTodoCompleted?.Invoke(new TodoCompletedEvent { todoResponse = it })
                    );
                    SyncToken = response.sync_token;
                    Debug.Log($"Finished {finishedItems.Count} Todos");
                });
                yield return new WaitForSeconds(pollingDelaySec);
            }

            Debug.Log("Done requesting updates");
        }

        private IEnumerator RequestUpdates(string token, Action<TodoistSyncResponse> then)
        {
            var form = new WWWForm();
            form.AddField("sync_token", token);
            form.AddField("resource_types", "[\"items\"]");

            var uwr = UnityWebRequest.Post(settings.TodoistSyncUri, form);
            uwr.SetRequestHeader("Authorization", $"Bearer {settings.apiKey}");
            yield return uwr.SendWebRequest();

            if (uwr.result
                is UnityWebRequest.Result.ConnectionError
                or UnityWebRequest.Result.ProtocolError
                or UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError(uwr.error);
                Debug.LogError(uwr.downloadHandler.text);
            }
            else
            {
                var responseJson = uwr.downloadHandler.text;
                var response = JsonUtility.FromJson<TodoistSyncResponse>(responseJson);

                then(response);
            }
        }
    }
}