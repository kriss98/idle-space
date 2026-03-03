using UnityEngine;

namespace IdleSpace.Data
{
    [CreateAssetMenu(menuName = "IdleSpace/Weapons/Weapon Definition", fileName = "WeaponDefinition")]
    public class WeaponDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite icon;
        [TextArea] public string description;

        [Header("Economy")]
        [Min(1)] public double baseCost = 10d;
        [Min(1f)] public float costGrowth = 1.15f;

        [Header("DPS")]
        [Min(0.1)] public double baseDps = 1d;
        [Min(1f)] public float dpsGrowth = 1.09f;
        public bool multiplyByLevel = true;

        public double GetCostForLevel(int level)
        {
            int safeLevel = Mathf.Max(1, level);
            return baseCost * System.Math.Pow(costGrowth, safeLevel - 1);
        }

        public double GetDpsAtLevel(int level)
        {
            int safeLevel = Mathf.Max(0, level);
            if (safeLevel == 0)
            {
                return 0d;
            }

            double scaled = baseDps * System.Math.Pow(dpsGrowth, safeLevel - 1);
            return multiplyByLevel ? scaled * safeLevel : scaled;
        }
    }
}
