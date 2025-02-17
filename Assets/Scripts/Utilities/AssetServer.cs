using System.Collections.Generic;

using Crops;

using Items;

using UnityEngine;

using Utilities;

[DefaultExecutionOrder(-99)]
public class AssetServer : Singleton<AssetServer> {
    [SerializeField] private Crop[] _crops;

    public Dictionary<ItemType, Crop> Crops = new Dictionary<ItemType, Crop>();

    private void Start() {
        foreach (Crop crop in _crops) {
            Crops.Add(crop.YieldType, crop);
        }
    }
}