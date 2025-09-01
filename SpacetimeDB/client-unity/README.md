Unity Client (Stub)

- Goal: Connect to SpacetimeDB, call reducers, subscribe to tables.

Prereqs
- Unity 2021.3+ (LTS recommended)
- SpacetimeDB Unity/C# SDK (see https://spacetimedb.com/docs/unity/part-1)

Quick Start
- Create/open a Unity project
- Add the SpacetimeDB package per docs
- Create a MonoBehaviour (e.g. `SpacetimeClient.cs`) and wire it to a scene

Consumables & Buffs
- Generate data: run `python3 SpacetimeDB/design/consumables/generate_consumables.py` (already generated: 500 items).
- In Unity: Tools → MMORPG → Create Sample Scenes → open `Assets/Scenes/ConsumablesScene.unity`.
- Press Play; click any row to consume; right panel shows active buffs and aggregated modifiers.

Example Sketch (pseudocode)
- Connect, register, set name, subscribe to `player` and `world` tables:

```
using System.Threading.Tasks;
using SpacetimeDB.Client; // exact namespace per SDK

public class SpacetimeClient : MonoBehaviour {
  async void Start() {
    var db = await Database.Connect("ws://127.0.0.1:3000", "mmorpg_server");
    await db.CallReducer("register_player");
    await db.CallReducer("set_player_name", new object[]{ "Hero" });
    db.Subscribe("player", rows => { /* update UI */ });
    db.Subscribe("world", rows => { /* update world tick */ });
  }
}
```

Docs
- Unity tutorial: https://spacetimedb.com/docs/unity
- C# SDK quickstart: https://spacetimedb.com/docs/sdks/c-sharp/quickstart
