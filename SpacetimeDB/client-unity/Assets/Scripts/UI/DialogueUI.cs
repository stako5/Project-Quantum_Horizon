using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class DialogueUI : MonoBehaviour
    {
        private static DialogueUI _inst;
        [Header("UI Refs")]
        [SerializeField] private CanvasGroup overlay;
        [SerializeField] private Text speakerText;
        [SerializeField] private Text lineText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;

        private List<MMORPG.Client.Quests.Local.DialogueLine> _lines;
        private int _index;
        private System.Action _onClosed;

        void Awake()
        {
            if (_inst && _inst != this) { Destroy(gameObject); return; }
            _inst = this; DontDestroyOnLoad(gameObject);
            if (!overlay) BuildUI();
            if (nextButton) nextButton.onClick.AddListener(Next);
            if (closeButton) closeButton.onClick.AddListener(Close);
            HideInternal();
        }

        void OnDestroy()
        {
            if (nextButton) nextButton.onClick.RemoveListener(Next);
            if (closeButton) closeButton.onClick.RemoveListener(Close);
        }

        void BuildUI()
        {
            var canvasGo = new GameObject("DialogueCanvas");
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>(); canvasGo.AddComponent<GraphicRaycaster>();
            var root = new GameObject("Overlay"); root.transform.SetParent(canvasGo.transform, false);
            overlay = root.AddComponent<CanvasGroup>(); overlay.alpha = 0f; overlay.interactable = false; overlay.blocksRaycasts = false;
            var bg = root.AddComponent<Image>(); bg.color = new Color(0,0,0,0.4f);

            var panel = new GameObject("Panel"); panel.transform.SetParent(root.transform, false);
            var prt = panel.AddComponent<RectTransform>(); prt.anchorMin = new Vector2(0.2f, 0.1f); prt.anchorMax = new Vector2(0.8f, 0.3f);
            var pimg = panel.AddComponent<Image>(); pimg.color = new Color(0.05f, 0.05f, 0.07f, 0.9f);

            var speaker = new GameObject("Speaker"); speaker.transform.SetParent(panel.transform, false);
            var srt = speaker.AddComponent<RectTransform>(); srt.anchorMin = new Vector2(0,1); srt.anchorMax = new Vector2(1,1); srt.pivot = new Vector2(0.5f,1); srt.offsetMin = new Vector2(8,-28); srt.offsetMax = new Vector2(-8,-4);
            speakerText = speaker.AddComponent<Text>(); speakerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); speakerText.color = Color.white; speakerText.alignment = TextAnchor.UpperLeft; speakerText.fontSize = 16;

            var body = new GameObject("Body"); body.transform.SetParent(panel.transform, false);
            var brt = body.AddComponent<RectTransform>(); brt.anchorMin = new Vector2(0,0); brt.anchorMax = new Vector2(1,1); brt.offsetMin = new Vector2(8,8); brt.offsetMax = new Vector2(-8,-36);
            lineText = body.AddComponent<Text>(); lineText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); lineText.color = Color.white; lineText.alignment = TextAnchor.UpperLeft; lineText.fontSize = 14;

            var nextGo = new GameObject("Next"); nextGo.transform.SetParent(panel.transform, false);
            var nrt = nextGo.AddComponent<RectTransform>(); nrt.anchorMin = new Vector2(1,0); nrt.anchorMax = new Vector2(1,0); nrt.pivot = new Vector2(1,0); nrt.anchoredPosition = new Vector2(-8,8); nrt.sizeDelta = new Vector2(100,28);
            var nimg = nextGo.AddComponent<Image>(); nimg.color = new Color(0.2f,0.2f,0.25f,0.8f);
            nextButton = nextGo.AddComponent<Button>();
            var ntext = new GameObject("Text").AddComponent<Text>(); ntext.transform.SetParent(nextGo.transform, false); ntext.text = ">"; ntext.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); ntext.alignment = TextAnchor.MiddleCenter; ntext.color = Color.white;
            var nt = ntext.GetComponent<RectTransform>(); nt.anchorMin = Vector2.zero; nt.anchorMax = Vector2.one; nt.offsetMin = Vector2.zero; nt.offsetMax = Vector2.zero;

            var closeGo = new GameObject("Close"); closeGo.transform.SetParent(panel.transform, false);
            var crt = closeGo.AddComponent<RectTransform>(); crt.anchorMin = new Vector2(0,0); crt.anchorMax = new Vector2(0,0); crt.pivot = new Vector2(0,0); crt.anchoredPosition = new Vector2(8,8); crt.sizeDelta = new Vector2(100,28);
            var cimg = closeGo.AddComponent<Image>(); cimg.color = new Color(0.2f,0.2f,0.25f,0.8f);
            closeButton = closeGo.AddComponent<Button>();
            var ctext = new GameObject("Text").AddComponent<Text>(); ctext.transform.SetParent(closeGo.transform, false); ctext.text = "X"; ctext.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); ctext.alignment = TextAnchor.MiddleCenter; ctext.color = Color.white;
            var ct = ctext.GetComponent<RectTransform>(); ct.anchorMin = Vector2.zero; ct.anchorMax = Vector2.one; ct.offsetMin = Vector2.zero; ct.offsetMax = Vector2.zero;
        }

        void ShowInternal()
        {
            overlay.alpha = 1f; overlay.interactable = true; overlay.blocksRaycasts = true;
        }
        void HideInternal()
        {
            overlay.alpha = 0f; overlay.interactable = false; overlay.blocksRaycasts = false;
        }

        void Next()
        {
            if (_lines == null) { Close(); return; }
            _index++;
            if (_index >= _lines.Count) { Close(); return; }
            ApplyLine();
        }

        void Close()
        {
            HideInternal();
            _lines = null; _index = 0; _onClosed?.Invoke(); _onClosed = null;
        }

        void ApplyLine()
        {
            var line = _lines[_index];
            if (speakerText) speakerText.text = line.speaker;
            if (lineText) lineText.text = line.text;
        }

        public static void ShowLines(List<MMORPG.Client.Quests.Local.DialogueLine> lines, System.Action onClosed = null)
        {
            if (lines == null || lines.Count == 0) { onClosed?.Invoke(); return; }
            var ui = _inst ? _inst : new GameObject("DialogueUI").AddComponent<DialogueUI>();
            ui._lines = lines; ui._index = 0; ui._onClosed = onClosed; ui.ApplyLine(); ui.ShowInternal();
        }
    }
}

