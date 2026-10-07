using TurkGezer.Interaction;
using UnityEngine;

namespace TurkGezer.Platform
{
    /// <summary>XRI Select Entered/Activated UnityEvent'ine Select() baglayin. SDK cekirdegin zorunlu bagimliligi degildir.</summary>
    public sealed class XRInteractionBridge : MonoBehaviour
    {
        public ModuleInteractable target;
        public void Select() { if (target != null) target.Interact(); }
    }
}
