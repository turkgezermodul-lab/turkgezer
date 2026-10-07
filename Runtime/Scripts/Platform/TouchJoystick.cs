using UnityEngine;
using UnityEngine.EventSystems;

namespace TurkGezer.Platform
{
    public sealed class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public MobileInputSource input;
        public RectTransform handle;
        public bool controlsLook;
        private int pointerId = int.MinValue;
        public void OnPointerDown(PointerEventData e)
        {
            if (pointerId != int.MinValue) return;
            pointerId = e.pointerId;
            OnDrag(e);
        }
        public void OnDrag(PointerEventData e)
        {
            if (e.pointerId != pointerId || input == null) return;
            var rect = (RectTransform)transform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, e.position, e.pressEventCamera, out var point)) return;
            Vector2 center = rect.rect.center;
            float radius = Mathf.Max(1, Mathf.Min(rect.rect.width, rect.rect.height) * .5f);
            Vector2 value = Vector2.ClampMagnitude((point - center) / radius, 1);
            if (handle != null) handle.anchoredPosition = value * radius * .65f;
            if (controlsLook) input.SetLook(value); else input.SetMove(value);
        }
        public void OnPointerUp(PointerEventData e) { if (e.pointerId == pointerId) Release(); }
        private void OnDisable() => Release();
        private void Release()
        {
            pointerId = int.MinValue;
            if (handle != null) handle.anchoredPosition = Vector2.zero;
            if (input == null) return;
            if (controlsLook) input.SetLook(Vector2.zero); else input.SetMove(Vector2.zero);
        }
    }
}
