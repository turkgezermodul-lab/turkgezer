using TurkGezer.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TurkGezer.Platform
{
    /// <summary>XRI 2.5 el/poke/ray secimini ortak etkilesime aktarir. Cekirdekte XRI zorunlu degildir.</summary>
    public sealed class XRModuleInteractable : XRBaseInteractable
    {
        public ModuleInteractable target;
        protected override void Awake()
        {
            base.Awake();
            if (target == null) target = GetComponent<ModuleInteractable>();
        }
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (target != null) target.Interact();
        }
    }
}
