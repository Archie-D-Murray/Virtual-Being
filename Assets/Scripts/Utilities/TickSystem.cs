using UnityEngine;
using UnityEngine.Events;

namespace Utilities {

    public class TickSystem : MonoBehaviour {
        [SerializeField] private float _tickDelay = 1.0f;
        [SerializeField] private float _currentTime = 0.0f;
        public UnityAction<float> TickLoop;

        public void SetTickSpeed(float tickDelay) {
            _tickDelay = tickDelay;
        }

        private void FixedUpdate() {
            _currentTime += Time.fixedDeltaTime;
            if (_currentTime >= _tickDelay) {
                TickLoop?.Invoke(_currentTime);
                _currentTime -= _tickDelay;
            }
        }
    }
}