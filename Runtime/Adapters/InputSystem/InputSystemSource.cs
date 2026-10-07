using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace TurkGezer.Platform
{
    public sealed class InputSystemSource : ModuleInputSource
    {
        public InputActionReference move;
        public InputActionReference look;
        public InputActionReference interact;
        public InputActionReference vertical;
        public bool lookIsDelta = true;
        private readonly HashSet<InputAction> owned = new HashSet<InputAction>();
        public override Vector2 Move => move != null && move.action.enabled ? move.action.ReadValue<Vector2>() : Vector2.zero;
        public override Vector2 Look => look != null && look.action.enabled ? look.action.ReadValue<Vector2>() : Vector2.zero;
        public override float Vertical => vertical != null && vertical.action.enabled ? vertical.action.ReadValue<float>() : 0;
        public override bool InteractPressed => interact != null && interact.action.enabled && interact.action.WasPressedThisFrame();
        public override bool LookIsDelta => lookIsDelta;
        private void OnEnable()
        {
            foreach (var reference in new[] { move, look, interact, vertical })
                if (reference != null && reference.action != null && !reference.action.enabled)
                { reference.action.Enable(); owned.Add(reference.action); }
        }
        private void OnDisable() { foreach (var action in owned) action.Disable(); owned.Clear(); }
    }
}
