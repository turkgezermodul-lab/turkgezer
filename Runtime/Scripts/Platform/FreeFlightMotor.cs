using UnityEngine;

namespace TurkGezer.Platform
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FreeFlightMotor : MonoBehaviour
    {
        public ModuleInputSource input;
        public Transform view;
        [Min(0)] public float speed = 3;
        [Min(0)] public float lookSensitivity = 2;
        public bool rotateView = true;
        public bool captureDesktopCursor = true;
        private Rigidbody body;
        private float pitch;
        private CursorLockMode originalLock;
        private bool originalVisibility;
        private bool captured;
        private void Awake() { body = GetComponent<Rigidbody>(); body.useGravity = false; body.freezeRotation = true; }
        private void OnEnable()
        {
            if (!captureDesktopCursor || Application.isMobilePlatform) return;
            originalLock = Cursor.lockState;
            originalVisibility = Cursor.visible;
            captured = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void Update()
        {
            if (input == null || !input.isActiveAndEnabled || view == null || !rotateView || Time.timeScale == 0) return;
            float factor = lookSensitivity * (input.LookIsDelta ? 1 : Time.deltaTime * 60);
            Vector2 look = input.Look * factor;
            transform.Rotate(0, look.x, 0);
            pitch = Mathf.Clamp(pitch - look.y, -85, 85);
            view.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
        private void FixedUpdate()
        {
            if (input == null || !input.isActiveAndEnabled || view == null) { body.velocity = Vector3.zero; return; }
            Vector2 move = input.Move;
            Vector3 direction = view.forward * move.y + view.right * move.x + Vector3.up * input.Vertical;
            body.velocity = Vector3.ClampMagnitude(direction, 1) * speed;
        }
        private void OnDisable()
        {
            if (body != null && !body.isKinematic) body.velocity = Vector3.zero;
            if (!captured) return;
            Cursor.lockState = originalLock;
            Cursor.visible = originalVisibility;
            captured = false;
        }
    }
}
