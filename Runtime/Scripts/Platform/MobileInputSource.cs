using UnityEngine;

namespace TurkGezer.Platform
{
    /// <summary>UI joystick, buton veya harici kontrolcu bu metotlari cagirabilir.</summary>
    public sealed class MobileInputSource : ModuleInputSource
    {
        private Vector2 move;
        private Vector2 look;
        private float vertical;
        private int interactFrame = -1;
        public override Vector2 Move => move;
        public override Vector2 Look => look;
        public override float Vertical => vertical;
        public override bool LookIsDelta => false;
        public override bool InteractPressed => interactFrame == Time.frameCount;
        public void SetMove(Vector2 value) => move = Vector2.ClampMagnitude(value, 1);
        public void SetLook(Vector2 value) => look = Vector2.ClampMagnitude(value, 1);
        public void SetVertical(float value) => vertical = Mathf.Clamp(value, -1, 1);
        public void PressInteract() => interactFrame = Time.frameCount;
        private void OnDisable() { move = look = Vector2.zero; vertical = 0; interactFrame = -1; }
    }
}
