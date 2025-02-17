using System.Linq;

using UnityEngine;

using Items;
using System.Collections.Generic;
using Farms;
using System;

namespace Crops {
    public class Plot : MonoBehaviour {
        [SerializeField] private Crop _crop;
        [SerializeField] private CropSlot[] _slots;
        [SerializeField] private bool _isActive;

        private SpriteRenderer _select;

        private void Start() {
            _slots = GetComponentsInChildren<CropSlot>();
            _select = GetComponentInChildren<Tags.Crop.SelectTag>().GetComponent<SpriteRenderer>();
            foreach (CropSlot slot in _slots) {
                slot.SetPlot(this);
            }
            Farm.Instance.CropUpdate.TickLoop += UpdateCrops;
        }

        public Crop GetCrop() {
            return _crop;
        }

        public void UpdateCrops(float deltaTime) {
            if (!_isActive) {
                return;
            }
            foreach (CropSlot slot in _slots) {
                slot.HydrationDrainTick(this);
                slot.GrowthTick(deltaTime, this);
                slot.WeedTick(deltaTime, this);
            }
        }

        public List<Item> Harvest(float multiplier) {
            List<Item> items = new List<Item>();
            if (!_isActive) {
                return items;
            }
            foreach (CropSlot slot in _slots) {
                if (slot.TryGetHarvest(multiplier, out Item harvest)) {
                    items.Add(harvest);
                }
            }
            return items;
        }

        private void OnMouseEnter() {
            if (!_isActive) {
                return;
            }
            _select.Fade(Color.white, 0.5f, this);
        }

        private void OnMouseExit() {
            if (!_isActive) {
                return;
            }
            _select.Fade(Color.clear, 0.5f, this);
        }

        public void Hydrate() {
            if (!_isActive) {
                return;
            }
            foreach (CropSlot slot in _slots) {
                slot.Hydrate(this);
            }
        }

        public void Weed(float preventionTime) {
            if (!_isActive) {
                return;
            }
            foreach (CropSlot slot in _slots) {
                slot.Weed(preventionTime, this);
            }
        }

        public void Enable() {
            _isActive = true;
        }
    }
}