using UnityEngine;

using Utilities;
using Items;
using System;
using System.Linq;

namespace Crops {

    [Serializable]
    public class CropGrowthStage {
        [Range(0f, 1f)] public float GrowthProgress = 0;
        public Sprite SpriteFrame;
    }

    [Serializable]
    public class CropGrowthStages {
        public CropGrowthStage[] Wheat;
        public CropGrowthStage[] Carrot;
        public CropGrowthStage[] Leek;

        public CropGrowthStage[] GetGrowthStage(ItemType type) {
            switch (type) {
                case ItemType.Carrot:
                    return Carrot;
                case ItemType.Leek:
                    return Leek;
                default:
                    return Wheat;
            }
        }
    }

    public class GrowthManager : Singleton<GrowthManager> {
        public Sprite HydratedSoil;
        public Sprite DehydratedSoil;
        public CropGrowthStages GrowthStages;

        public Sprite GetGrowthSprite(ItemType type, float growthProgress) {
            foreach (CropGrowthStage stage in GrowthStages.GetGrowthStage(type).Reverse()) {
                if (growthProgress >= stage.GrowthProgress) {
                    return stage.SpriteFrame;
                }
            }
            return GrowthStages.GetGrowthStage(type)[0].SpriteFrame;
        }
    }
}