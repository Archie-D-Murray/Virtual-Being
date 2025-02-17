using UnityEngine;

using System.Collections.Generic;
using System;

namespace Items {
    public class Inventory : MonoBehaviour {
        [SerializeField] private List<Item> _items = new List<Item>();
        [SerializeField] private int _money = 0;

        private Dictionary<ItemType, int> _lookup = new Dictionary<ItemType, int>();

        public Action OnItemChange;
        public Action<int> OnMoneyChange;

        public int Money { get => _money; set { _money = value; OnMoneyChange?.Invoke(_money); } }

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
                    OnItemChange?.Invoke();
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
                OnItemChange?.Invoke();
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
            OnItemChange?.Invoke();
        }

        public void Remove(ItemType type, int count) {
            if (!_lookup.TryGetValue(type, out int index)) {
                return;
            } else {
                _items[index].Count = Mathf.Max(0, _items[index].Count - count);
                OnItemChange?.Invoke();
            }
        }
    }
}