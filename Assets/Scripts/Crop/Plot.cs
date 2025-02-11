using UnityEngine;

namespace Crops {
    public class Plot : MonoBehaviour {
        [SerializeField] private Crop _crop;
        [SerializeField] private CropSlot[] _slots;

        private void Start() {
            _slots = GetComponentsInChildren<CropSlot>();
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
    }
}