using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MMORPG.Client.Data;

namespace MMORPG.Client.UI
{
    public interface ICodexDataSource
    {
        Task<List<string>> GetClassesAsync();
        Task<List<AbilityModel>> GetAbilitiesAsync(string className);
        Task<List<PerkModel>> GetPerksAsync(string className);
    }

    // Dev fallback: reads JSON from repo paths. Use only in Editor.
    public class LocalJsonCodexDataSource : ICodexDataSource
    {
        private readonly string _root;
        public LocalJsonCodexDataSource(string projectAssetsPath)
        {
            // Unity Assets/ -> repo root
            _root = Path.GetFullPath(Path.Combine(projectAssetsPath, "..", "SpacetimeDB", "design", "classes", "data"));
        }

        public Task<List<string>> GetClassesAsync()
        {
            if (!Directory.Exists(_root)) return Task.FromResult(new List<string>());
            var classes = Directory.GetDirectories(_root).Select(Path.GetFileName).ToList();
            return Task.FromResult(classes);
        }

        public Task<List<AbilityModel>> GetAbilitiesAsync(string className)
        {
            var path = Path.Combine(_root, className, "full", "abilities.json");
            if (!File.Exists(path)) return Task.FromResult(new List<AbilityModel>());
            var json = File.ReadAllText(path);
            return Task.FromResult(ContentLoader.LoadAbilitiesFromJson(json));
        }

        public Task<List<PerkModel>> GetPerksAsync(string className)
        {
            var path = Path.Combine(_root, className, "full", "perks.json");
            if (!File.Exists(path)) return Task.FromResult(new List<PerkModel>());
            var json = File.ReadAllText(path);
            return Task.FromResult(ContentLoader.LoadPerksFromJson(json));
        }
    }

    // SpacetimeDB-backed data source (requires SPACETIMEDB_SDK define and generated table bindings)
    public class SpacetimeCodexDataSource : ICodexDataSource
    {
        private bool _initialized;
        private readonly Dictionary<string, List<AbilityModel>> _abilities = new();
        private readonly Dictionary<string, List<PerkModel>> _perks = new();
        private readonly List<string> _classes = new();

        public async Task<List<string>> GetClassesAsync()
        {
#if SPACETIMEDB_SDK
            if (!_initialized)
            {
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                _initialized = true;
            }
            _classes.Clear();
            foreach (var row in MMORPG.Client.Net.BindingsBridge.GetClasses())
                _classes.Add(row.ClassName);
            return new List<string>(_classes);
#else
            return new List<string>();
#endif
        }

        public Task<List<AbilityModel>> GetAbilitiesAsync(string className)
        {
#if SPACETIMEDB_SDK
            var list = new List<AbilityModel>();
            foreach (var a in MMORPG.Client.Net.BindingsBridge.GetAbilities(className))
            {
                list.Add(new AbilityModel
                {
                    id = a.Id,
                    name = a.Name,
                    branch = a.Branch,
                    tier = a.Tier,
                    type = a.Kind,
                    description = a.Description,
                    cooldown_s = a.CooldownS ?? 0f,
                    resource = a.ResourceType != null ? new ResourceModel{ type = a.ResourceType, cost = a.ResourceCost ?? 0f } : null,
                    tags = new List<string>()
                });
            }
            _abilities[className] = list; return Task.FromResult(list);
#else
            return Task.FromResult(new List<AbilityModel>());
#endif
        }

        public Task<List<PerkModel>> GetPerksAsync(string className)
        {
#if SPACETIMEDB_SDK
            var list = new List<PerkModel>();
            foreach (var p in MMORPG.Client.Net.BindingsBridge.GetPerks(className))
            {
                list.Add(new PerkModel
                {
                    id = p.Id,
                    name = p.Name,
                    tier = p.Tier,
                    tags = new List<string>((p.TagsCsv ?? "").Split(',')),
                    description = p.Description,
                    rolls = new List<PerkModel.PerkRoll>()
                });
            }
            _perks[className] = list; return Task.FromResult(list);
#else
            return Task.FromResult(new List<PerkModel>());
#endif
        }
    }

    public class CodexUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Dropdown classDropdown;
        [SerializeField] private RectTransform abilitiesContent;
        [SerializeField] private RectTransform perksContent;
        [SerializeField] private Text itemTemplate;
        [Header("Data Source")]
        [SerializeField] private bool useLocalJson = true;

        private ICodexDataSource _data;

        private async void Start()
        {
            _data = useLocalJson ? new LocalJsonCodexDataSource(Application.dataPath) as ICodexDataSource : new SpacetimeCodexDataSource();
            await PopulateClasses();
        }

        async Task PopulateClasses()
        {
            var classes = await _data.GetClassesAsync();
            classDropdown.ClearOptions();
            classDropdown.AddOptions(classes);
            classDropdown.onValueChanged.AddListener(async _ => await OnClassChanged());
            if (classes.Count > 0)
                await OnClassChanged();
        }

        async Task OnClassChanged()
        {
            var selected = classDropdown.options[classDropdown.value].text;
            var abilities = await _data.GetAbilitiesAsync(selected);
            var perks = await _data.GetPerksAsync(selected);
            RebuildList(abilitiesContent, abilities.Select(a => $"[{a.branch} T{a.tier}] {a.name} — {a.description}"));
            RebuildList(perksContent, perks.Select(p => $"[{p.tier}] {p.name} — {p.description}"));
        }

        void RebuildList(RectTransform parent, IEnumerable<string> lines)
        {
            foreach (Transform child in parent) Destroy(child.gameObject);
            foreach (var line in lines)
            {
                var item = Instantiate(itemTemplate, parent);
                item.text = line;
                item.gameObject.SetActive(true);
            }
        }
    }
}
