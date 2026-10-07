using UnityEngine;

namespace TurkGezer.Experiments
{
    [CreateAssetMenu(menuName = "TurkGezer/Bilimsel Deney", fileName = "YeniDeney")]
    public sealed class ExperimentDefinition : ScriptableObject
    {
        public string experimentId;
        public string title;
        [TextArea(3, 10)] public string explanation;
        public string sourceUrl;
        public AudioClip narration;
        public Sprite cardImage;
        public GameObject modelPrefab;
    }
}
