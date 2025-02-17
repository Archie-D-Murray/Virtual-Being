using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class PlotHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private Collider2D _plot;
        private SpriteRenderer _indicator;

        public void Init(Collider2D plot, SpriteRenderer indicator) {
            _plot = plot;
            _indicator = indicator;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _indicator.transform.position = _plot.offset + (Vector2)_plot.transform.position;
            _indicator.color = Color.white;
        }

        public void OnPointerExit(PointerEventData eventData) {
            _indicator.transform.position = Vector3.left * 10;
            _indicator.color = Color.clear;
        }
    }
}