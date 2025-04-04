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
        [SerializeField] private TodoistIntegrationSettings settings;
        [SerializeField] private bool startAllOver;
        [SerializeField] [MinValue(0.5f)] private float pollingDelaySec = 1f;

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
            var syncToken = "*";

            // if we do not want to start all over, request sync once to get the current sync token
            if (!startAllOver)
                yield return RequestUpdates(syncToken, r => syncToken = r.sync_token);

            while (isActiveAndEnabled)
            {
                yield return RequestUpdates(syncToken, response =>
                {
                    var finishedItems = response.items
                        .Where(it => it.completed_at is { Length: > 0 })
                        .ToList();

                    finishedItems.ForEach(it => Debug.Log(it.completed_at));
                    syncToken = response.sync_token;
                    Debug.Log($"Finished {finishedItems.Count} Todos");
                });
                yield return new WaitForSeconds(pollingDelaySec);
            }
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