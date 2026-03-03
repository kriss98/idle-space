#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using IdleSpace.Data;

namespace IdleSpace.Editor
{
    public static class BalanceAssetCreator
    {
        private const string BalanceDir = "Assets/ScriptableObjects/Balance";

        [MenuItem("IdleSpace/Create Default Balance Assets")]
        public static void CreateDefaults()
        {
            if (!AssetDatabase.IsValidFolder(BalanceDir))
            {
                Directory.CreateDirectory(BalanceDir);
                AssetDatabase.Refresh();
            }

            CreateBalanceSettings();
            CreateWeaponDefinitions();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Default balance assets created/updated.");
        }

        private static void CreateBalanceSettings()
        {
            string path = $"{BalanceDir}/GameBalanceSettings.asset";
            GameBalanceSettings asset = AssetDatabase.LoadAssetAtPath<GameBalanceSettings>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<GameBalanceSettings>();
                AssetDatabase.CreateAsset(asset, path);
            }

            EditorUtility.SetDirty(asset);
        }

        private static void CreateWeaponDefinitions()
        {
            const double baseCost1 = 10d;
            const double baseDps1 = 1d;
            const double k = 12d;
            const double m = 7d;

            for (int i = 0; i < 10; i++)
            {
                int index = i + 1;
                string path = $"{BalanceDir}/WeaponDefinition_{index:00}.asset";
                WeaponDefinition def = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(path);
                if (def == null)
                {
                    def = ScriptableObject.CreateInstance<WeaponDefinition>();
                    AssetDatabase.CreateAsset(def, path);
                }

                def.id = $"weapon_{index:00}";
                def.displayName = $"Weapon {index}";
                def.baseCost = baseCost1 * System.Math.Pow(k, i);
                def.costGrowth = 1.15f;
                def.baseDps = baseDps1 * System.Math.Pow(m, i);
                def.dpsGrowth = 1.09f;
                def.multiplyByLevel = true;
                def.description = "Auto-fire weapon module.";
                EditorUtility.SetDirty(def);
            }
        }
    }
}
#endif
