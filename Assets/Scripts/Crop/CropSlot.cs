using UnityEngine;

using Farms;

using Items;

using Utilities;
using System;

using Tags.Crop;

namespace Crops {
    public class CropSlot : MonoBehaviour {
        [SerializeField] private float _nonWeedTime = 0f;
        [SerializeField] private Plot _plot;
        [SerializeField] private float _hydration = 0.0f;
        [SerializeField] private CropState _state = CropState.None;
        [SerializeField] private CountDownTimer _growthTimer = new CountDownTimer(0f);
        [SerializeField] private CountDownTimer _weedPreventionTimer = new CountDownTimer(5f);
        private SpriteRenderer _cropRenderer;
        private SpriteRenderer _plotRenderer;
        private SpriteRenderer _weedRenderer;
        private Crop _crop => _plot.GetCrop();

        private void Start() {
            if (!_cropRenderer) {
                GetRenderers();
            }
        }

        private void GetRenderers() {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true)) {
                if (renderer.gameObject.HasComponent<PlotTag>()) {
                    _plotRenderer = renderer;
                } else if (renderer.gameObject.HasComponent<CropTag>()) {
                    _cropRenderer = renderer;
                } else if (renderer.gameObject.HasComponent<WeedTag>()) {
                    _weedRenderer = renderer;
                }
            }
        }

        public void SetPlot(Plot plot) {
            if (!_cropRenderer) {
                GetRenderers();
            }
            _plot = plot;
            _state = CropState.Growing;
            _hydration = _crop.HydrationMax;
            _growthTimer.Reset(_crop.GrowthTime);
            _cropRenderer.sprite = GrowthManager.Instance.GetGrowthSprite(_crop.YieldType, 0f);
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

        public bool TryGetHarvest(float multipler, out Item harvest) {
            if (_state == CropState.FullyGrown) {
                harvest = _crop.GetYield(multipler);
                _state = CropState.Growing;
                _growthTimer.Reset();
                _growthTimer.Start();
                _cropRenderer.sprite = GrowthManager.Instance.GetGrowthSprite(_crop.YieldType, _growthTimer.Progress());
                return true;
            } else {
                harvest = null;
                return false;
            }
        }

        public void Hydrate(float waterAmount) {
            _hydration = Mathf.Min(_hydration + waterAmount, _crop.HydrationMax);
        }

        public void WeedTick(float deltaTime) {
            _weedPreventionTimer.Update(deltaTime);
            if (_weedPreventionTimer.IsFinished) { return; }
            _nonWeedTime += deltaTime;
            if (_nonWeedTime >= _crop.WeedTime && UnityEngine.Random.value <= _crop.WeedChance) {
                _state = CropState.Weeds;
                _weedRenderer.color = Color.white;
                _nonWeedTime = 0f;
            }
        }

        public void Weed(float preventionTime) {
            _weedPreventionTimer.Reset(preventionTime);
            _weedRenderer.color = Color.clear;
            _state = _growthTimer.IsFinished ? CropState.FullyGrown : CropState.Growing;
        }
    }
}