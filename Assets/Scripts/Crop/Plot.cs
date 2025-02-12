using System.Linq;

using UnityEngine;

using Items;
using System.Collections.Generic;
using Farms;

namespace Crops {
    public class Plot : MonoBehaviour {
        [SerializeField] private Crop _crop;
        [SerializeField] private CropSlot[] _slots;

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
            foreach (CropSlot slot in _slots) {
                slot.HydrationDrainTick();
                slot.GrowthTick(deltaTime);
            }
        }

        public List<Item> Harvest() {
            List<Item> items = new List<Item>();
            foreach (CropSlot slot in _slots) {
                if (slot.TryGetHarvest(out Item harvest)) {
                    items.Add(harvest);
                }
            }
            return items;
        }

        private void OnMouseEnter() {
            _select.Fade(Color.white, 0.5f, this);
        }

        private void OnMouseExit() {
            _select.Fade(Color.clear, 0.5f, this);
        }
    }
}