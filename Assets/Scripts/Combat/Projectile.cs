using UnityEngine;
using IdleSpace.Pooling;

namespace IdleSpace.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        private float speed;
        private float damage;
        private float lifetime;
        private float age;
        private Vector3 direction;
        private ObjectPool pool;

        public void Setup(Vector3 fireDirection, float projectileSpeed, float projectileDamage, float projectileLifetime, ObjectPool sourcePool)
        {
            direction = fireDirection.normalized;
            speed = projectileSpeed;
            damage = projectileDamage;
            lifetime = projectileLifetime;
            age = 0f;
            pool = sourcePool;
        }

        private void Update()
        {
            transform.position += direction * (speed * Time.deltaTime);
            age += Time.deltaTime;
            if (age >= lifetime)
            {
                pool?.Release(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null || !enemy.IsAlive)
            {
                return;
            }

            enemy.TakeDamage(damage);
            pool?.Release(gameObject);
        }
    }
}
