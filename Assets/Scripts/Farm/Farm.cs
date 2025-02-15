using UnityEngine;

using Utilities;

namespace Farms {

    [DefaultExecutionOrder(-99)]
    public class Farm : Singleton<Farm> {
        public float GrowthInterval = 1f;
        public float HydrationDrainInterval = 1f;
        public TickSystem CropUpdate;
        [SerializeField] private float _growthSpeedMultiplier = 1f;
        [SerializeField] private float _harvestMultiplier = 1f;

        public float GrowthSpeedMultiplier {
            get => _growthSpeedMultiplier;
            set {
                _growthSpeedMultiplier = value;
                CropUpdate.SetTickSpeed(GrowthInterval / _growthSpeedMultiplier);
            }
        }
        public float HarvestMultiplier { get => _harvestMultiplier; set => _harvestMultiplier = value; }

        private void Start() {
            CropUpdate = GetComponent<TickSystem>();
        }
    }
}