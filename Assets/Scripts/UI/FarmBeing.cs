using UnityEngine;
using UnityEngine.UI;

using TMPro;

using System.Collections.Generic;
using Crops;
using System;
using System.Linq;
using Tags.UI;

namespace UI {

    public enum IssueType { Weeds, Dehydration, Dead }

    [Serializable]
    public class IssueData {
        public IssueType Type;
        public Sprite Icon;
        public string Text;
    }

    public class FarmBeing : MonoBehaviour {

        [SerializeField] private GameObject _issuePrefab;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private IssueData[] _data;
        [SerializeField] private CanvasGroup _issueCanvas;
        [SerializeField] private Sprite _happy;
        [SerializeField] private Sprite _unhappy;
        [SerializeField] private Image _being;

        private SpriteRenderer _indicator;
        private Dictionary<IssueType, int> _lookup = new Dictionary<IssueType, int>();

        [SerializeField] private Dictionary<(Plot plot, IssueType issue), GameObject> _issues = new();

        public bool IsHappy => _issues.Count == 0;

        private void Start() {
            _indicator = Instantiate(_indicatorPrefab, Vector3.left * 10, Quaternion.identity).GetComponent<SpriteRenderer>();
            for (int i = 0; i < _data.Length; i++) {
                _lookup.Add(_data[i].Type, i);
            }
        }

        public void RaiseIssue(Plot plot, IssueType issue) {
            if (_issues.ContainsKey((plot, issue))) {
                return;
            }
            GameObject ui = Instantiate(_issuePrefab, _issueCanvas.transform);
            ui.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<IconTag>()).sprite = _data[_lookup[issue]].Icon;
            ui.GetComponentInChildren<TMP_Text>().text = _data[_lookup[issue]].Text;
            ui.GetComponent<PlotHighlight>().Init(plot.GetComponent<Collider2D>(), _indicator);
            _issues.Add((plot, issue), ui);
            UpdateBeing();
        }


        public void ResolveIssue(Plot plot, IssueType issue) {
            if (_issues.TryGetValue((plot, issue), out GameObject uiPopup)) {
                Destroy(uiPopup);
                _issues.Remove((plot, issue));
            }
            UpdateBeing();
        }

        private void UpdateBeing() {
            _being.sprite = IsHappy ? _happy : _unhappy;
        }
    }
}