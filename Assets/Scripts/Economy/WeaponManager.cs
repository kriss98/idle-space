using System;
using System.Collections.Generic;
using UnityEngine;
using IdleSpace.Audio;
using IdleSpace.Data;

namespace IdleSpace.Economy
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private List<WeaponDefinition> weaponDefinitions = new List<WeaponDefinition>();

        private readonly Dictionary<string, int> levelsById = new Dictionary<string, int>();

        public event Action OnWeaponsChanged;

        public IReadOnlyList<WeaponDefinition> Definitions => weaponDefinitions;

        public void Initialize(IReadOnlyList<WeaponSaveEntry> saveEntries)
        {
            levelsById.Clear();
            for (int i = 0; i < weaponDefinitions.Count; i++)
            {
                WeaponDefinition def = weaponDefinitions[i];
                int level = 0;
                for (int j = 0; j < saveEntries.Count; j++)
                {
                    if (saveEntries[j].weaponId == def.id)
                    {
                        level = Mathf.Max(0, saveEntries[j].level);
                        break;
                    }
                }

                levelsById[def.id] = level;
            }

            OnWeaponsChanged?.Invoke();
        }

        public int GetLevel(WeaponDefinition definition)
        {
            return levelsById.TryGetValue(definition.id, out int level) ? level : 0;
        }

        public double GetCurrentDps(WeaponDefinition definition)
        {
            return definition.GetDpsAtLevel(GetLevel(definition));
        }

        public double GetNextCost(WeaponDefinition definition)
        {
            return definition.GetCostForLevel(GetLevel(definition) + 1);
        }

        public bool TryPurchase(WeaponDefinition definition, EconomyManager economyManager, AudioManager audioManager)
        {
            double cost = GetNextCost(definition);
            if (!economyManager.TrySpend(cost))
            {
                audioManager?.PlayClick();
                return false;
            }

            levelsById[definition.id] = GetLevel(definition) + 1;
            audioManager?.PlayBuy();
            OnWeaponsChanged?.Invoke();
            return true;
        }

        public double GetTotalDps()
        {
            double total = 0d;
            for (int i = 0; i < weaponDefinitions.Count; i++)
            {
                total += GetCurrentDps(weaponDefinitions[i]);
            }

            return total;
        }

        public List<WeaponSaveEntry> ToSaveEntries()
        {
            List<WeaponSaveEntry> entries = new List<WeaponSaveEntry>(weaponDefinitions.Count);
            for (int i = 0; i < weaponDefinitions.Count; i++)
            {
                WeaponDefinition def = weaponDefinitions[i];
                entries.Add(new WeaponSaveEntry
                {
                    weaponId = def.id,
                    level = GetLevel(def)
                });
            }

            return entries;
        }
    }
}
