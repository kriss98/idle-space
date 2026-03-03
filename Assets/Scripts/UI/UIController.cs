using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IdleSpace.Audio;
using IdleSpace.Core;
using IdleSpace.Data;
using IdleSpace.Economy;

namespace IdleSpace.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private Ship ship;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private AudioManager audioManager;

        [Header("Top HUD")]
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private Image hpFill;
        [SerializeField] private TMP_Text goldText;

        [Header("Weapons")]
        [SerializeField] private Transform weaponListRoot;
        [SerializeField] private WeaponRowUI weaponRowPrefab;

        private readonly List<WeaponRowUI> rows = new List<WeaponRowUI>();

        private void Start()
        {
            BuildWeaponRows();
            ship.OnHpChanged += HandleHpChanged;
            waveManager.OnWaveChanged += HandleWaveChanged;
            economyManager.OnGoldChanged += HandleGoldChanged;
            weaponManager.OnWeaponsChanged += RefreshRows;

            HandleHpChanged(ship.CurrentHp, ship.MaxHp);
            HandleGoldChanged(economyManager.Gold);
            HandleWaveChanged(waveManager.CurrentWave, waveManager.LastClearedWave);
        }

        private void OnDestroy()
        {
            ship.OnHpChanged -= HandleHpChanged;
            waveManager.OnWaveChanged -= HandleWaveChanged;
            economyManager.OnGoldChanged -= HandleGoldChanged;
            weaponManager.OnWeaponsChanged -= RefreshRows;
        }

        private void BuildWeaponRows()
        {
            IReadOnlyList<WeaponDefinition> defs = weaponManager.Definitions;
            for (int i = 0; i < defs.Count; i++)
            {
                WeaponRowUI row = Instantiate(weaponRowPrefab, weaponListRoot);
                row.Bind(defs[i], weaponManager, economyManager, audioManager);
                rows.Add(row);
            }
        }

        private void HandleWaveChanged(int currentWave, int _)
        {
            waveText.text = $"Wave {currentWave}";
        }

        private void HandleHpChanged(float currentHp, float maxHp)
        {
            hpFill.fillAmount = maxHp <= 0f ? 0f : currentHp / maxHp;
        }

        private void HandleGoldChanged(double gold)
        {
            goldText.text = NumberFormatter.Compact(gold);
            RefreshRows();
        }

        private void RefreshRows()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Refresh();
            }
        }
    }
}
