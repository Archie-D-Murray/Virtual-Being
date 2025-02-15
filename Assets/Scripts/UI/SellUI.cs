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
    public class SellUI : MonoBehaviour {
        private const string ZERO = "0";
        [SerializeField] private CanvasGroup _shopCanvas;
        [SerializeField] private GameObject _cropPrefab;
        [SerializeField] private Button _sell;

        private Dictionary<ItemType, (TMP_InputField input, TMP_Text money)> _slots = new Dictionary<ItemType, (TMP_InputField input, TMP_Text money)>();
        private Dictionary<ItemType, int> _sellData = new Dictionary<ItemType, int>();
        private Inventory _inventory;

        private void Start() {
            _inventory = FindFirstObjectByType<Inventory>();
            foreach (Crop crop in AssetServer.Instance.Crops.Values) {
                GameObject sellSlot = Instantiate(_cropPrefab, _shopCanvas.transform);
                TMP_InputField input = sellSlot.GetComponentInChildren<TMP_InputField>();
                _slots.Add(crop.YieldType, (input, sellSlot.GetComponentsInChildren<TMP_Text>().First(text => text.gameObject.HasComponent<ReadoutTag>())));
                input.onSubmit.AddListener((string value) => UpdateReadout(_slots[crop.YieldType].input, _slots[crop.YieldType].money, crop.YieldType, value));
                sellSlot.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<IconTag>()).sprite = crop.Icon;
            }
            _sell.onClick.AddListener(SellAll);
        }

        private void SellAll() {
            foreach (KeyValuePair<ItemType, int> kvp in _sellData) {
                _inventory.Money += AssetServer.Instance.Crops[kvp.Key].Value * kvp.Value;
            }
            _sellData.Clear();
            foreach ((TMP_InputField input, TMP_Text money) value in _slots.Values) {
                value.input.text = ZERO;
                value.money.text = ZERO;
            }
        }

        private void UpdateReadout(TMP_InputField input, TMP_Text money, ItemType type, string sellString) {
            int sellAmount = Mathf.Clamp(int.Parse(sellString), 0, _inventory[type]?.Count ?? 0);
            money.text = (AssetServer.Instance.Crops[type].Value * sellAmount).ToString();
            if (sellAmount > 0) {
                _sellData[type] = sellAmount;
            }
        }
    }
}