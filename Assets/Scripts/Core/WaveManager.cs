using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleSpace.Combat;
using IdleSpace.Data;
using IdleSpace.Economy;
using IdleSpace.Pooling;

namespace IdleSpace.Core
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private GameBalanceSettings balance;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private ObjectPool enemyPool;
        [SerializeField] private Ship ship;
        [SerializeField] private EconomyManager economy;
        [SerializeField] private CombatSystem combatSystem;
        [SerializeField] private float spawnInterval = 0.3f;

        private readonly List<Enemy> activeEnemies = new List<Enemy>(128);
        private Coroutine runningWave;

        public int CurrentWave { get; private set; } = 1;
        public int LastClearedWave { get; private set; }

        public event Action<int, int> OnWaveChanged;
        public event Action OnWaveCleared;

        public void Initialize(int currentWave, int lastClearedWave)
        {
            CurrentWave = Mathf.Max(1, currentWave);
            LastClearedWave = Mathf.Max(0, lastClearedWave);
            OnWaveChanged?.Invoke(CurrentWave, LastClearedWave);
        }

        public void StartWaveLoop()
        {
            if (runningWave != null)
            {
                StopCoroutine(runningWave);
            }

            runningWave = StartCoroutine(RunWave());
        }

        private IEnumerator RunWave()
        {
            ClearAllEnemies();
            int count = balance.GetEnemiesPerWave(CurrentWave);
            float hp = balance.GetEnemyHp(CurrentWave);
            float gold = balance.GetEnemyGold(CurrentWave);

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(hp, gold);
                yield return new WaitForSeconds(spawnInterval);
            }

            while (activeEnemies.Count > 0)
            {
                yield return null;
            }

            LastClearedWave = Mathf.Max(LastClearedWave, CurrentWave);
            CurrentWave += 1;
            OnWaveChanged?.Invoke(CurrentWave, LastClearedWave);
            OnWaveCleared?.Invoke();
            runningWave = StartCoroutine(RunWave());
        }

        public void FailCurrentWave()
        {
            if (runningWave != null)
            {
                StopCoroutine(runningWave);
            }

            CurrentWave = Mathf.Max(1, LastClearedWave == 0 ? 1 : LastClearedWave);
            OnWaveChanged?.Invoke(CurrentWave, LastClearedWave);
            ClearAllEnemies();
            StartWaveLoop();
        }

        private void SpawnEnemy(float hp, double gold)
        {
            GameObject enemyGo = enemyPool.Get();
            enemyGo.transform.position = enemySpawner.GetSpawnPosition();
            Enemy enemy = enemyGo.GetComponent<Enemy>();
            enemy.Setup(hp, gold, ship, economy, enemyPool);
            enemy.OnEnemyRemoved += HandleEnemyRemoved;
            activeEnemies.Add(enemy);
            combatSystem.RegisterEnemy(enemy);

            if (balance.IsBossWave(CurrentWave))
            {
                enemyGo.transform.localScale = Vector3.one * 2f;
            }
            else
            {
                enemyGo.transform.localScale = Vector3.one;
            }
        }

        private void HandleEnemyRemoved(Enemy enemy)
        {
            enemy.OnEnemyRemoved -= HandleEnemyRemoved;
            activeEnemies.Remove(enemy);
        }

        public void ClearAllEnemies()
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = activeEnemies[i];
                if (enemy != null)
                {
                    enemy.OnEnemyRemoved -= HandleEnemyRemoved;
                    enemyPool.Release(enemy.gameObject);
                }
            }

            activeEnemies.Clear();
            combatSystem.ClearEnemies();
        }
    }
}
