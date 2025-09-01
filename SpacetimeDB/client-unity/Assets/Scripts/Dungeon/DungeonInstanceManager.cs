using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Dungeon
{
    public class DungeonInstanceManager : MonoBehaviour
    {
        public static DungeonInstanceManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private DungeonGenerator generatorPrefab;

        [Header("Defaults (copied to instance)")]
        [SerializeField] private TextAsset bossesJson;
        [SerializeField] private GameObject startRoomPrefab;
        [SerializeField] private GameObject bossArenaPrefab;
        [SerializeField] private List<DungeonGenerator.PuzzleModuleBinding> puzzleBindings = new List<DungeonGenerator.PuzzleModuleBinding>();
        [SerializeField] private MMORPG.Client.UI.InstanceHUD hud;

        private Vector3 _returnPosition;
        private DungeonGenerator _current;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this; DontDestroyOnLoad(gameObject);
        }

        public void SetPlayer(Transform t) { player = t; }

        public void SpawnInstance(string bossId)
        {
            if (!player) { Debug.LogWarning("DungeonInstanceManager: Missing player"); return; }
            if (_current) { Debug.LogWarning("DungeonInstanceManager: Instance already running"); return; }
            _returnPosition = player.position;
            if (generatorPrefab) _current = Instantiate(generatorPrefab); else _current = new GameObject("DungeonGenerator").AddComponent<DungeonGenerator>();
            _current.bossesJson = bossesJson;
            _current.startRoomPrefab = startRoomPrefab;
            _current.bossArenaPrefab = bossArenaPrefab;
            _current.puzzleBindings = puzzleBindings;
            _current.GenerateForBossId(bossId);
            if (_current.entryPoint) player.position = _current.entryPoint.position + Vector3.up * 1f;
            if (hud) hud.ShowEnter(bossId);
        }

        public void ExitInstance()
        {
            if (_current)
            {
                Destroy(_current.gameObject);
                _current = null;
            }
            if (player) player.position = _returnPosition;
            if (hud) hud.ShowExit();
        }
    }
}
