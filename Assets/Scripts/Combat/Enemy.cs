using System;
using UnityEngine;
using IdleSpace.Core;
using IdleSpace.Economy;
using IdleSpace.Pooling;

namespace IdleSpace.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float contactDamage = 10f;

        private float currentHp;
        private double goldReward;
        private Ship ship;
        private EconomyManager economy;
        private ObjectPool pool;

        public bool IsAlive => gameObject.activeSelf && currentHp > 0f;
        public float DistanceToShip => ship == null ? float.MaxValue : Vector2.Distance(transform.position, ship.transform.position);

        public event Action<Enemy> OnEnemyDied;
        public event Action<Enemy> OnEnemyRemoved;

        public void Setup(float hp, double reward, Ship shipRef, EconomyManager economyManager, ObjectPool sourcePool)
        {
            currentHp = Mathf.Max(1f, hp);
            goldReward = Math.Max(0d, reward);
            ship = shipRef;
            economy = economyManager;
            pool = sourcePool;
        }

        private void Update()
        {
            if (!IsAlive || ship == null)
            {
                return;
            }

            Vector3 dir = (ship.transform.position - transform.position).normalized;
            transform.position += dir * (moveSpeed * Time.deltaTime);
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive || damage <= 0f)
            {
                return;
            }

            currentHp -= damage;
            if (currentHp <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            economy?.AddGold(goldReward, transform.position);
            OnEnemyDied?.Invoke(this);
            OnEnemyRemoved?.Invoke(this);
            pool?.Release(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Ship collidedShip = other.GetComponent<Ship>();
            if (collidedShip == null || collidedShip != ship)
            {
                return;
            }

            collidedShip.TakeDamage(contactDamage);
            OnEnemyRemoved?.Invoke(this);
            pool?.Release(gameObject);
        }
    }
}
