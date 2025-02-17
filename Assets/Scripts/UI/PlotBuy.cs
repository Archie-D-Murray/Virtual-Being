using UnityEngine;
using UnityEngine.UI;

using Crops;

using Farms;
using System;
using TMPro;

namespace UI {
    public class PlotBuy : MonoBehaviour {
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private int _moneyPerCropSlot = 100;
        [SerializeField] private Button _buy;
        [SerializeField] private Plot _plot;

        private void Start() {
            _canvas = GetComponent<CanvasGroup>();
            _buy = GetComponentInChildren<Button>();
            _buy.interactable = false;
            _canvas.GetComponentInChildren<TMP_Text>().text = Cost().ToString();
            if (Farm.Instance.Inventory) {
                Farm.Instance.Inventory.OnMoneyChange += UpdateButton;
            }
        }

        private void UpdateButton(int money) {
            _buy.interactable = money >= Cost();
        }

        private void Buy() {
            Farm.Instance.Inventory.Money -= Cost();
            _buy.interactable = false;
            _plot.Enable();
            _canvas.FadeCanvas(1.0f, true, this);
            Destroy(gameObject, 1.1f);
        }

        private int Cost() {
            return _moneyPerCropSlot * (_plot.transform.childCount - 1);
        }
    }
}