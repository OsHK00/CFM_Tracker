using System.Collections;
using UnityEngine;

namespace CarefreeTracker
{
    internal class HitDisplay : MonoBehaviour
    {
        public static HitDisplay Instance { get; private set; }

        private string _text = "";
        private GUIStyle _style;

        private void Awake()
        {
            Instance = this;
            CarefreeTracker.Instance.Log("HitDisplay.Awake OK");
            StartCoroutine(RefreshLoop());
        }

        private IEnumerator RefreshLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                CarefreeTracker.Instance.UpdateDisplay();
            }
        }

        private void OnGUI()
        {
            if (string.IsNullOrEmpty(_text)) return;

            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperRight
                };
            }

            float w = 250f;
            float h = 80f;
            float x = Screen.width - w - 10f;
            float y = 10f;


            _style.normal.textColor = new Color(0f, 0f, 0f, 0.9f);
            GUI.Label(new Rect(x + 2, y + 2, w, h), _text, _style);

            _style.normal.textColor = new Color(1f, 1f, 1f, 1f);
            GUI.Label(new Rect(x, y, w, h), _text, _style);
        }

        public void Show(string text)
        {
            _text = text;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _text = "";
        }

        public static HitDisplay Create()
        {
            var go = new GameObject("CarefreeTrackerGUI");
            DontDestroyOnLoad(go);
            return go.AddComponent<HitDisplay>();
        }

        public void DestroyThis()
        {
            StopAllCoroutines();
            Instance = null;
            Destroy(gameObject);
        }
    }
}