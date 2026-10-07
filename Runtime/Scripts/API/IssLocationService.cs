using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace TurkGezer.Api
{
    [DisallowMultipleComponent]
    public sealed class IssLocationService : MonoBehaviour
    {
        public ApiConfiguration configuration;
        public bool startAutomatically = true;
        public UnityEvent<string> onDisplay = new UnityEvent<string>();
        public UnityEvent<string> onError = new UnityEvent<string>();
        public event System.Action<IssPosition> PositionReceived;
        public IssPosition LastPosition { get; private set; }
        public bool IsStale { get; private set; } = true;
        private Coroutine polling;
        private UnityWebRequest activeRequest;

        private void OnEnable() { if (startAutomatically) StartPolling(); }
        public void StartPolling()
        {
            if (polling == null && isActiveAndEnabled) polling = StartCoroutine(Poll());
        }
        public void StopPolling()
        {
            if (activeRequest != null) activeRequest.Abort();
            if (polling != null) StopCoroutine(polling);
            polling = null;
            if (activeRequest != null) activeRequest.Dispose();
            activeRequest = null;
        }
        private void OnDisable() => StopPolling();
        private IEnumerator Poll()
        {
            while (true)
            {
                string endpoint = configuration != null ? configuration.issEndpoint : "https://api.wheretheiss.at/v1/satellites/25544";
                var provider = configuration != null ? configuration.issProvider : IssProvider.WhereTheIssAt;
                if (!System.Uri.TryCreate(endpoint, System.UriKind.Absolute, out var uri) ||
                    (uri.Scheme != "https" && uri.Scheme != "http"))
                {
                    Fail("ISS endpoint adresi gecersiz.");
                    yield return new WaitForSecondsRealtime(10);
                    continue;
                }
                activeRequest = UnityWebRequest.Get(endpoint);
                activeRequest.timeout = configuration != null ? Mathf.Clamp(configuration.timeoutSeconds, 5, 60) : 20;
                yield return activeRequest.SendWebRequest();
                if (activeRequest.result == UnityWebRequest.Result.Success &&
                    IssPositionParser.TryParse(activeRequest.downloadHandler.text, provider, out var position))
                {
                    LastPosition = position;
                    IsStale = false;
                    PositionReceived?.Invoke(position);
                    onDisplay.Invoke(Format(position));
                }
                else Fail("ISS konumu alinamadi veya yanit gecersiz. Son veri guncel olmayabilir.");
                if (activeRequest != null) activeRequest.Dispose();
                activeRequest = null;
                float interval = configuration != null ? Mathf.Max(5, configuration.issPollSeconds) : 10;
                yield return new WaitForSecondsRealtime(interval);
            }
        }
        private void Fail(string message) { IsStale = true; onError.Invoke(message); }
        public static string Format(IssPosition p)
        {
            if (p == null) return "ISS verisi bekleniyor.";
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string text = "ISS GUNCEL KONUM\nEnlem: " + p.latitude.ToString("F2", culture) +
                "°\nBoylam: " + p.longitude.ToString("F2", culture) + "°";
            if (p.hasAltitudeAndVelocity) text += "\nYukseklik: " + p.altitude.ToString("F2", culture) +
                " km\nHiz: " + p.velocity.ToString("F0", culture) + " km/h";
            return text + "\nVeri zamani (UTC): " + System.DateTimeOffset.FromUnixTimeSeconds(p.timestamp).ToString("u");
        }
    }
}
