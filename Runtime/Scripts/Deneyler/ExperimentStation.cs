using TurkGezer.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace TurkGezer.Experiments
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class ExperimentStation : ModuleInteractable
    {
        public ExperimentDefinition experiment;
        public UnityEvent<string> onInformation = new UnityEvent<string>();
        private static AudioSource activeNarration;

        public override void Interact()
        {
            bool before = WasUsed;
            base.Interact();
            if (!WasUsed || (once && before) || experiment == null) return;
            onInformation.Invoke(experiment.title + "\n" + experiment.explanation);
            var source = GetComponent<AudioSource>();
            if (activeNarration != null && activeNarration != source) activeNarration.Stop();
            if (experiment.narration != null)
            {
                source.clip = experiment.narration;
                source.Play();
                activeNarration = source;
            }
        }
        private void OnDisable()
        {
            var source = GetComponent<AudioSource>();
            if (source != null) source.Stop();
            if (activeNarration == source) activeNarration = null;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => activeNarration = null;
    }
}
