using UnityEngine;

namespace TurkGezer.Platform
{
    public sealed class LegacyDesktopInput : ModuleInputSource
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        public override Vector2 Move => new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        public override Vector2 Look => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        public override float Vertical => (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftControl) ? 1 : 0);
        public override bool InteractPressed => Input.GetKeyDown(KeyCode.E);
#else
        public override Vector2 Move => Vector2.zero;
        public override Vector2 Look => Vector2.zero;
        public override bool InteractPressed => false;
        private void Start() => Debug.LogWarning("LegacyDesktopInput: Active Input Handling=Both veya Input Manager gerekli. Yeni Input System icin InputSystemSource kullanin.", this);
#endif
    }
}
