using TurkGezer.Api;
using TurkGezer.Missions;
using UnityEngine;
using UnityEngine.UI;

namespace TurkGezer.Samples
{
    /// <summary>Yalniz ornek gorunum. Cekirdek gorev ve API kodu UI'ya bagimli degildir.</summary>
    public sealed class SampleHud : MonoBehaviour
    {
        public MissionRunner mission;
        public Text progressText;
        public Text informationText;
        public Text positionText;
        public InputField questionInput;
        public ChatService chat;
        public void ShowInformation(string text) { if (informationText != null) informationText.text = text; }
        public void ShowPosition(string text) { if (positionText != null) positionText.text = text; }
        public void SendQuestion()
        {
            if (chat != null && questionInput != null) chat.Ask(questionInput.text);
        }
        private void Update()
        {
            if (mission == null || mission.definition == null || progressText == null) return;
            var text = new System.Text.StringBuilder(mission.definition.title + "\n");
            foreach (var task in mission.definition.tasks)
                text.Append(task.title).Append(": ").Append(mission.Session != null ? mission.Session.GetProgress(task.id) : 0)
                    .Append('/').Append(task.targetCount).Append('\n');
            progressText.text = text.ToString();
        }
    }
}
