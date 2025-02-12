using UnityEngine;

using Items;

namespace Crops {

    public enum CropState { None, Growing, FullyGrown, Dehydrated, Dead }

    [CreateAssetMenu(menuName = "Crop")]
    public class Crop : ScriptableObject {
        public string Name = "Crop";
        public ItemType YieldType = ItemType.Wheat;
        public int YieldAmount = 1;
        public int ExperiencePerHarvest = 10;
        public float GrowthTime = 5f;
        public float HydrationThreshold = 0.5f;
        public float HydrationMax = 10f;
        public float HydrationDrain = 0.5f;

        public Item GetYield() {
            return new Item(YieldType, YieldAmount);
        }
    }
}