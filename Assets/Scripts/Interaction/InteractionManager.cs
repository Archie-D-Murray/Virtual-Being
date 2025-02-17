using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Utilities;

using Tags.UI;

namespace Interaction {
    public enum InteractionType { None, Harvest, Water, Weed }
    public class InteractionManager : Singleton<InteractionManager> {
        public InteractionType CurrentInteractionType = InteractionType.None;
        [SerializeField] private Image _harvestIndicator;
        [SerializeField] private Image _wateringIndicator;
        [SerializeField] private Image _weedingIndicator;
        [SerializeField] private CanvasGroup _interactionIndicatorCanvas;
        [SerializeField] private KeyCode _interactionKey = KeyCode.E;

        public KeyCode InteractionKey => _interactionKey;

        private void Start() {
            foreach (Image image in _interactionIndicatorCanvas.GetComponentsInChildren<Image>(true).Where(image => image.transform.parent != _interactionIndicatorCanvas)) {
                if (image.gameObject.HasComponent<HarvestIndicator>()) {
                    _harvestIndicator = image;
                    continue;
                }
                if (image.gameObject.HasComponent<WateringIndicator>()) {
                    _wateringIndicator = image;
                    continue;
                }
                if (image.gameObject.HasComponent<WeedingIndicator>()) {
                    _weedingIndicator = image;
                    continue;
                }
            }
            SetInteractionType(InteractionType.None);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                SetInteractionType(InteractionType.None);
            } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SetInteractionType(InteractionType.Harvest);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SetInteractionType(InteractionType.Water);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SetInteractionType(InteractionType.Weed);
            }
        }

        private void SetInteractionType(InteractionType interactionType) {
            CurrentInteractionType = interactionType;
            switch (interactionType) {
                case InteractionType.Harvest:
                    _harvestIndicator.transform.parent.gameObject.SetActive(true);
                    _wateringIndicator.transform.parent.gameObject.SetActive(false);
                    _weedingIndicator.transform.parent.gameObject.SetActive(false);
                    break;
                case InteractionType.Water:
                    _harvestIndicator.transform.parent.gameObject.SetActive(false);
                    _wateringIndicator.transform.parent.gameObject.SetActive(true);
                    _weedingIndicator.transform.parent.gameObject.SetActive(false);
                    break;
                case InteractionType.Weed:
                    _harvestIndicator.transform.parent.gameObject.SetActive(false);
                    _wateringIndicator.transform.parent.gameObject.SetActive(false);
                    _weedingIndicator.transform.parent.gameObject.SetActive(true);
                    break;
                default:
                    _harvestIndicator.transform.parent.gameObject.SetActive(false);
                    _wateringIndicator.transform.parent.gameObject.SetActive(false);
                    _weedingIndicator.transform.parent.gameObject.SetActive(false);
                    break;
            }
        }

        public void SetInteractionProgress(InteractionType type, float amount) {
            switch (type) {
                case InteractionType.Harvest:
                    _harvestIndicator.fillAmount = amount;
                    break;
                case InteractionType.Water:
                    _wateringIndicator.fillAmount = amount;
                    break;
                case InteractionType.Weed:
                    _weedingIndicator.fillAmount = amount;
                    break;
            }
        }
    }
}