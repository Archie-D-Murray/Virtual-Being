using UnityEngine;

using Utilities;

namespace Farms {

    [DefaultExecutionOrder(-99)]
    public class Farm : Singleton<Farm> {
        public float GrowthInterval = 1f;
        public float HydrationDrainInterval = 1f;
        public TickSystem CropUpdate;

        private void Start() {
            CropUpdate = GetComponent<TickSystem>();
        }
    }
}