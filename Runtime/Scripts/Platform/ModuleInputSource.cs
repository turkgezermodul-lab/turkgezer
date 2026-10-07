using UnityEngine;

namespace TurkGezer.Platform
{
    public abstract class ModuleInputSource : MonoBehaviour
    {
        public abstract Vector2 Move { get; }
        public abstract Vector2 Look { get; }
        public virtual float Vertical => 0;
        public virtual bool LookIsDelta => true;
        public abstract bool InteractPressed { get; }
    }
}
