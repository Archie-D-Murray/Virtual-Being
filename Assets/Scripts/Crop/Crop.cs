using UnityEngine;

namespace Crops {

    public enum CropState { None, Growing, FullyGrown, Dehydrated, Dead }

    public class Crop : ScriptableObject {
        public string Name = "Crop";
        public int ExperiencePerHarvest = 10;
        public float GrowthTime = 5f;
        public float HydrationThreshold = 0.5f;
        public float HydrationMax = 10f;
        public float HydrationDrain = 0.5f;
    }
}