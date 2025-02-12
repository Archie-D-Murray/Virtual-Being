using UnityEngine;

using Items;
using Utilities;
using Crops;

public class HarvestManager : MonoBehaviour {
    private Inventory _inventory;
    [SerializeField] private LayerMask _cropLayer;

    private void Start() {
        _inventory = GetComponent<Inventory>();
        _cropLayer = 1 << LayerMask.NameToLayer("Plot");
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Vector2 mousePosition = Helpers.Instance.WorldMousePosition();
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.down, 1f, _cropLayer);
            if (hit && hit.transform.TryGetComponent(out Plot plot)) {
                foreach (Item item in plot.Harvest()) {
                    _inventory.AddItem(item);
                }
            }
        }
    }
}