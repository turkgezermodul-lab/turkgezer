using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TurkGezer.Menu
{
    public sealed class SceneNavigator : MonoBehaviour
    {
        public UnityEvent<string> onError = new UnityEvent<string>();
        private bool loading;
        public void Load(string sceneNameOrPath)
        {
            if (loading) return;
            if (string.IsNullOrWhiteSpace(sceneNameOrPath) || !Application.CanStreamedLevelBeLoaded(sceneNameOrPath))
            { onError.Invoke("Sahne Build Settings'e eklenmemis: " + sceneNameOrPath); return; }
            StartCoroutine(LoadRoutine(sceneNameOrPath));
        }
        private IEnumerator LoadRoutine(string path)
        {
            loading = true;
            Time.timeScale = 1;
            var operation = SceneManager.LoadSceneAsync(path);
            if (operation != null) yield return operation;
            loading = false;
        }
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
