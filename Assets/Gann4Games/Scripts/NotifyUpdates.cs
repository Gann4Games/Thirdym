using UnityEngine;
using UnityEngine.Events;
using Gann4Games.Thirdym;

namespace Thirdym
{
    public class NotifyUpdates : MonoBehaviour
    {
        [SerializeField] UnityEvent onUpdateFound;

        readonly ThirdymAPI _gameAPI = new ThirdymAPI();

        private void Start() => StartCoroutine(_gameAPI.InitializeRequest());
        private void OnEnable() => _gameAPI.OnRequestFinished += CheckForUpdates;
        private void OnDisable() => _gameAPI.OnRequestFinished -= CheckForUpdates;

        void CheckForUpdates(object sender, System.EventArgs args)
        {
            if (_gameAPI.IsUpToDate) return;

            NotificationHandler.Notify($"Update available! (v{_gameAPI.LastVersion})");
            onUpdateFound.Invoke();
        }
    }
}
