using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class RegionDropdownUI : MonoBehaviour
    {
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private int regionCount = 10;
        [SerializeField] private World.WorldRegionManager regionManager;
        [SerializeField] private Net.PlayerNetSync netSync;
        [SerializeField] private Net.RemotePlayersManager remoteMgr;

        void Awake()
        {
            if (!dropdown) dropdown = GetComponent<Dropdown>();
            if (!regionManager) regionManager = FindObjectOfType<World.WorldRegionManager>();
            if (!netSync) netSync = FindObjectOfType<Net.PlayerNetSync>();
            if (!remoteMgr) remoteMgr = FindObjectOfType<Net.RemotePlayersManager>();
        }

        void Start()
        {
            if (!dropdown) return;
            dropdown.ClearOptions();
            var opts = new List<string>(); for (int i=0;i<regionCount;i++) opts.Add($"Region {i+1}");
            dropdown.AddOptions(opts);
            dropdown.onValueChanged.AddListener(OnChanged);
        }

        void OnChanged(int index)
        {
            if (regionManager) regionManager.ApplyRegionByIndex(index);
            if (netSync) netSync.SetRegion((byte)index);
            if (remoteMgr)
            {
                var f = typeof(Net.RemotePlayersManager).GetField("localRegionId", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
                if (f != null) f.SetValue(remoteMgr, (byte)index);
            }
        }
    }
}
