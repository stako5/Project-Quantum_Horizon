using System.Collections.Generic;
using UnityEngine;
using MMORPG.Client.UI;

namespace MMORPG.Client.Dungeon
{
    public enum PuzzleModuleType
    {
        GlyphKeys, PressurePlates, LaserMirror, BatteryRouting, PatternMatch, RuneDials, WeightScales, PipeValves, HoloBridge, CodeSwitches
    }

    [System.Serializable]
    public class PuzzleModuleBinding
    {
        public string name; // must match bosses.json strings
        public PuzzleModuleType type;
        public GameObject prefab; // room/puzzle prefab
    }

    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField] public TextAsset bossesJson; // SpacetimeDB/design/bosses/data/bosses.json
        [SerializeField] private int bossIndex = 0;
        [SerializeField] public GameObject startRoomPrefab;
        [SerializeField] public GameObject bossArenaPrefab;
        [SerializeField] public List<PuzzleModuleBinding> puzzleBindings = new List<PuzzleModuleBinding>();

        [Header("Layout")]
        [SerializeField] private Vector3 roomSize = new Vector3(40, 10, 40);
        [SerializeField] private int puzzleRooms = 3;

        [Header("Preview/Debug")] [SerializeField] private BestiaryPreviewRenderer miniMapRenderer;

        private Transform _root;
        public Transform entryPoint { get; private set; }
        public Transform exitPoint { get; private set; }

        [System.Serializable]
        public class BossEnvelope { public int count; public List<BossDef> bosses; }
        [System.Serializable]
        public class BossDef { public string id; public string name; public string biome; public int tier; public List<string> mechanics; public List<string> puzzle_modules; public List<string> phases; public ArmorSet armor_set; public string description; }
        [System.Serializable]
        public class ArmorSet { public string id; public string name; public List<ArmorPiece> pieces; public List<string> set_bonuses; }
        [System.Serializable]
        public class ArmorPiece { public string slot; public string name; public string bonus; }

        void Start()
        {
            if (!string.IsNullOrEmpty(MMORPG.Client.Dungeon.DungeonRun.TargetBossId))
                GenerateForBossId(MMORPG.Client.Dungeon.DungeonRun.TargetBossId);
            else
                Generate();
        }

        public void Clear()
        {
            if (_root) Destroy(_root.gameObject);
        }

        public void Generate()
        {
            Clear();
            _root = new GameObject("Dungeon").transform;
            var boss = LoadBoss();
            if (boss == null)
            {
                Debug.LogWarning("DungeonGenerator: No boss loaded");
                return;
            }

            Vector3 cursor = Vector3.zero;
            // Start room
            if (startRoomPrefab)
            {
                var start = Instantiate(startRoomPrefab, cursor, Quaternion.identity, _root);
                entryPoint = start.transform;
            }
            cursor += new Vector3(roomSize.x, 0, 0);

            // Puzzle sequence
            var modules = boss.puzzle_modules ?? new List<string>();
            for (int i = 0; i < Mathf.Max(1, puzzleRooms) && i < modules.Count; i++)
            {
                var mod = modules[i];
                var prefab = FindPuzzlePrefab(mod);
                if (prefab == null) prefab = startRoomPrefab; // fallback
                var room = Instantiate(prefab, cursor, Quaternion.identity, _root);
                // Add a simple door component placeholder to be wired by puzzles
                room.AddComponent<SimpleDoor>();
                cursor += new Vector3(roomSize.x, 0, 0);
            }

            // Boss arena
            var arena = bossArenaPrefab ? Instantiate(bossArenaPrefab, cursor, Quaternion.identity, _root) : new GameObject("BossArena");
            var bossSpawn = arena.AddComponent<BossSpawn>();
            bossSpawn.BossId = boss.id;
            // Exit portal
            var exitObj = new GameObject("ExitPortal"); exitObj.transform.SetParent(arena.transform, false); exitObj.transform.position = cursor + new Vector3(5,0,0);
            var col = exitObj.AddComponent<BoxCollider>(); col.isTrigger = true; col.size = new Vector3(2,2,2);
            exitObj.AddComponent<DungeonExitPortal>();
            exitPoint = exitObj.transform;
        }

        public void GenerateForBossId(string bossId)
        {
            Clear();
            _root = new GameObject("Dungeon").transform;
            var boss = LoadBossById(bossId) ?? LoadBoss();
            if (boss == null) return;
            Vector3 cursor = Vector3.zero;
            if (startRoomPrefab)
            {
                var start = Instantiate(startRoomPrefab, cursor, Quaternion.identity, _root);
                entryPoint = start.transform;
            }
            cursor += new Vector3(roomSize.x, 0, 0);
            var modules = boss.puzzle_modules ?? new List<string>();
            for (int i = 0; i < Mathf.Max(1, puzzleRooms) && i < modules.Count; i++)
            {
                var mod = modules[i];
                var prefab = FindPuzzlePrefab(mod) ?? startRoomPrefab;
                GameObject room;
                if (prefab)
                {
                    room = Instantiate(prefab, cursor, Quaternion.identity, _root);
                    if (!room.GetComponent<SimpleDoor>()) room.AddComponent<SimpleDoor>();
                }
                else
                {
                    room = BuildSamplePuzzleRoom(cursor);
                }
                cursor += new Vector3(roomSize.x, 0, 0);
            }
            var arena = bossArenaPrefab ? Instantiate(bossArenaPrefab, cursor, Quaternion.identity, _root) : new GameObject("BossArena");
            var bossSpawn = arena.AddComponent<BossSpawn>(); bossSpawn.BossId = boss.id;
            var exitObj = new GameObject("ExitPortal"); exitObj.transform.SetParent(arena.transform, false); exitObj.transform.position = cursor + new Vector3(5,0,0);
            var col = exitObj.AddComponent<BoxCollider>(); col.isTrigger = true; col.size = new Vector3(2,2,2);
            exitObj.AddComponent<DungeonExitPortal>();
            exitPoint = exitObj.transform;
        }

        BossDef LoadBoss()
        {
            // If a portal set a target boss, prefer by id
            if (!string.IsNullOrEmpty(MMORPG.Client.Dungeon.DungeonRun.TargetBossId))
            {
                var env = JsonUtility.FromJson<BossEnvelope>(bossesJson.text);
                if (env != null && env.bosses != null)
                {
                    foreach (var b in env.bosses)
                        if (b.id == MMORPG.Client.Dungeon.DungeonRun.TargetBossId) return b;
                }
            }
            if (bossesJson && !string.IsNullOrEmpty(bossesJson.text))
            {
                try
                {
                    var env = JsonUtility.FromJson<BossEnvelope>(bossesJson.text);
                    if (env != null && env.bosses != null && env.bosses.Count > 0)
                    {
                        int idx = Mathf.Clamp(bossIndex, 0, env.bosses.Count - 1);
                        return env.bosses[idx];
                    }
                }
                catch { }
            }
            return null;
        }

        BossDef LoadBossById(string id)
        {
            if (bossesJson && !string.IsNullOrEmpty(bossesJson.text))
            {
                try
                {
                    var env = JsonUtility.FromJson<BossEnvelope>(bossesJson.text);
                    if (env != null && env.bosses != null)
                    {
                        foreach (var b in env.bosses) if (b.id == id) return b;
                    }
                }
                catch { }
            }
            return null;
        }

        GameObject FindPuzzlePrefab(string name)
        {
            foreach (var b in puzzleBindings)
            {
                if (!string.IsNullOrEmpty(b.name) && b.name == name) return b.prefab;
            }
            return null;
        }

        GameObject BuildSamplePuzzleRoom(Vector3 origin)
        {
            // Simple rectangular room with two trigger plates and a door
            var room = new GameObject("SamplePuzzleRoom");
            room.transform.position = origin;
            room.transform.SetParent(_root, false);

            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(room.transform, false);
            // Scale approximate to roomSize
            floor.transform.localScale = new Vector3(roomSize.x / 10f, 1f, roomSize.z / 10f);

            // Door
            var doorGo = new GameObject("Door");
            doorGo.transform.SetParent(room.transform, false);
            doorGo.transform.localPosition = new Vector3(roomSize.x * 0.45f, 1f, 0f);
            var door = doorGo.AddComponent<SimpleDoor>();
            var doorCol = doorGo.AddComponent<BoxCollider>();
            doorCol.isTrigger = true; doorCol.size = new Vector3(2f, 3f, 1f);

            // Trigger plates
            var platesParent = new GameObject("Plates"); platesParent.transform.SetParent(room.transform, false);
            var plateA = CreatePlate(new Vector3(-roomSize.x * 0.25f, 0.1f, -roomSize.z * 0.25f), platesParent.transform);
            var plateB = CreatePlate(new Vector3(-roomSize.x * 0.25f, 0.1f, roomSize.z * 0.25f), platesParent.transform);

            // Puzzle controller
            var ctrl = room.AddComponent<PuzzleRoomController>();
            var list = new List<PuzzleTriggerPlate> { plateA, plateB };
            typeof(PuzzleRoomController).GetField("plates", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance)?.SetValue(ctrl, list);
            typeof(PuzzleRoomController).GetField("door", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance)?.SetValue(ctrl, door);

            return room;
        }

        PuzzleTriggerPlate CreatePlate(Vector3 localPos, Transform parent)
        {
            var plateGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plateGo.name = "TriggerPlate";
            plateGo.transform.SetParent(parent, false);
            plateGo.transform.localPosition = localPos;
            plateGo.transform.localScale = new Vector3(0.8f, 0.05f, 0.8f);
            var plate = plateGo.AddComponent<PuzzleTriggerPlate>();
            var col = plateGo.GetComponent<Collider>(); col.isTrigger = true;
            return plate;
        }
    }

    public class SimpleDoor : MonoBehaviour
    {
        public bool IsLocked = true;
        public void Unlock() { IsLocked = false; }
    }
}
