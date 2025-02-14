using Crops;

using UnityEngine;

using Utilities;

namespace Interaction {
    public class WeedingManager : MonoBehaviour {
        [SerializeField] private LayerMask _plotLayer;
        [SerializeField] private float _weedingCooldown = 1.5f;
        [SerializeField] private float _weedingPreventionTime = 10f;

        private CountDownTimer _weedingTimer = new CountDownTimer(0f);
        public float CooldownProgress => _weedingTimer.Progress();

        private void Start() {
            _plotLayer = 1 << LayerMask.NameToLayer("Plot");
        }

        private void Update() {
            _weedingTimer.Update(Time.deltaTime);
            InteractionManager.Instance.SetInteractionProgress(InteractionType.Weed, _weedingTimer.Progress());
            if (InteractionManager.Instance.CurrentInteractionType != InteractionType.Weed) {
                return;
            }
            if (Input.GetMouseButtonDown(1) && _weedingTimer.IsFinished) {
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.WorldMousePosition(), Vector2.down, 10f, _plotLayer);
                if (hit && hit.transform.TryGetComponent(out Plot plot)) {
                    plot.Weed(_weedingPreventionTime);
                    _weedingTimer.Reset(_weedingCooldown);
                }
            }
        }
    }
}