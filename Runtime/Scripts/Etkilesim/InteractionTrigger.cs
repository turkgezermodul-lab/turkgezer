using UnityEngine;

namespace TurkGezer.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class InteractionTrigger : MonoBehaviour
    {
        public ModuleInteractable target;
        private void Reset() { GetComponent<Collider>().isTrigger = true; target = GetComponent<ModuleInteractable>(); }
        private void OnTriggerEnter(Collider other)
        {
            if (target != null && other.GetComponentInParent<PlayerIdentity>() != null) target.Interact();
        }
    }
}
