using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace TurkGezer.Api
{
    [DisallowMultipleComponent]
    public sealed class ChatService : MonoBehaviour
    {
        public ApiConfiguration configuration;
        [TextArea] public string systemPrompt = "Bir egitsel uzay uygulamasinin sanal rehberisin. Kullanici duzeyine uygun, kisa ve kaynaklarla kontrol edilebilir yanitlar ver. Bilmedigin konuda uydurma; gercek bir kisiyi temsil ettigini iddia etme.";
        public UnityEvent<string> onResponse = new UnityEvent<string>();
        public UnityEvent<string> onError = new UnityEvent<string>();
        public bool IsBusy { get; private set; }
        private UnityWebRequest activeRequest;
        [Serializable] private sealed class Message { public string role; public string content; }
        [Serializable] private sealed class Request { public string model; public Message[] messages; public bool stream; }
        [Serializable] private sealed class Choice { public Message message; }
        [Serializable] private sealed class Response { public Choice[] choices; }

        public void Ask(string question)
        {
            if (IsBusy || !isActiveAndEnabled) return;
            StartCoroutine(Generate(question, systemPrompt, answer => onResponse.Invoke(answer), error => onError.Invoke(error)));
        }
        /// <summary>Proxy: ayni request semasi; yanit choices[0].message.content. Anahtar yalniz sunucuda.</summary>
        public IEnumerator Generate(string question, string prompt, Action<string> success, Action<string> failure)
        {
            if (IsBusy) { failure?.Invoke("Onceki yanit bekleniyor."); yield break; }
            if (string.IsNullOrWhiteSpace(question) || question.Length > 1500)
            { failure?.Invoke("Soru bos olamaz ve 1500 karakteri gecemez."); yield break; }
            string endpoint = configuration != null ? configuration.chatEndpoint : "";
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != "https")
            { failure?.Invoke("Soru-cevap icin HTTPS proxy sunucusu yapilandirilmali."); yield break; }
            if (string.IsNullOrWhiteSpace(configuration.chatModel))
            { failure?.Invoke("API yapilandirmasinda saglayicinin model kimligini belirtin."); yield break; }
            string key = null;
            if (!configuration.chatUsesProxy)
            {
#if UNITY_EDITOR
                if (configuration.allowEditorDirectChat) key = Environment.GetEnvironmentVariable("TURKGEZER_CHAT_API_KEY");
#endif
                if (string.IsNullOrWhiteSpace(key))
                { failure?.Invoke("Dogrudan API kullanimi yalniz Editor'de ortam anahtariyla mumkun. Dagitimda proxy kullanin."); yield break; }
            }
            var payload = new Request { model = configuration.chatModel,
                messages = new[] { new Message { role = "system", content = prompt }, new Message { role = "user", content = question.Trim() } } };
            IsBusy = true;
            try
            {
                activeRequest = new UnityWebRequest(endpoint, "POST");
                activeRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload)));
                activeRequest.downloadHandler = new DownloadHandlerBuffer();
                activeRequest.timeout = Mathf.Clamp(configuration.timeoutSeconds, 5, 60);
                activeRequest.SetRequestHeader("Content-Type", "application/json");
                if (key != null) activeRequest.SetRequestHeader("Authorization", "Bearer " + key.Trim());
                yield return activeRequest.SendWebRequest();
                if (activeRequest == null) { failure?.Invoke("Soru-cevap istegi iptal edildi."); yield break; }
                if (activeRequest.result != UnityWebRequest.Result.Success)
                { failure?.Invoke("Soru-cevap sunucusuna ulasilamadi (HTTP " + activeRequest.responseCode + ")."); yield break; }
                string answer = ParseResponse(activeRequest.downloadHandler.text);
                if (answer == null) failure?.Invoke("Soru-cevap yaniti gecersiz.");
                else success?.Invoke(answer);
            }
            finally
            {
                if (activeRequest != null) activeRequest.Dispose();
                activeRequest = null;
                IsBusy = false;
            }
        }
        public static string ParseResponse(string json)
        {
            try
            {
                var response = JsonUtility.FromJson<Response>(json);
                if (response == null || response.choices == null || response.choices.Length == 0) return null;
                return response.choices[0]?.message?.content;
            }
            catch (ArgumentException) { return null; }
        }
        private void OnDisable()
        {
            if (activeRequest != null) activeRequest.Abort();
            StopAllCoroutines();
            if (activeRequest != null) activeRequest.Dispose();
            activeRequest = null;
            IsBusy = false;
        }
    }
}
