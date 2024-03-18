using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Plugins.Scripts.Dropbox
{
    public static class DropboxHelper
    {
        // paste from dropbox console
        private const string AppKey = "ken2tpyvd50urd2";
        private const string AppSecret = "mu6bfy3gbztvl3j";

#if UNITY_EDITOR
        // paste auth code from browser here. Only valid for about an hour
        private const string AuthCode = "KVKuMq6fMooAAAAAAAAAEhsGoLbH68_jgA37SnFxnf8";
#endif

        // paste from GetRefreshToken() result, value of "refresh_token"
        private const string RefreshToken = "k2LX2MsDo-UAAAAAAAAAAYkzNizg9xIilD1zucVXKlgJolhan3T83D6FpqhQjCGZ";


        private static string _tempRuntimeToken = null;

#if UNITY_EDITOR
        // Сначала в редакторе запускаем это и вставляем ответ в поле AuthCode
        public static void GetAuthCode()
        {
            var url = $"https://www.dropbox.com/oauth2/authorize?client_id={AppKey}&response_type=code&token_access_type=offline";
            Application.OpenURL(url);
        }

        // Затем, когда вставили AuthCode, вызываем это. В ответе придёт refreshToken, который нужно тоже записать
        public static async void GetRefreshToken()
        {
            var form = new WWWForm();
            form.AddField("code", AuthCode);
            form.AddField("grant_type", "authorization_code");

            var base64Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AppKey}:{AppSecret}"));

            using var request = UnityWebRequest.Post("https://api.dropbox.com/oauth2/token", form);
            request.SetRequestHeader("Authorization", $"Basic {base64Authorization}");

            var sendRequest = request.SendWebRequest();

            while (!sendRequest.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
                return;
            }

            var parsedAnswer = JObject.Parse(request.downloadHandler.text);
            var refreshTokenString = parsedAnswer["refresh_token"]?.Value<string>();

            Debug.Log("Copy this string to RefreshToken: " + refreshTokenString);
        }
#endif

        /// <summary>
        /// Инициализируем хелпер перед тем как начинаем скачивать файлы с сервера.
        /// Чтобы вызвать внутри корутины используйте
        /// var task = DropboxHelper.Initialize();
        /// yield return new WaitUntil(() => task.IsCompleted);
        /// </summary>
        public static async Task Initialize()
        {
            if (string.IsNullOrEmpty(RefreshToken))
            {
                Debug.LogError("refreshToken should be defined before runtime");
            }

            var form = new WWWForm();
            form.AddField("grant_type", "refresh_token");
            form.AddField("refresh_token", RefreshToken);

            var base64Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AppKey}:{AppSecret}"));

            using var request = UnityWebRequest.Post("https://api.dropbox.com/oauth2/token", form);
            request.SetRequestHeader("Authorization", $"Basic {base64Authorization}");

            var sendRequest = request.SendWebRequest();

            while (!sendRequest.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
            }

            // Debug.Log("Success! Full message from dropbox: " + request.downloadHandler.text);

            var data = JObject.Parse(request.downloadHandler.text);
            _tempRuntimeToken = data["access_token"]?.Value<string>();
        }

        /// <summary>
        /// Отсылаем возвращаемый запрос. В ответе придёт файл, лежащий по указанному относительному пути.
        /// Чтобы вызвать внутри корутины используйте
        /// var task = DropboxHelper.GetRequestForFileDownload();
        /// yield return new WaitUntil(() => task.IsCompleted);
        /// </summary>
        /// <param name="relativePathToFile"></param>
        /// <returns></returns>
        public static UnityWebRequest GetRequestForFileDownload(string relativePathToFile)
        {
            var request = UnityWebRequest.Get("https://content.dropboxapi.com/2/files/download");
            request.SetRequestHeader("Authorization", $"Bearer {_tempRuntimeToken}");
            request.SetRequestHeader("Dropbox-API-Arg", $"{{\"path\": \"/{relativePathToFile}\"}}");
            Debug.Log(request.url);
            return request;
        }
    }
}