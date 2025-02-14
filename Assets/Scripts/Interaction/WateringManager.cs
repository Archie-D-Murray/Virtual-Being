using Crops;

using UnityEngine;

using Utilities;

namespace Interaction {
    public class WateringManager : MonoBehaviour {
        [SerializeField] private float _waterAmount = 20f;
        [SerializeField] private LayerMask _plotLayer;
        [SerializeField] private float _wateringCooldown = 1.5f;

        private CountDownTimer _waterTimer = new CountDownTimer(0f);
        public float CooldownProgress => _waterTimer.Progress();

        private void Start() {
            _plotLayer = 1 << LayerMask.NameToLayer("Plot");
        }

        private void Update() {
            _waterTimer.Update(Time.deltaTime);
            InteractionManager.Instance.SetInteractionProgress(InteractionType.Water, _waterTimer.Progress());
            if (InteractionManager.Instance.CurrentInteractionType != InteractionType.Water) {
                return;
            }
            if (Input.GetMouseButtonDown(1) && _waterTimer.IsFinished) {
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.WorldMousePosition(), Vector2.down, 10f, _plotLayer);
                if (hit && hit.transform.TryGetComponent(out Plot plot)) {
                    plot.Hydrate(_waterAmount);
                    _waterTimer.Reset(_wateringCooldown);
                }
            }
        }
    }
}