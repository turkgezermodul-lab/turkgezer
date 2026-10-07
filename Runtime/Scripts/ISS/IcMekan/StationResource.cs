using UnityEngine;
using UnityEngine.Events;

namespace TurkGezer.Station
{
    public sealed class StationResource : MonoBehaviour
    {
        public string resourceId = "oxygen";
        [Min(1)] public float capacity = 100;
        public float initialValue = 100;
        [Min(0)] public float consumptionPerSecond = .1f;
        public UnityEvent<float> onValueChanged = new UnityEvent<float>();
        public UnityEvent onDepleted = new UnityEvent();
        public float Value { get; private set; }
        private bool depleted;
        private void Awake() { Value = Mathf.Clamp(initialValue, 0, Mathf.Max(1, capacity)); }
        private void Update() { if (Value > 0 && consumptionPerSecond > 0) Change(-consumptionPerSecond * Time.deltaTime); }
        public void Change(float amount)
        {
            Value = Mathf.Clamp(Value + amount, 0, Mathf.Max(1, capacity));
            onValueChanged.Invoke(Value);
            if (Value <= 0 && !depleted) { depleted = true; onDepleted.Invoke(); }
            if (Value > 0) depleted = false;
        }
    }
}
