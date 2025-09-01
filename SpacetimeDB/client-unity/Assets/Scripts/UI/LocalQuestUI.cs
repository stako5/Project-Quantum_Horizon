using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class LocalQuestUI : MonoBehaviour
    {
        [SerializeField] private Quests.Local.LocalQuestTracker tracker;
        [SerializeField] private RectTransform content;
        [SerializeField] private Text rowTemplate;
        [SerializeField] private float refreshInterval = 0.5f;
        [Header("NLP Fallback")]
        [SerializeField] private bool useNlpFallback = true;
        [SerializeField] private string defaultNpcName = "NPC";
        [SerializeField, Range(0f,1f)] private float o = 0.5f;
        [SerializeField, Range(0f,1f)] private float c = 0.5f;
        [SerializeField, Range(0f,1f)] private float e = 0.5f;

        private float _t;

        void Awake()
        {
            if (!tracker) tracker = FindObjectOfType<Quests.Local.LocalQuestTracker>();
            if (rowTemplate) rowTemplate.gameObject.SetActive(false);
        }

        void Update()
        {
            _t -= Time.deltaTime; if (_t > 0f) return; _t = refreshInterval;
            Rebuild();
        }

        void Rebuild()
        {
            if (!tracker || !content || !rowTemplate) return;
            foreach (Transform c in content) if (c != rowTemplate.transform) Destroy(c.gameObject);
            foreach (var q in tracker.AllQuests())
            {
                var row = Instantiate(rowTemplate, content);
                row.gameObject.SetActive(true);
                var kill = tracker.GetKillProgress(q);
                var collects = tracker.GetCollectProgress(q).ToList();
                string prog = "";
                var L = MMORPG.Client.Localization.LocalizationManager.Get;
                if (q.killCount > 0) prog += $"{L("kill_label")} {q.killFamily} {kill.cur}/{kill.req}\n";
                if (collects.Count > 0)
                {
                    foreach (var c in collects) prog += $"{L("collect_label")} {c.id} {c.cur}/{c.req}\n";
                }
                bool done = tracker.IsCompleted(q);
                bool claimed = tracker.IsClaimed(q);
                string claimTxt = done && !claimed ? $"[{L("click_to_claim")}]" : (claimed ? $"({L("claimed")})" : "");
                row.text = $"{q.displayName}\n{prog}{claimTxt}";
                var btn = row.GetComponent<Button>(); if (!btn) btn = row.gameObject.AddComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(async () =>
                {
                    bool canClaim = tracker.IsCompleted(q) && !tracker.IsClaimed(q);
                    var dlgAsset = MMORPG.Client.Quests.Local.QuestDialogueDB.Get(q.questId);
                    var lines = new System.Collections.Generic.List<MMORPG.Client.Quests.Local.DialogueLine>();
                    if (dlgAsset)
                    {
                        if (canClaim && dlgAsset.completion != null && dlgAsset.completion.Count > 0) lines = dlgAsset.completion;
                        else if (!canClaim && dlgAsset.inProgress != null && dlgAsset.inProgress.Count > 0) lines = dlgAsset.inProgress;
                    }
                    if (lines.Count > 0)
                    {
                        DialogueUI.ShowLines(lines, onClosed: () =>
                        {
                            if (tracker.IsCompleted(q) && !tracker.IsClaimed(q)) { tracker.TryClaim(q); Rebuild(); }
                        });
                    }
                    else
                    {
                        if (useNlpFallback)
                        {
                            var text = await GenerateNlpAsync(q, canClaim ? "quest_complete" : "quest_progress");
                            if (!string.IsNullOrEmpty(text))
                            {
                                var tmp = new System.Collections.Generic.List<MMORPG.Client.Quests.Local.DialogueLine>{ new MMORPG.Client.Quests.Local.DialogueLine{ speaker = defaultNpcName, text = text } };
                                DialogueUI.ShowLines(tmp, onClosed: () =>
                                {
                                    if (tracker.IsCompleted(q) && !tracker.IsClaimed(q)) { tracker.TryClaim(q); Rebuild(); }
                                });
                                return;
                            }
                        }
                        if (tracker.IsCompleted(q) && !tracker.IsClaimed(q)) { tracker.TryClaim(q); Rebuild(); }
                    }
                });
            }
        }

        [System.Serializable]
        class NlpInput
        {
            public string speaker_name;
            public string player_name;
            public string topic;
            public string style;
            public string tone;
            public float o; public float c; public float e;
            public string last_player_line;
            public string locale;
        }
        [System.Serializable] class NlpResult { public string text; }

        async System.Threading.Tasks.Task<string> GenerateNlpAsync(Quests.Local.QuestDefinition q, string style)
        {
            try
            {
                var inp = new NlpInput
                {
                    speaker_name = string.IsNullOrEmpty(defaultNpcName) ? "NPC" : defaultNpcName,
                    player_name = UnityEngine.SystemInfo.deviceName,
                    topic = q != null ? q.displayName : "",
                    style = style,
                    tone = "friendly",
                    o = o, c = c, e = e,
                    last_player_line = "",
                    locale = MMORPG.Client.Localization.LocalizationManager.Language
                };
                string json = JsonUtility.ToJson(inp);
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                var res = await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerForStringAsync("nlp_generate_reply", json);
                if (!string.IsNullOrEmpty(res))
                {
                    var r = JsonUtility.FromJson<NlpResult>(res);
                    return r != null ? r.text : res;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[NLP] fallback failed: {ex.Message}");
            }
            return null;
        }
    }
}
