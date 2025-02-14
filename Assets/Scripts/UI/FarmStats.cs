using System.Collections.Generic;
using System.Linq;

using Crops;

using Items;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class FarmUI : MonoBehaviour {
        const string NONE = "0";
        [SerializeField] private CanvasGroup _farmCanvas;
        [SerializeField] private Crop[] _crops;
        [SerializeField] private GameObject _cropReadoutPrefab;

        private Dictionary<ItemType, TMP_Text> _cropReadouts;
        private Inventory _inventory;

        private void Start() {
            _farmCanvas = GetComponent<CanvasGroup>();
            _inventory = FindFirstObjectByType<Inventory>();
            _cropReadouts = new Dictionary<ItemType, TMP_Text>();
            foreach (Crop crop in _crops) {
                GameObject instance = Instantiate(_cropReadoutPrefab, _farmCanvas.transform);
                instance.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<Tags.UI.IconTag>()).sprite = crop.Icon;
                _cropReadouts.Add(crop.YieldType, instance.GetComponentInChildren<TMP_Text>());
            }
            UpdateReadouts();
            _inventory.OnItemChange += UpdateReadouts;
        }

        private void UpdateReadouts() {
            foreach (Crop crop in _crops) {
                _cropReadouts[crop.YieldType].text = _inventory[crop.YieldType]?.Count.ToString() ?? NONE;
            }
        }
    }
}