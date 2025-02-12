using System.Linq;

using UnityEngine;

using Items;
using System.Collections.Generic;
using Farms;

namespace Crops {
    public class Plot : MonoBehaviour {
        [SerializeField] private Crop _crop;
        [SerializeField] private CropSlot[] _slots;

        private void Start() {
            _slots = GetComponentsInChildren<CropSlot>();
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
    }
}