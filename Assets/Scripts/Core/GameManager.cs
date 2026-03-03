using UnityEngine;
using IdleSpace.Audio;
using IdleSpace.Data;
using IdleSpace.Economy;

namespace IdleSpace.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameBalanceSettings balanceSettings;
        [SerializeField] private Ship ship;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private AudioManager audioManager;

        private SaveData saveData;

        private void Awake()
        {
            ship.ConfigureMaxHp(balanceSettings.shipMaxHp);
            saveData = saveManager.Load(balanceSettings.shipMaxHp);

            ship.ResetHp(saveData.shipHp <= 0f ? balanceSettings.shipMaxHp : saveData.shipHp);
            economyManager.SetGold(saveData.gold);
            weaponManager.Initialize(saveData.weaponLevels);
            waveManager.Initialize(saveData.currentWave, saveData.lastClearedWave);

            ship.OnShipDefeated += OnShipDefeated;
            waveManager.OnWaveChanged += OnWaveChanged;
            waveManager.OnWaveCleared += Persist;
            economyManager.OnGoldChanged += HandleGoldChanged;
            weaponManager.OnWeaponsChanged += Persist;
            ship.OnHpChanged += HandleHpChanged;
        }

        private void Start()
        {
            waveManager.StartWaveLoop();
        }

        private void OnDestroy()
        {
            ship.OnShipDefeated -= OnShipDefeated;
            waveManager.OnWaveChanged -= OnWaveChanged;
            waveManager.OnWaveCleared -= Persist;
            economyManager.OnGoldChanged -= HandleGoldChanged;
            weaponManager.OnWeaponsChanged -= Persist;
            ship.OnHpChanged -= HandleHpChanged;
        }

        private void OnShipDefeated()
        {
            ship.ResetHp(balanceSettings.shipMaxHp);
            waveManager.FailCurrentWave();
            Persist();
        }

        private void OnWaveChanged(int currentWave, int lastClearedWave)
        {
            saveData.currentWave = currentWave;
            saveData.lastClearedWave = lastClearedWave;
            Persist();
        }

        private void HandleGoldChanged(double _) => Persist();

        private void HandleHpChanged(float _, float __) => Persist();

        private void Persist()
        {
            saveData.gold = economyManager.Gold;
            saveData.shipHp = ship.CurrentHp;
            saveData.weaponLevels = weaponManager.ToSaveEntries();
            saveData.currentWave = waveManager.CurrentWave;
            saveData.lastClearedWave = waveManager.LastClearedWave;
            saveManager.SetDirty(saveData);
        }

        // TODO: Prestige reset path that keeps meta-upgrades.
        // TODO: Offline progress reward simulation.
        // TODO: Multiple enemy archetypes and resistances.
        public AudioManager GetAudioManager() => audioManager;
    }
}
