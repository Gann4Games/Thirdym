using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Gann4Games.Thirdym
{
    /// <summary>
    /// General game information and data.
    /// </summary>
    public class ThirdymAPI
    {

        public event EventHandler OnRequestFinished;

        public class GameCloudData
        {
            public string version;
            public string download;
        }

        public string LastVersion => _gameCloudData.version;
        public string DownloadLink => _gameCloudData.version;
        public string CurrentVersion => Application.version;
        public bool IsUpToDate => LastVersion == CurrentVersion;

        string _dataURL = "https://raw.githubusercontent.com/Gann4Life/Thirdym/release/data.json";
        string _json;
        GameCloudData _gameCloudData;

        public ThirdymAPI() => OnRequestFinished += JsonToClass;

        /// <summary>
        /// Sends the json string data into `_gameCloudData` field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void JsonToClass(object sender, EventArgs args) => _gameCloudData = JsonUtility.FromJson<GameCloudData>(_json);
        
        /// <summary>
        /// Creates http request to save last game version into a json string for further use ingame.
        /// </summary>
        public IEnumerator InitializeRequest()
        {
            var request = UnityWebRequest.Get(_dataURL);
            yield return request.SendWebRequest();
            
            if(request.result == UnityWebRequest.Result.Success)
            {
                _json = request.downloadHandler.text;
                OnRequestFinished?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
