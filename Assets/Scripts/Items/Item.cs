using UnityEngine;

namespace Items {

    public enum ItemType { Carrot, Wheat, Leek }

    [System.Serializable]
    public class Item {
        public ItemType Type;
        public int Count;

        public Item(ItemType type, int count) {
            Type = type;
            Count = count;
        }
    }
}