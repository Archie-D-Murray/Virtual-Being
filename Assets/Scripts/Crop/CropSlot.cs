using UnityEngine;

using Farms;

using Items;

using Utilities;
using System;

using Tags.Crop;
using UI;

namespace Crops {
    public class CropSlot : MonoBehaviour {
        [SerializeField] private float _nonWeedTime = 0f;
        [SerializeField] private Plot _plot;
        [SerializeField] private float _hydration = 0.0f;
        [SerializeField] private CropState _state = CropState.None;
        [SerializeField] private CountDownTimer _growthTimer = new CountDownTimer(0f);
        [SerializeField] private CountDownTimer _weedPreventionTimer = new CountDownTimer(5f);
        [SerializeField] private bool _hasWeeds = false;
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

        public void HydrationDrainTick(Plot plot) {
            _hydration = Mathf.Max(0f, _hydration - _crop.HydrationDrain * Farm.Instance.HydrationDrainMultiplier);
            if (_hydration == 0.0f) {
                GrowthManager.Instance.FarmBeing.RaiseIssue(plot, IssueType.Dead);
            } else if (_hydration / _crop.HydrationMax <= _crop.HydrationThreshold) {
                if (_state != CropState.Dehydrated) {
                    _plotRenderer.sprite = GrowthManager.Instance.DehydratedSoil;
                    GrowthManager.Instance.FarmBeing.RaiseIssue(plot, IssueType.Dehydration);
                }
            }
            _state = GetGrowthState();
        }

        public void GrowthTick(float deltaTime, Plot plot) {
            if (_state != CropState.Growing) { return; }
            _growthTimer.Update(deltaTime);
            _cropRenderer.sprite = GrowthManager.Instance.GetGrowthSprite(_crop.YieldType, _growthTimer.Progress());
            _state = GetGrowthState();
        }

        public bool TryGetHarvest(float multipler, out Item harvest) {
            if (_state == CropState.FullyGrown) {
                harvest = _crop.GetYield(multipler);
                _growthTimer.Reset();
                _growthTimer.Start();
                _cropRenderer.sprite = GrowthManager.Instance.GetGrowthSprite(_crop.YieldType, _growthTimer.Progress());
                _state = GetGrowthState();
                return true;
            } else {
                harvest = null;
                return false;
            }
        }

        public void Hydrate(float waterAmount, Plot plot) {
            _hydration = Mathf.Min(_hydration + waterAmount, _crop.HydrationMax);
            if (_state == CropState.Dehydrated || _state == CropState.Dead) {
                _plotRenderer.sprite = GrowthManager.Instance.HydratedSoil;
                if (_state == CropState.Dead) {
                    _growthTimer.Reset();
                    _growthTimer.Start();
                    GrowthManager.Instance.FarmBeing.ResolveIssue(plot, IssueType.Dead);
                    GrowthManager.Instance.FarmBeing.ResolveIssue(plot, IssueType.Dehydration);
                } else {
                    GrowthManager.Instance.FarmBeing.ResolveIssue(plot, IssueType.Dehydration);
                }
            }
            _state = GetGrowthState();
        }

        private bool IsDehydrated() {
            return _hydration / _crop.HydrationMax < _crop.HydrationThreshold;
        }

        public void WeedTick(float deltaTime, Plot plot) {
            _weedPreventionTimer.Update(deltaTime);
            if (_weedPreventionTimer.IsFinished) { return; }
            _nonWeedTime += deltaTime;
            if (_nonWeedTime >= _crop.WeedTime && UnityEngine.Random.value <= _crop.WeedChance) {
                _weedRenderer.color = Color.white;
                _nonWeedTime = 0f;
                _hasWeeds = true;
                _state = GetGrowthState();
                GrowthManager.Instance.FarmBeing.RaiseIssue(plot, IssueType.Weeds);
            }
        }

        public void Weed(float preventionTime, Plot plot) {
            _weedPreventionTimer.Reset(preventionTime);
            _weedRenderer.color = Color.clear;
            _hasWeeds = false;
            _state = GetGrowthState();
            GrowthManager.Instance.FarmBeing.ResolveIssue(plot, IssueType.Weeds);
        }

        private CropState GetGrowthState() {
            if (_hydration == 0.0f) {
                return CropState.Dead;
            } else if (IsDehydrated()) {
                return CropState.Dehydrated;
            } else if (_hasWeeds) {
                return CropState.Weeds;
            } else if (_growthTimer.IsFinished) {
                return CropState.FullyGrown;
            } else {
                return CropState.Growing;
            }
        }
    }
}