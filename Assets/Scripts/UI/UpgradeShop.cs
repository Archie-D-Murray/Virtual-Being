using System;
using System.Collections.Generic;

using Farms;

using Items;

using Tags.UI;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public enum UpgradeType { GrowthTime, HarvestAmount }
    [Serializable]
    public class Upgrade {
        public UpgradeType Type;
        public float Multiplier = 1.25f;
        public int MaxUpgrades = 4;
        public int CurrentUpgrades = 0;
        public int BaseCost = 100;
        public float IncreasePerLevel = 0.5f;
        public Sprite Icon;

        // [HideInInspector]
        public Button Button;
        public Image Progress;
        public TMP_Text Text;

        public int Cost => Mathf.RoundToInt(BaseCost * (1f + CurrentUpgrades * IncreasePerLevel));
    }

    public class UpgradeShop : MonoBehaviour {
        private const string ZERO = "0";
        [SerializeField] private CanvasGroup _upgradeCanvas;
        [SerializeField] private GameObject _upgradePrefab;
        [SerializeField] private Upgrade[] _upgrades = new Upgrade[2];

        private Inventory _inventory;
        private Dictionary<UpgradeType, int> _lookups = new Dictionary<UpgradeType, int>();

        private void Start() {
            _inventory = FindFirstObjectByType<Inventory>();
            _inventory.OnMoneyChange += UpdateUpgrades;
            for (int i = 0; i < _upgrades.Length; i++) {
                Upgrade upgrade = _upgrades[i];
                _lookups.Add(upgrade.Type, i);
                GameObject instance = Instantiate(_upgradePrefab, _upgradeCanvas.transform);
                instance.GetComponentInChildren<TMP_Text>().text = upgrade.BaseCost.ToString();
                foreach (Image image in instance.GetComponentsInChildren<Image>()) {
                    if (image.gameObject.HasComponent<IconTag>()) {
                        image.sprite = upgrade.Icon;
                    } else if (image.gameObject.HasComponent<ReadoutTag>()) {
                        upgrade.Progress = image;
                        image.fillAmount = 0;
                    }
                }
                Button button = instance.GetComponentInChildren<Button>();
                upgrade.Button = button;
                upgrade.Text = button.GetComponentInChildren<TMP_Text>();
                upgrade.Text.text = upgrade.BaseCost.ToString();
                button.onClick.AddListener(() => Buy(upgrade));
                button.interactable = false;
            }
        }

        private void Buy(Upgrade upgrade) {
            _inventory.Money -= upgrade.Cost;
            switch (upgrade.Type) {
                case UpgradeType.GrowthTime:
                    Farm.Instance.GrowthSpeedMultiplier += upgrade.Multiplier;
                    break;
                case UpgradeType.HarvestAmount:
                    Farm.Instance.HarvestMultiplier += upgrade.Multiplier;
                    break;
            }
            upgrade.CurrentUpgrades++;
            upgrade.Progress.fillAmount = (float)upgrade.CurrentUpgrades / (float)upgrade.MaxUpgrades;
            if (upgrade.CurrentUpgrades < upgrade.MaxUpgrades) {
                upgrade.Text.text = upgrade.Cost.ToString();
            } else {
                upgrade.Text.text = ZERO;
            }
        }

        private void UpdateUpgrades(int money) {
            Debug.Log("Updated upgrades");
            foreach (Upgrade upgrade in _upgrades) {
                upgrade.Button.interactable = money > upgrade.Cost || upgrade.CurrentUpgrades == upgrade.MaxUpgrades;
            }
        }
    }
}