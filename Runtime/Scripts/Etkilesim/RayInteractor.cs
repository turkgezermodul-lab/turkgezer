using TurkGezer.Platform;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TurkGezer.Interaction
{
    public sealed class RayInteractor : MonoBehaviour
    {
        public ModuleInputSource input;
        public Transform rayOrigin;
        [Min(.1f)] public float distance = 5;
        public LayerMask layers = ~0;
        private void Update()
        {
            if (input != null && input.isActiveAndEnabled && input.InteractPressed) Interact();
        }
        public void Interact()
        {
            if (rayOrigin == null || Time.timeScale == 0) return;
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out var hit, distance, layers, QueryTriggerInteraction.Collide))
            {
                var target = hit.collider.GetComponentInParent<ModuleInteractable>();
                if (target != null) target.Interact();
            }
        }
    }
}
