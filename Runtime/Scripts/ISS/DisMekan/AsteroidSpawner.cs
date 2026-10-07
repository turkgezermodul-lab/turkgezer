using System.Collections;
using UnityEngine;

namespace TurkGezer.Station
{
    public sealed class AsteroidSpawner : MonoBehaviour
    {
        public GameObject[] prefabs;
        public Transform target;
        [Min(.1f)] public float interval = 2;
        [Min(1)] public float lifetime = 15;
        [Min(0)] public float speed = 3;
        [Min(1)] public int maximumAlive = 15;
        private readonly System.Collections.Generic.List<GameObject> spawned = new System.Collections.Generic.List<GameObject>();
        private void OnEnable() => StartCoroutine(Spawn());
        private IEnumerator Spawn()
        {
            while (true)
            {
                spawned.RemoveAll(item => item == null);
                if (prefabs != null && prefabs.Length > 0 && spawned.Count < maximumAlive)
                {
                    var prefab = prefabs[Random.Range(0, prefabs.Length)];
                    if (prefab != null)
                    {
                        var item = Instantiate(prefab, transform.position, transform.rotation);
                        spawned.Add(item);
                        var body = item.GetComponent<Rigidbody>();
                        if (body == null) body = item.AddComponent<Rigidbody>();
                        body.useGravity = false;
                        Vector3 direction = target != null ? target.position - transform.position : transform.forward;
                        body.velocity = direction.normalized * speed;
                        Destroy(item, Mathf.Max(1, lifetime));
                    }
                }
                yield return new WaitForSeconds(Mathf.Max(.1f, interval));
            }
        }
        private void OnDisable()
        {
            StopAllCoroutines();
            foreach (var item in spawned) if (item != null) Destroy(item);
            spawned.Clear();
        }
    }
}
