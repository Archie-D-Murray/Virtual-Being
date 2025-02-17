using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Crops;

using Farms;

using Items;

using Tags.UI;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Utilities;

namespace UI {
    public class FarmUI : MonoBehaviour {
        const string NONE = "0";
        [SerializeField] private CanvasGroup _farmCanvas;
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private CanvasGroup _finishCanvas;
        [SerializeField] private GameObject _cropReadoutPrefab;
        [SerializeField] private GameObject _upgradeReadoutPrefab;
        [SerializeField] private int _plots = 6;
        [SerializeField] private int _currentPlots = 0;
        [SerializeField] private int _finishedUpgrades = 0;
        [SerializeField] private Image _fader;

        private Dictionary<ItemType, TMP_Text> _cropReadouts;
        private Dictionary<UpgradeType, TMP_Text> _upgradeReadouts;
        private TMP_Text _moneyReadout;
        private Inventory _inventory;
        private UpgradeShop _shop;
        private bool _shownFinish = false;

        private void Start() {
            _canvas = GetComponent<CanvasGroup>();
            _canvas.FadeCanvas(0.0f, true, this);
            _inventory = Farm.Instance.Inventory;
            _plots = FindObjectsOfType<Plot>(true).Length;
            _shop = GetComponent<UpgradeShop>();
            _moneyReadout = GetComponentInChildren<TMP_Text>();
            _cropReadouts = new Dictionary<ItemType, TMP_Text>();
            _upgradeReadouts = new Dictionary<UpgradeType, TMP_Text>();
            foreach (Crop crop in AssetServer.Instance.Crops.Values) {
                GameObject instance = Instantiate(_cropReadoutPrefab, _farmCanvas.transform);
                instance.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<Tags.UI.IconTag>()).sprite = crop.Icon;
                _cropReadouts.Add(crop.YieldType, instance.GetComponentInChildren<TMP_Text>());
            }
            UpdateReadouts();
            UpdateMoney();
            _inventory.OnItemChange += UpdateReadouts;
            _inventory.OnItemChange += UpdateMoney;
        }

        private void UpdateMoney() {
            Debug.Log("Updated money");
            _moneyReadout.text = _inventory.Money.ToString();
        }

        private void UpdateReadouts() {
            foreach (ItemType type in AssetServer.Instance.Crops.Keys) {
                _cropReadouts[type].text = _inventory[type]?.Count.ToString() ?? NONE;
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Tab) && _canvas.alpha == 0.0f) {
                _canvas.FadeCanvas(0.5f, false, this);
            } else if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) && _canvas.alpha == 1.0f) {
                _canvas.FadeCanvas(0.5f, true, this);
            }
        }

        public void BuyPlot() {
            _currentPlots++;
            IsFinished();
        }

        public void FinishedUpgrade() {
            _finishedUpgrades++;
            IsFinished();
        }

        private void IsFinished() {
            if (_finishedUpgrades == _shop.Upgrades.Length && _currentPlots == _plots && !_shownFinish) {
                _finishCanvas.FadeCanvas(1.0f, false, this);
                _shownFinish = true;
            }
        }

        public void MainMenu() {
            StartCoroutine(MainMenu(1f));
        }

        private IEnumerator MainMenu(float time) {
            float timer = time;
            while (timer >= 0f) {
                timer -= Time.fixedDeltaTime;
                yield return Yielders.WaitForFixedUpdate;
                _fader.color = Color.Lerp(Color.clear, Color.black, 1f - timer / time);
            }
            SceneManager.LoadScene(0);
        }
    }
}