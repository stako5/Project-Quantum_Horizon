Procedural World (5km) Setup

Goal
- Generate a 5km x 5km space‑punk world, chunked and streamed around the player.

What’s Included
- Config asset: `ProcGenConfig` ScriptableObject defines world size (default 5000m), chunk size (100m), view distance, densities, and spawn rules.
- Runtime streaming: `WorldStreamer` loads/unloads `Chunk` objects around the player.
- Scatter: each `Chunk` uses seeded sampling per rule to place props at specified densities.

Quick Start
- Create `Assets/Game/ProcGenConfig.asset` via Create → MMORPG → ProcGen Config.
- Set World Size = 5000, Chunk Size = 100, View Distance = 5–8.
- Assign a ground tile prefab (1×1 plane scaled by code to chunk size) or keep empty if using a Terrain.
- Add Spawn Rules for your CC0/permissive prefabs and set density per hectare.
- Drop `WorldStreamer` into a bootstrap scene, assign the Config and Player.

Notes
- 1 Unity unit = 1 meter. 5km = 5000 units. With 100m chunks, that’s 50×50 chunks.
- Densities are per hectare (10,000 m²). The system samples a Poisson count per chunk and scatters with minimum spacing.
- Replace y=0 placement with proper terrain sampling if using Unity Terrain.
- Everything is deterministic from the seed + chunk coord, so all clients match.

Server Coordination (optional)
- Server table `world_config` exposes `seed`, `size_meters`, `chunk_size_m`.
- Reducer `init_world(seed, size_meters, chunk_size_m)` to set config. Clients can read public table to match settings.

