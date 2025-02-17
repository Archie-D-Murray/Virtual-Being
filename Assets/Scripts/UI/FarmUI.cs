using System;
using System.Collections.Generic;
using System.Linq;

using Crops;

using Farms;

using Items;

using Tags.UI;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class FarmUI : MonoBehaviour {
        const string NONE = "0";
        [SerializeField] private CanvasGroup _farmCanvas;
        [SerializeField] private GameObject _cropReadoutPrefab;
        [SerializeField] private GameObject _upgradeReadoutPrefab;

        private Dictionary<ItemType, TMP_Text> _cropReadouts;
        private Dictionary<UpgradeType, TMP_Text> _upgradeReadouts;
        private Inventory _inventory;

        private void Start() {
            _inventory = Farm.Instance.Inventory;
            _cropReadouts = new Dictionary<ItemType, TMP_Text>();
            _upgradeReadouts = new Dictionary<UpgradeType, TMP_Text>();
            foreach (Crop crop in AssetServer.Instance.Crops.Values) {
                GameObject instance = Instantiate(_cropReadoutPrefab, _farmCanvas.transform);
                instance.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<Tags.UI.IconTag>()).sprite = crop.Icon;
                _cropReadouts.Add(crop.YieldType, instance.GetComponentInChildren<TMP_Text>());
            }
            UpdateReadouts();
            _inventory.OnItemChange += UpdateReadouts;
        }

        private void UpdateReadouts() {
            foreach (ItemType type in AssetServer.Instance.Crops.Keys) {
                _cropReadouts[type].text = _inventory[type]?.Count.ToString() ?? NONE;
            }
        }
    }
}