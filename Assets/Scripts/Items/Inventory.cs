using UnityEngine;

using System.Collections.Generic;

namespace Items {
    public class Inventory : MonoBehaviour {
        [SerializeField] private List<Item> _items = new List<Item>();

        private Dictionary<ItemType, int> _lookup = new Dictionary<ItemType, int>();

        private void Start() {
            PopulateLookup();
        }

        private void PopulateLookup() {
            _lookup.Clear();
            for (int i = 0; i < _items.Count; i++) {
                _lookup.Add(_items[i].Type, i);
            }
        }

        public Item this[ItemType type] {
            get {
                if (_lookup.TryGetValue(type, out int index)) {
                    return _items[index];
                } else {
                    return null;
                }
            }
            set {
                if (_lookup.ContainsKey(value.Type)) {
                    _items[_lookup[value.Type]].Count += value.Count;
                } else {
                    int i = 0;
                    while (i < _items.Count) {
                        if (_items[i] == null) {
                            _items.Insert(i, value);
                            break;
                        }
                        i++;
                    }
                    if (i == _items.Count) {
                        _items.Add(value);
                    }
                    _lookup.Add(value.Type, i);
                }
            }

        }

        public void AddItem(Item item) {
            if (_lookup.ContainsKey(item.Type)) {
                _items[_lookup[item.Type]].Count += item.Count;
            } else {
                int i = 0;
                while (i < _items.Count) {
                    if (_items[i] == null) {
                        _items.Insert(i, item);
                        break;
                    }
                    i++;
                }
                if (i == _items.Count) {
                    _items.Add(item);
                }
                _lookup.Add(item.Type, i);
            }
        }
    }
}