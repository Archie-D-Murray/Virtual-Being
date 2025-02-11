using UnityEngine;

using Farms;

using Items;

using Utilities;
using System;

using Tags.Crop;

namespace Crops {
    public class CropSlot : MonoBehaviour {

        [SerializeField] private Plot _plot;
        [SerializeField] private float _hydration = 0.0f;
        [SerializeField] private CropState _state = CropState.None;
        [SerializeField] private CountDownTimer _growthTimer = new CountDownTimer(0f);
        private SpriteRenderer _cropRenderer;
        private SpriteRenderer _plotRenderer;
        private SpriteRenderer _weedRenderer;
        private Crop _crop => _plot.GetCrop();

        private void Start() {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true)) {
                if (renderer.gameObject.HasComponent<PlotTag>()) {
                    _plotRenderer = renderer;
                } else if (renderer.gameObject.HasComponent<CropTag>()) {
                    _cropRenderer = renderer;
                } else {
                    _weedRenderer = renderer;
                }
            }
        }

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
                if (_state != CropState.Dehydrated) {
                    _state = CropState.Dehydrated;
                    // TODO: Colour crop + soil to show dehydration
                }
            }
        }

        public void GrowthTick(float deltaTime) {
            if (_state != CropState.Growing) { return; }
            _growthTimer.Update(deltaTime);
            _cropRenderer.sprite = GrowthManager.Instance.GetGrowthSprite(_crop.YieldType, _growthTimer.Progress());
            if (_growthTimer.IsFinished) {
                _state = CropState.FullyGrown;
            }
        }

        public bool TryGetHarvest(out Item harvest) {
            if (_state == CropState.FullyGrown) {
                harvest = _crop.GetYield();
                _state = CropState.Growing;
                _growthTimer.Reset();
                _growthTimer.Start();
                return true;
            } else {
                harvest = null;
                return false;
            }
        }
    }
}