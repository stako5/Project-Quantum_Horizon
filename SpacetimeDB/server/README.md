MMORPG Server (SpacetimeDB)

- Language: Rust (SpacetimeDB module)
- Tables: `player`, `world`, `item`, `world_config`
- Reducers: `register_player`, `set_player_name`, `increment_world_tick`, `grant_item`, `init_world`, `set_avatar`, `clear_class_data`, `import_abilities`, `import_perks`, `roll_weapon`, `clear_enemy_types`, `import_enemy_types`, `clear_world_regions`, `import_world_regions`, `clear_bosses`, `import_bosses`
  - Indexing/catalog: `refresh_class_catalog`

Prereqs
- Rust + Cargo (1.88+)
- SpacetimeDB CLI (see https://spacetimedb.com/docs/cli-reference)

Build Locally
- From this folder: `cargo check`
- With CLI (after install):
  - `spacetime build` (compiles the module to WASM)

Key Files
- `src/lib.rs`: Tables + reducers

Next Steps
- Add auth and RLS: use `Identity` + row-level security to restrict reads/writes
- Add subscription queries for clients (Unity/TS) to track world/player updates
- Wire Unity client to call reducers and subscribe to public tables

World Configuration
- Table: `world_config` (public) stores `seed`, `size_meters`, `chunk_size_m`.
- Reducer: `init_world(seed, size_meters, chunk_size_m)` sets or updates config (id=1).
- Suggested defaults: `seed=1337`, `size_meters=5000`, `chunk_size_m=100`.

Content Import (classes/abilities/perks)
- Tables: `ability`, `perk`, `perk_catalog`, `weapon_instance`, `weapon_perk`.
- Catalog: `class_catalog` (public) lists available classes for clients; maintained via imports or `refresh_class_catalog`.
- Clear + import flow per class:
  1) `clear_class_data(class_name)`
  2) `import_abilities(class_name, abilities_json)` with the JSON generated in `design/classes/.../full/abilities.json`.
  3) `import_perks(class_name, perks_json)` with the JSON generated in `design/classes/.../full/perks.json`.
- Loot roller: `roll_weapon(class_name, weapon_type, level, num_perks)` inserts a `weapon_instance` plus its `weapon_perk` rows for the caller (`ctx.sender`).

Avatar Customization
- Table: `avatar` holds validated player avatar data (height, weight, body/face JSON) keyed by `Identity`.
- Reducer: `set_avatar(avatar_json)` validates anthropometric constraints and upserts the caller's avatar.
- Enemies: `design/enemies/generate_enemies.py` produces `data/enemies.json`. Import with `import_enemy_types` (use `clear_enemy_types` to reset).
- Biomes/Overworld: `design/biomes/generate_biomes.py` produces `data/biomes.json` for 10x 5km regions.
  - Import with `clear_world_regions` then `import_world_regions`.
- Bosses/Dungeons/Armor: `design/bosses/generate_bosses.py` produces `data/bosses.json` (20 bosses with puzzle modules and mythic armor sets).
  - Import with `clear_bosses` then `import_bosses`.
  - Loot: call `award_boss_loot(boss_id, mode)` where `mode` is `"all_missing"` (default) or `"random"`.
    - If all pieces owned, awards 10 `mythic_shards` in `player_currency`.
