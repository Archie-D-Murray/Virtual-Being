using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Utilities;

namespace UI {
    public class Finish : MonoBehaviour {

        private CanvasGroup _canvas;

        private void Start() {
            _canvas = GetComponent<CanvasGroup>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && _canvas.alpha == 1.0f) {
                _canvas.FadeCanvas(0.5f, true, this);
            }
        }
    }
}