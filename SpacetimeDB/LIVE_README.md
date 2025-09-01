Live README — Build Roadmap and TODOs

Purpose
- Track current state, next steps, and acceptance criteria to reach a fully playable game and beyond.
- Kept concise and actionable; update as features land.

Quick Start Checklist
- [x] Overworld spawns with AI (Utility, personality-driven)
- [x] Basic combat (melee/ranged, cooldowns, windups, leashing)
- [x] Damage numbers, hit-stop, impact VFX/SFX, muzzle flash, telegraphs
- [x] Loot + pickups + gold/XP; minimal HUD
- [x] Enemy stat scaling by tier/stage/size (tunable), debug panel
- [x] NavMesh movement + runtime bootstrap fallback
- [x] Editor helpers (scaling asset creator, EnemyMover inspector + gizmos)
 - [x] Player death/respawn (spawn point API; shrine later)
- [ ] Save/persist player gold/XP/position (local and SpacetimeDB path)

Current Alpha State (Implemented)
- AI: UtilityAgent with cooldowns/min durations; O/C/E personality biasing; leashing; LOS/hearing; threat table + group aggro.
- Actions: Strike/Lunge/Backstep/Roar/RangedShot with windup/active/recovery and root windows; ranged telegraph + pooled projectiles.
- Combat feel: hit-stop on hits; floating damage numbers (pooled); impact flash + SFX; muzzle flashes.
- Progression: gold + XP from drops; PlayerProgress with level thresholds; Souls-like HUD link.
- Spawner: Chunk-based enemy spawns with identity/personality parsing; auto-inject brain, mover, melee, scaler, loot, numbers.
- Scaling: tier/stage/size/role multipliers with asset-driven tuning and hard caps.
- Tools: Enemy Tools (create/select scaling asset); EnemyMover custom inspector + gizmos; NavMesh runtime rebuild.
- Bestiary: detail panel shows Stage/Size/O/C/E.

Milestones
M1 — Playable Alpha (combat loop complete)
- [x] Player death and respawn (spawn point API; shrine later)
- [x] Basic inventory UI (gold, items grid) + pickup toast
- [x] Enemy drops table (weights, tiers) → item pickups
- [x] Simple quest chain: kill X family, collect Y drops, reward gold/XP
  - [x] Local quests: definition asset + tracker + UI + sample generator
- [x] NavMesh polish: avoid walls during strafes; return-home animation
- [x] Melee telegraph cones/ground decals (in addition to line telegraph)
Acceptance: Start new game → defeat a few enemies → collect drops → complete a quest → die/respawn without softlocks.

M2 — Content Beta (progression systems)
- [ ] XP/level rewards UI; perk selection (3 choices) on level-up
- [ ] Vendors: sell/buy, price curves; basic crafting (upgrade material → better item)
- [ ] Map/minimap with fog-of-war and pins (player, quests, portals)
- [ ] Dungeon portal: one themed boss with 2-phase mechanics + unique reward
- [ ] Party system (local stub UI; server hooks placeholder)
Acceptance: Player can level, pick perks, buy/sell, craft an upgrade, navigate by minimap, and clear one dungeon boss.

M3 — 1.0 Polish & Ops
- [ ] Performance: pooling audit, LOD, occlusion, addressables/bundles
- [ ] Audio pass: layered impacts, footsteps, ambient zones, music stingers
- [ ] Animation pass: blend spaces, root motion where needed, hit reacts
- [ ] CI: editor tests/smoke build; crash reporting; analytics funnels
- [ ] LiveOps: feature flags, AB toggles for enemy scaling and droprates
Acceptance: Stable 60fps target scenes, reproducible builds, crash/analytics wired, content togglable live.

Near-Term Task Backlog (2–3 sprints)
Combat/AI
- [x] Enemy melee decals (cone/arc) using VFXPool; scale by action range
- [ ] Add cooldown UI hints to actions (debug)
- [ ] LOS occlusion layers refinement + memory of last seen target
- [ ] Threat dampening after leash return

UI/UX
- [x] Minimal inventory panel (gold, items grid); pickup toast
- [ ] XP/level bar, perk selection screen prototype
- [ ] Toggleable debug overlays (AI choice, threat, perception)

World/Spawns
- [x] DropsTable ScriptableObject per family/tier; link via LootDropper registry
- [ ] POIs: chest, camp, shrine prefabs and rules; fast travel node

Quests
- [x] Local quest scriptable objects; unify with SpacetimeDB reducers later
- [ ] Quest log entries; simple rewards (gold/XP/items/perks)

Persistence/Backend (SpacetimeDB integration)
- [ ] Persist player base: identity, gold, XP, position
- [ ] Sync bestiary discoveries; spawn stats recorded server-side
- [ ] Rate limit inputs; cooldown validation server-side

Polish/Perf
- [ ] Projectile pooling capacity + warmup; damage numbers pool sizing
- [ ] Physics layers for pickups/damage to reduce overlap scans
- [ ] GPU instancing/material variants for VFX quads

Nice-to-Haves
- [ ] Photo mode (free cam + UI hide)
- [ ] Accessibility: colorblind palette for VFX/telegraphs; text scaling
- [ ] Controller layout + rumble hooks for impacts

How To Contribute
- File small, scoped PRs by milestone feature; keep README and Live README in sync.
- For new systems, include a minimal debug UI and acceptance criteria.

Acceptance Criteria Templates
- Feature works in sample scene and Overworld with WorldStreamer.
- Has a debug toggle or UI to validate at runtime.
- Has reasonable defaults and no hard scene dependencies.

Links
- Main README: ./README.md
- Scaling Asset: Create via menu MMORPG → Enemy Tools, saved at Resources/EnemyStatScaling.asset
