using System;
using UnityEngine;
using UnityEngine.Events;

namespace TurkGezer.Missions
{
    [Serializable] public sealed class TaskProgressEvent : UnityEvent<string, int, bool> { }

    [DisallowMultipleComponent]
    public sealed class MissionRunner : MonoBehaviour
    {
        public MissionDefinition definition;
        public bool initializeOnAwake = true;
        public TaskProgressEvent onProgress = new TaskProgressEvent();
        public UnityEvent onCompleted = new UnityEvent();
        public MissionSession Session { get; private set; }

        private void Awake() { if (initializeOnAwake && definition != null) Initialize(definition); }
        public bool Initialize(MissionDefinition mission)
        {
            try
            {
                var session = new MissionSession(mission);
                Detach();
                definition = mission;
                Session = session;
                Session.ProgressChanged += NotifyProgress;
                Session.Completed += NotifyCompleted;
                return true;
            }
            catch (Exception ex) { Debug.LogError("[TurkGezer] " + ex.Message, this); return false; }
        }
        public bool Report(string taskId, string token = null, int amount = 1)
        {
            if (Session == null && !Initialize(definition)) return false;
            return Session.AddProgress(taskId, amount, token);
        }
        public void CompleteTask(string taskId) => Report(taskId);
        public void ResetMission() { if (Session != null) Session.Reset(); }
        private void NotifyProgress(string id, int count, bool completed) => onProgress.Invoke(id, count, completed);
        private void NotifyCompleted() => onCompleted.Invoke();
        private void Detach()
        {
            if (Session == null) return;
            Session.ProgressChanged -= NotifyProgress;
            Session.Completed -= NotifyCompleted;
        }
        private void OnDestroy() => Detach();
    }
}
