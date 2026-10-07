using TurkGezer.Missions;
using UnityEngine;
using UnityEngine.Events;

namespace TurkGezer.Interaction
{
    [DisallowMultipleComponent]
    public class ModuleInteractable : MonoBehaviour
    {
        public string label = "Etkilesim";
        public MissionRunner mission;
        public string taskId;
        public string uniqueToken;
        public bool once = true;
        public UnityEvent onInteracted = new UnityEvent();
        public bool WasUsed { get; private set; }

        /// <summary>Desktop ray, touch button, XRI selectEntered veya Vuforia UI ayni girisi kullanir.</summary>
        public virtual void Interact()
        {
            if (!isActiveAndEnabled || (once && WasUsed)) return;
            if (mission != null && !string.IsNullOrEmpty(taskId) &&
                !mission.Report(taskId, string.IsNullOrEmpty(uniqueToken) ? GetInstanceID().ToString() : uniqueToken)) return;
            WasUsed = true;
            onInteracted.Invoke();
        }
        public void ResetInteraction() => WasUsed = false;
    }
}
