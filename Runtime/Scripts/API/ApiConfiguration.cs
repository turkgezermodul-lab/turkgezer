using UnityEngine;

namespace TurkGezer.Api
{
    public enum IssProvider { WhereTheIssAt, OpenNotify }

    [CreateAssetMenu(menuName = "TurkGezer/API Yapilandirmasi", fileName = "ApiAyarlar")]
    public sealed class ApiConfiguration : ScriptableObject
    {
        public IssProvider issProvider = IssProvider.WhereTheIssAt;
        public string issEndpoint = "https://api.wheretheiss.at/v1/satellites/25544";
        [Min(5)] public float issPollSeconds = 10;
        [Range(5, 60)] public int timeoutSeconds = 20;
        [Tooltip("Uretimde kendi HTTPS sunucunuz. Saglayici anahtari sunucuda tutulur.")]
        public string chatEndpoint = "";
        public string chatModel = "";
        public bool chatUsesProxy = true;
        [Tooltip("Yalniz Editor'de TURKGEZER_CHAT_API_KEY ortam degiskeninden dogrudan istek.")]
        public bool allowEditorDirectChat;
    }
}
