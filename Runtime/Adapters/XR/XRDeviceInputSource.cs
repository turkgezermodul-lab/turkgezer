using UnityEngine;
using UnityEngine.XR;

namespace TurkGezer.Platform
{
    /// <summary>Aktif OpenXR/Meta saglayicisindan veri okur. Saglayici kurulumu zorunludur.</summary>
    [DefaultExecutionOrder(-50)]
    public sealed class XRDeviceInputSource : ModuleInputSource
    {
        public XRNode controller = XRNode.RightHand;
        public Transform trackedHead;
        private Vector2 move;
        private bool previousTrigger;
        private bool pressed;
        public override Vector2 Move => move;
        public override Vector2 Look => Vector2.zero;
        public override bool InteractPressed => pressed;
        private void Update()
        {
            var hand = InputDevices.GetDeviceAtXRNode(controller);
            hand.TryGetFeatureValue(CommonUsages.primary2DAxis, out move);
            hand.TryGetFeatureValue(CommonUsages.triggerButton, out var trigger);
            pressed = trigger && !previousTrigger;
            previousTrigger = trigger;
            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (trackedHead != null && head.isValid)
            {
                if (head.TryGetFeatureValue(CommonUsages.devicePosition, out var position)) trackedHead.localPosition = position;
                if (head.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation)) trackedHead.localRotation = rotation;
            }
        }
        private void OnDisable() { move = Vector2.zero; previousTrigger = pressed = false; }
    }
}
