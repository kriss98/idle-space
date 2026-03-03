using System.Collections.Generic;
using UnityEngine;

namespace IdleSpace.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 16;

        private readonly Queue<GameObject> queue = new Queue<GameObject>();

        public void Initialize(GameObject targetPrefab, int size)
        {
            prefab = targetPrefab;
            initialSize = Mathf.Max(1, size);
            Warm();
        }

        public void Warm()
        {
            if (prefab == null)
            {
                return;
            }

            for (int i = queue.Count; i < initialSize; i++)
            {
                CreateAndEnqueue();
            }
        }

        public GameObject Get()
        {
            if (prefab == null)
            {
                return null;
            }

            if (queue.Count == 0)
            {
                CreateAndEnqueue();
            }

            GameObject go = queue.Dequeue();
            go.SetActive(true);
            return go;
        }

        public void Release(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            go.SetActive(false);
            go.transform.SetParent(transform);
            queue.Enqueue(go);
        }

        private void CreateAndEnqueue()
        {
            GameObject created = Instantiate(prefab, transform);
            created.SetActive(false);
            queue.Enqueue(created);
        }
    }
}
