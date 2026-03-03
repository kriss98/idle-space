using System;
using UnityEngine;
using IdleSpace.Data;

namespace IdleSpace.Core
{
    public class SaveManager : MonoBehaviour
    {
        private const string SaveKey = "idle_space_save_v1";

        [SerializeField] private float autoSaveInterval = 15f;

        private SaveData pendingData;
        private float timer;

        public event Action<SaveData> OnLoaded;

        public SaveData Load(float defaultShipHp)
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                SaveData fresh = new SaveData { shipHp = defaultShipHp };
                OnLoaded?.Invoke(fresh);
                return fresh;
            }

            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            SaveData loaded = string.IsNullOrEmpty(json) ? new SaveData() : JsonUtility.FromJson<SaveData>(json);
            loaded ??= new SaveData();
            if (loaded.shipHp <= 0f)
            {
                loaded.shipHp = defaultShipHp;
            }

            if (loaded.currentWave <= 0)
            {
                loaded.currentWave = 1;
            }

            OnLoaded?.Invoke(loaded);
            return loaded;
        }

        public void SetDirty(SaveData data)
        {
            pendingData = data;
            SaveNow();
        }

        public void SaveNow()
        {
            if (pendingData == null)
            {
                return;
            }

            string json = JsonUtility.ToJson(pendingData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void ClearSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        private void Update()
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= autoSaveInterval)
            {
                timer = 0f;
                SaveNow();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveNow();
            }
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }
    }
}
