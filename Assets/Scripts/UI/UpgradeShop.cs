using System;
using System.Collections.Generic;

using Farms;

using Interaction;

using Items;

using Tags.UI;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public enum UpgradeType { HydrationDrain, HarvestAmount }
    [Serializable]
    public class Upgrade {
        public UpgradeType Type;
        public float Multiplier = 1.25f;
        public int MaxUpgrades = 4;
        public int CurrentUpgrades = 0;
        public int BaseCost = 100;
        public float IncreasePerLevel = 0.5f;
        public Sprite Icon;
        public String Description;

        // [HideInInspector]
        public Button Button;
        public Image Progress;
        public TMP_Text Readout;
        public TMP_Text Price;

        public int Cost => Mathf.RoundToInt(BaseCost * (1f + CurrentUpgrades * IncreasePerLevel));
    }

    public class UpgradeShop : MonoBehaviour {
        private const string MAX_UPGRADES = "Max Upgrades";
        private const string DEFAULT_VALUE = "100%";
        [SerializeField] private CanvasGroup _upgradeCanvas;
        [SerializeField] private GameObject _upgradePrefab;
        [SerializeField] private Upgrade[] _upgrades = new Upgrade[2];

        public Upgrade[] Upgrades => _upgrades;
        public Action OnUpgradeApply;

        private Inventory _inventory;
        private FarmUI _farmUI;
        private Dictionary<UpgradeType, int> _lookups = new Dictionary<UpgradeType, int>();

        private void Start() {
            _inventory = FindFirstObjectByType<Inventory>();
            _farmUI = FindFirstObjectByType<FarmUI>();
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
                foreach (TMP_Text text in instance.GetComponentsInChildren<TMP_Text>()) {
                    if (text.gameObject.HasComponent<ReadoutTag>()) {
                        upgrade.Readout = text;
                        upgrade.Readout.text = "100%";
                    } else if (text.gameObject.HasComponent<PriceTag>()) {
                        upgrade.Price = text;
                        upgrade.Price.text = upgrade.BaseCost.ToString();
                    } else {
                        text.text = upgrade.Description;
                    }
                }
                button.onClick.AddListener(() => Buy(upgrade));
                button.interactable = false;
            }
        }

        private void Buy(Upgrade upgrade) {
            _inventory.Money -= upgrade.Cost;
            switch (upgrade.Type) {
                case UpgradeType.HydrationDrain:
                    Farm.Instance.HydrationDrainMultiplier += upgrade.Multiplier;
                    InteractionManager.Instance.WateringManager.ReduceWaterCooldown(0.1f * upgrade.Multiplier);
                    upgrade.Readout.text = Farm.Instance.HydrationDrainMultiplier.ToString("0%");
                    break;
                case UpgradeType.HarvestAmount:
                    Farm.Instance.HarvestMultiplier += upgrade.Multiplier;
                    upgrade.Readout.text = Farm.Instance.HarvestMultiplier.ToString("0%");
                    break;
            }
            upgrade.CurrentUpgrades++;
            OnUpgradeApply?.Invoke();
            upgrade.Progress.fillAmount = (float)upgrade.CurrentUpgrades / (float)upgrade.MaxUpgrades;
            if (upgrade.CurrentUpgrades != upgrade.MaxUpgrades) {
                upgrade.Price.text = upgrade.Cost.ToString();
            } else {
                upgrade.Price.text = MAX_UPGRADES;
                upgrade.Button.interactable = false;
                EventSystem.current.SetSelectedGameObject(null);
                _farmUI.FinishedUpgrade();
            }
            UpdateUpgrades(_inventory.Money);
        }

        private void UpdateUpgrades(int money) {
            Debug.Log("Updated upgrades");
            foreach (Upgrade upgrade in _upgrades) {
                upgrade.Button.interactable = money >= upgrade.Cost && upgrade.CurrentUpgrades < upgrade.MaxUpgrades;
            }
        }
    }
}