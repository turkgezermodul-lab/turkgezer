using UnityEngine;
using UnityEngine.UI;

namespace TurkGezer.Menu
{
    public sealed class AudioSettings : MonoBehaviour
    {
        public Slider volumeSlider;
        private const string Key = "TurkGezer.AudioVolume";
        private void Start()
        {
            float volume = PlayerPrefs.GetFloat(Key, 1);
            AudioListener.volume = volume;
            if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(volume);
        }
        public void SetVolume(float value)
        {
            float volume = Mathf.Clamp01(value);
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat(Key, volume);
        }
        private void OnDisable() => PlayerPrefs.Save();
    }
}
