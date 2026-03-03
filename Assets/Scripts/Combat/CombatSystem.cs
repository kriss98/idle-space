using System.Collections.Generic;
using UnityEngine;
using IdleSpace.Core;
using IdleSpace.Data;
using IdleSpace.Economy;
using IdleSpace.Pooling;

namespace IdleSpace.Combat
{
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField] private Ship ship;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private GameBalanceSettings balanceSettings;
        [SerializeField] private ObjectPool projectilePool;

        private readonly List<Enemy> trackedEnemies = new List<Enemy>(128);
        private float fireTimer;

        public void RegisterEnemy(Enemy enemy)
        {
            if (enemy == null || trackedEnemies.Contains(enemy))
            {
                return;
            }

            trackedEnemies.Add(enemy);
            enemy.OnEnemyRemoved += HandleEnemyRemoved;
        }

        public void ClearEnemies()
        {
            for (int i = trackedEnemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = trackedEnemies[i];
                if (enemy != null)
                {
                    enemy.OnEnemyRemoved -= HandleEnemyRemoved;
                }
            }

            trackedEnemies.Clear();
        }

        private void HandleEnemyRemoved(Enemy enemy)
        {
            enemy.OnEnemyRemoved -= HandleEnemyRemoved;
            trackedEnemies.Remove(enemy);
        }

        private void Update()
        {
            if (ship == null || projectilePool == null || balanceSettings == null)
            {
                return;
            }

            float fireRate = Mathf.Max(0.1f, balanceSettings.fireRate);
            fireTimer += Time.deltaTime;
            if (fireTimer < 1f / fireRate)
            {
                return;
            }

            fireTimer = 0f;
            Enemy target = GetNearestEnemy();
            if (target == null)
            {
                return;
            }

            double totalDps = weaponManager != null ? weaponManager.GetTotalDps() : 0d;
            if (totalDps <= 0d)
            {
                return;
            }

            float damagePerShot = (float)Mathf.Clamp((float)(totalDps / fireRate), 0f, 1_000_000f);
            SpawnProjectile(target, damagePerShot);
        }

        private Enemy GetNearestEnemy()
        {
            Enemy best = null;
            float bestDistance = float.MaxValue;
            for (int i = trackedEnemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = trackedEnemies[i];
                if (enemy == null || !enemy.IsAlive)
                {
                    trackedEnemies.RemoveAt(i);
                    continue;
                }

                float dist = enemy.DistanceToShip;
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    best = enemy;
                }
            }

            return best;
        }

        private void SpawnProjectile(Enemy target, float damage)
        {
            GameObject projectileGo = projectilePool.Get();
            if (projectileGo == null)
            {
                return;
            }

            projectileGo.transform.position = ship.transform.position;
            Vector3 dir = (target.transform.position - ship.transform.position).normalized;
            Projectile projectile = projectileGo.GetComponent<Projectile>();
            projectile.Setup(dir, balanceSettings.projectileSpeed, damage, balanceSettings.projectileLifetime, projectilePool);
        }
    }
}
