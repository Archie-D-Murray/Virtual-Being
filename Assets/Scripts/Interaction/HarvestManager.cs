using UnityEngine;

using Items;
using Utilities;
using Crops;

namespace Interaction {

    public class HarvestManager : MonoBehaviour {
        private Inventory _inventory;
        [SerializeField] private float _harvestMultiplier = 1f;
        [SerializeField] private LayerMask _cropLayer;
        [SerializeField] private float _harvestCooldown = 1f;

        private CountDownTimer _harvestTimer = new CountDownTimer(0f);
        public float CooldownProgress => _harvestTimer.Progress();

        private void Start() {
            _inventory = GetComponent<Inventory>();
            _cropLayer = 1 << LayerMask.NameToLayer("Plot");
        }

        private void Update() {
            _harvestTimer.Update(Time.deltaTime);
            InteractionManager.Instance.SetInteractionProgress(InteractionType.Harvest, _harvestTimer.Progress());
            if (InteractionManager.Instance.CurrentInteractionType != InteractionType.Harvest) {
                return;
            }
            if (Input.GetMouseButtonDown(1) && _harvestTimer.IsFinished) {
                Vector2 mousePosition = Helpers.Instance.WorldMousePosition();
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.down, 1f, _cropLayer);
                if (hit && hit.transform.TryGetComponent(out Plot plot)) {
                    _harvestTimer.Reset(_harvestCooldown);
                    foreach (Item item in plot.Harvest(_harvestMultiplier)) {
                        _inventory.AddItem(item);
                    }
                }
            }
        }
    }
}