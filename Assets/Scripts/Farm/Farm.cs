using Items;

using UnityEngine;

using Utilities;

namespace Farms {

    [DefaultExecutionOrder(-99)]
    public class Farm : Singleton<Farm> {
        public float GrowthInterval = 1f;
        public float HydrationDrainInterval = 1f;
        public TickSystem CropUpdate;
        [SerializeField] private float _hydrationDrainMultipler = 1f;
        [SerializeField] private float _harvestMultiplier = 1f;
        [SerializeField] private Inventory _inventory;

        public float HydrationDrainMultiplier { get => _hydrationDrainMultipler; set => _hydrationDrainMultipler = value; }
        public float HarvestMultiplier { get => _harvestMultiplier; set => _harvestMultiplier = value; }

        public Inventory Inventory => _inventory;

        protected override void Awake() {
            base.Awake();
            CropUpdate = GetComponent<TickSystem>();
            _inventory = GetComponent<Inventory>();
        }
    }
}