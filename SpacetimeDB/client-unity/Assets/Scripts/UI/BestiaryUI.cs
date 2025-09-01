using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Enemies;
using MMORPG.Client.Net;

namespace MMORPG.Client.UI
{
    [System.Serializable]
    public class FamilyPrefab { public string family; public GameObject prefab; }

    public class BestiaryUI : MonoBehaviour
    {
        [Header("Filters")]
        [SerializeField] private Dropdown familyDropdown;
        [SerializeField] private Dropdown roleDropdown;
        [SerializeField] private Dropdown elementDropdown;
        [SerializeField] private Dropdown tierDropdown; // All, 1..5

        [Header("List")] [SerializeField] private RectTransform content;
        [SerializeField] private Text itemTemplate;
        [Header("Detail")] [SerializeField] private BestiaryDetailUI detailUI;
        [SerializeField] private List<FamilyPrefab> familyPrefabs = new List<FamilyPrefab>();

        [Header("Data")] [SerializeField] private bool useLocalJson = true;
        [SerializeField] private TextAsset localEnemiesJson;

        private List<EnemyTypeModel> _all = new();

        async void Start()
        {
            if (useLocalJson && localEnemiesJson)
            {
                EnemyCatalog.Use(new LocalJsonEnemySource(localEnemiesJson));
                _all = EnemyCatalog.All;
            }
            else
            {
#if SPACETIMEDB_SDK
                await SpacetimeDbClient.Instance.ConnectAsync();
                _all = BindingsBridge.GetEnemies().Select(e => new EnemyTypeModel
                {
                    id = e.Id, name = e.Name, family = e.Family, tier = e.Tier, role = e.Role, element = e.Element,
                    size = e.Size, movement = e.Movement, armor = e.Armor, hp = (int)e.Hp, damage = (int)e.Damage, speed = e.Speed,
                    abilities = new List<string>((e.AbilitiesCsv ?? "").Split(',').Where(s => !string.IsNullOrEmpty(s))),
                    tags = new List<string>((e.TagsCsv ?? "").Split(',').Where(s => !string.IsNullOrEmpty(s))),
                    description = e.Description
                }).ToList();
#else
                _all = new List<EnemyTypeModel>();
#endif
            }

            PopulateFilters();
            ApplyFilters();
            familyDropdown.onValueChanged.AddListener(_ => ApplyFilters());
            roleDropdown.onValueChanged.AddListener(_ => ApplyFilters());
            elementDropdown.onValueChanged.AddListener(_ => ApplyFilters());
            tierDropdown.onValueChanged.AddListener(_ => ApplyFilters());
        }

        void PopulateFilters()
        {
            void SetOptions(Dropdown dd, IEnumerable<string> values)
            {
                dd.ClearOptions();
                var opts = new List<string> { "All" };
                opts.AddRange(values.Distinct().OrderBy(s => s));
                dd.AddOptions(opts);
            }
            SetOptions(familyDropdown, _all.Select(e => e.family));
            SetOptions(roleDropdown, _all.Select(e => e.role));
            SetOptions(elementDropdown, _all.Select(e => e.element));
            tierDropdown.ClearOptions(); tierDropdown.AddOptions(new List<string>{"All","1","2","3","4","5"});
        }

        void ApplyFilters()
        {
            string fam = GetSelected(familyDropdown);
            string role = GetSelected(roleDropdown);
            string elem = GetSelected(elementDropdown);
            int tier = tierDropdown.value; // 0=All

            var list = _all.Where(e => (fam=="All" || e.family==fam) && (role=="All" || e.role==role) && (elem=="All" || e.element==elem) && (tier==0 || e.tier==tier)).ToList();
            Rebuild(list);
        }

        string GetSelected(Dropdown dd) => dd.options[dd.value].text;

        void Rebuild(List<EnemyTypeModel> list)
        {
            foreach (Transform child in content) Destroy(child.gameObject);
            foreach (var e in list)
            {
                var go = new GameObject("Item");
                go.transform.SetParent(content, false);
                var rt = go.AddComponent<RectTransform>();
                var btn = go.AddComponent<Button>();
                var img = go.AddComponent<Image>(); img.color = new Color(1,1,1,0.05f);
                var t = Instantiate(itemTemplate, go.transform);
                t.gameObject.SetActive(true);
                t.alignment = TextAnchor.MiddleLeft;
                t.text = $"[{e.family} T{e.tier}] {e.name} — {e.role}/{e.element}  HP:{e.hp} DMG:{e.damage}";
                btn.onClick.AddListener(() => ShowDetail(e));
            }
        }

        void ShowDetail(EnemyTypeModel e)
        {
            if (!detailUI) return;
            var prefab = GetPrefabForFamily(e.family);
            detailUI.Show(e, prefab);
        }

        GameObject GetPrefabForFamily(string family)
        {
            foreach (var fp in familyPrefabs)
                if (fp != null && fp.family == family) return fp.prefab;
            return null;
        }
    }
}
