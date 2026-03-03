using UnityEngine;

namespace IdleSpace.Combat
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private float spawnPadding = 1.5f;

        public Vector3 GetSpawnPosition()
        {
            Camera cam = gameplayCamera != null ? gameplayCamera : Camera.main;
            if (cam == null)
            {
                return new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0f);
            }

            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            int edge = Random.Range(0, 4);

            switch (edge)
            {
                case 0: return new Vector3(-halfWidth - spawnPadding, Random.Range(-halfHeight, halfHeight), 0f);
                case 1: return new Vector3(halfWidth + spawnPadding, Random.Range(-halfHeight, halfHeight), 0f);
                case 2: return new Vector3(Random.Range(-halfWidth, halfWidth), halfHeight + spawnPadding, 0f);
                default: return new Vector3(Random.Range(-halfWidth, halfWidth), -halfHeight - spawnPadding, 0f);
            }
        }
    }
}
