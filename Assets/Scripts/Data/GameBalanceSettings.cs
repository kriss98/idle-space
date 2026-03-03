using UnityEngine;

namespace IdleSpace.Data
{
    [CreateAssetMenu(menuName = "IdleSpace/Balance/Game Balance Settings", fileName = "GameBalanceSettings")]
    public class GameBalanceSettings : ScriptableObject
    {
        [Header("Wave Scaling")]
        [Min(1f)] public float hpGrowthPerWave = 1.15f;
        [Min(1f)] public float goldGrowthPerWave = 1.13f;
        [Min(1f)] public float baseEnemyHp = 10f;
        [Min(0f)] public float baseEnemyGold = 5f;
        [Min(1)] public int enemiesPerWaveBase = 6;
        [Min(1)] public int enemiesPerWaveGrowthInterval = 5;
        [Min(0)] public int enemiesPerWaveGrowthAmount = 1;

        [Header("Boss")]
        [Min(1)] public int bossEveryNWaves = 10;
        [Min(1f)] public float bossHpMultiplierBase = 30f;
        [Min(1f)] public float bossHpMultiplierGrowth = 1.05f;
        [Min(1f)] public float bossGoldMultiplier = 10f;

        [Header("Ship/Combat")]
        [Min(1f)] public float shipMaxHp = 100f;
        [Min(1f)] public float projectileSpeed = 12f;
        [Min(0.1f)] public float projectileLifetime = 3f;
        [Min(0.1f)] public float fireRate = 5f;

        public bool IsBossWave(int wave)
        {
            return wave > 0 && wave % bossEveryNWaves == 0;
        }

        public int GetEnemiesPerWave(int wave)
        {
            if (IsBossWave(wave))
            {
                return 1;
            }

            int extra = Mathf.Max(0, (wave - 1) / Mathf.Max(1, enemiesPerWaveGrowthInterval)) * enemiesPerWaveGrowthAmount;
            return Mathf.Max(1, enemiesPerWaveBase + extra);
        }

        public float GetEnemyHp(int wave)
        {
            float hp = baseEnemyHp * Mathf.Pow(hpGrowthPerWave, Mathf.Max(0, wave - 1));
            if (IsBossWave(wave))
            {
                int bossIndex = Mathf.Max(1, wave / Mathf.Max(1, bossEveryNWaves));
                hp *= bossHpMultiplierBase * Mathf.Pow(bossHpMultiplierGrowth, bossIndex - 1);
            }

            return hp;
        }

        public float GetEnemyGold(int wave)
        {
            float gold = baseEnemyGold * Mathf.Pow(goldGrowthPerWave, Mathf.Max(0, wave - 1));
            if (IsBossWave(wave))
            {
                gold *= bossGoldMultiplier;
            }

            return gold;
        }
    }
}
