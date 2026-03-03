using System;
using System.Collections.Generic;

namespace IdleSpace.Data
{
    [Serializable]
    public class WeaponSaveEntry
    {
        public string weaponId;
        public int level;
    }

    [Serializable]
    public class SaveData
    {
        public double gold;
        public int currentWave = 1;
        public int lastClearedWave = 0;
        public float shipHp;
        public List<WeaponSaveEntry> weaponLevels = new List<WeaponSaveEntry>();
    }
}
