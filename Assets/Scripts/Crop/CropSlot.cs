using UnityEngine;

using Farms;

using Utilities;

namespace Crops {
    public class CropSlot : MonoBehaviour {

        [SerializeField] private Plot _plot;
        [SerializeField] private float _hydration = 0.0f;
        [SerializeField] private CropState _state = CropState.None;
        [SerializeField] private CountDownTimer _growthTimer = new CountDownTimer(0f);

        private Crop _crop => _plot.GetCrop();

        public void SetCrop(Plot plot) {
            _plot = plot;
            _state = CropState.Growing;
            _hydration = _crop.HydrationMax;
        }

        public void HydrationDrainTick() {
            _hydration = Mathf.Max(0f, _hydration - _crop.HydrationDrain);
            if (_hydration == 0.0f) {
                _state = CropState.Dead;
            } else if (_hydration / _crop.HydrationMax <= _crop.HydrationThreshold) {
                _state = CropState.Dehydrated;
            }
        }

        public void GrowthTick(float deltaTime) {
            if (_state != CropState.Growing) { return; }
            _growthTimer.Update(deltaTime);
            if (_growthTimer.IsFinished) {
                _state = CropState.FullyGrown;
            }
        }
    }
}