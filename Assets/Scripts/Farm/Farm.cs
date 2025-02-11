using UnityEngine;

using Utilities;

namespace Farms {

    public class Farm : Singleton<Farm> {
        public float GrowthInterval = 1f;
        public float HydrationDrainInterval = 1f;
    }
}