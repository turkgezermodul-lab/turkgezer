using UnityEngine;
using UnityEngine.Events;

namespace TurkGezer.Platform
{
    /// <summary>Vuforia found/lost veya AR Foundation tracking eventi icin SDK-bagimsiz giris.</summary>
    public sealed class ARTrackingBridge : MonoBehaviour
    {
        public GameObject contentRoot;
        public UnityEvent<bool> onTrackingChanged = new UnityEvent<bool>();
        public bool IsTracked { get; private set; }
        private void Awake() => TrackingLost();
        public void TrackingFound() => SetTracking(true);
        public void TrackingLost() => SetTracking(false);
        public void SetTracking(bool tracked)
        {
            IsTracked = tracked;
            // Bridge must live outside contentRoot so lost tracking doesn't disable the callback receiver.
            if (contentRoot != null && contentRoot != gameObject && !transform.IsChildOf(contentRoot.transform))
                contentRoot.SetActive(tracked);
            onTrackingChanged.Invoke(tracked);
        }
    }
}
