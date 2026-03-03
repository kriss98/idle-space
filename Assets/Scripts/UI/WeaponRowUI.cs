using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IdleSpace.Audio;
using IdleSpace.Data;
using IdleSpace.Economy;

namespace IdleSpace.UI
{
    public class WeaponRowUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text dpsText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Button buyButton;

        private WeaponDefinition definition;
        private WeaponManager weaponManager;
        private EconomyManager economyManager;
        private AudioManager audioManager;

        public void Bind(WeaponDefinition def, WeaponManager wm, EconomyManager em, AudioManager am)
        {
            definition = def;
            weaponManager = wm;
            economyManager = em;
            audioManager = am;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(BuyPressed);
            Refresh();
        }

        public void Refresh()
        {
            if (definition == null)
            {
                return;
            }

            iconImage.sprite = definition.icon;
            nameText.text = definition.displayName;
            int level = weaponManager.GetLevel(definition);
            double dps = weaponManager.GetCurrentDps(definition);
            double cost = weaponManager.GetNextCost(definition);

            levelText.text = $"Lv. {level}";
            dpsText.text = $"DPS {NumberFormatter.Compact(dps)}";
            costText.text = NumberFormatter.Compact(cost);
            buyButton.interactable = economyManager.Gold >= cost;
        }

        private void BuyPressed()
        {
            weaponManager.TryPurchase(definition, economyManager, audioManager);
            Refresh();
        }
    }
}
