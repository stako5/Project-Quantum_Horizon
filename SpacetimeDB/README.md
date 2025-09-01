MMORPG Mobile Development Plan with SpacetimeDB
Executive Summary
Building a mobile MMORPG that delivers the full desktop experience requires careful architectural planning, optimized development workflows, and strategic use of SpacetimeDB's unique capabilities. This comprehensive development plan outlines a phased approach to create a mobile MMORPG that maximizes desktop-like gameplay while optimizing for mobile constraints.

Phase 1: Foundation & Architecture (Months 1-3)
1.1 SpacetimeDB Setup & Integration
Week 1-2: Environment Setup

Install SpacetimeDB CLI and set up development environment

Create initial database schema with core tables (players, world, items)

Implement basic reducer functions for player authentication and world state

Set up Unity project with SpacetimeDB SDK integration

Configure network manager and connection handling

Week 3-4: Core Architecture

Design modular reducer architecture for game systems

Implement spatial partitioning for world management

Set up subscription queries for real-time data synchronization

Create basic authentication system with secure token handling

Establish coding standards and project structure

Key SpacetimeDB Features to Leverage:

Real-time subscriptions for seamless state synchronization

Transactional reducers for secure game logic execution

Built-in ACID compliance for data integrity

Automatic client cache management

1.2 Mobile-Optimized Unity Setup
Performance Configuration:

Target 60 FPS on mid-range devices (5+ years old)

Implement draw call optimization (150 draw calls maximum)

Configure texture compression (ETC2 for Android, PVRTC for iOS)

Set up LOD (Level of Detail) systems for 3D models

Implement object pooling for frequently spawned entities

Platform Optimization:

Configure build settings for iOS and Android

Set up adaptive quality settings based on device performance

Implement battery-friendly coding practices

Confi


NPC Cognitive Memory & Life Sim
- Server tables: `npc`, `npc_state`, `npc_memory`, `npc_schedule`; reducers: `import_npcs`, `tick_npcs`, `npc_remember`.
- Content: generator at `SpacetimeDB/design/npcs/generate_npcs.py` writes 500 NPCs with lore + daily schedules to `design/npcs/data/npcs.json`.
- Client cognition:
  - `NpcCognitiveMemory`: episodic memory with decay, valence/arousal, query by tags.
  - `NpcPersonality`: traits/preferences influencing decision scores.
  - `NpcCognition`: derives mood from memory + personality; records actions.
  - Utility actions (`AI/Actions`): Wander/Work/Socialize/Rest/Sleep consult cognition to score/execute.
- Bootstrap: `NpcsBootstrapper` imports NPCs and ticks simulation periodically; Overworld scene spawns a sample cognitive NPC.

Playable Alpha — Quick Setup
- Player prefab/root:
  - Add `Health`, `PlayerController`, `BuffManager`, `Items.Inventory`, `Items.PickupCollector`, `UI.PlayerHUDLink`.
  - Optional: `Combat.LockOnSystem` and `UI.SoulsHUD` canvas in scene.
  - Optional: `Player.PlayerHitReact` and configure your Animator to listen for a `HitReact` trigger.
- Enemy prefab used by `ProcGenConfig.enemyRules`:
  - Ensure it has a visible model and collider. At runtime spawns get:
    - `Combat.Health`, `Combat.DamageNumberSource`, `Combat.LootDropper`, `Enemies.EnemyDeathWatcher`,
      `Enemies.EnemyPersonality` (O/C/E parsed), `AI.UtilityAgent`, `Enemies.EnemyPerception`, `Enemies.ThreatTable`, `Enemies.EnemyMover`, `Enemies.EnemyTargeting`, `Enemies.EnemyMeleeAttack`, `Enemies.EnemyStatScaler`, `Enemies.EnemyBrain`.
- World streaming:
  - In a scene, add `World.WorldStreamer` and assign a `ProcGenConfig` with `enemiesJson` and enemy spawn rules.
  - Press Play; enemies spawn with personality-driven Utility AI (Strike/Lunge/Backstep/Roar/RangedShot).

What’s Included (Client)
- Utility AI upgrades: cooldowns, minimum action durations, action memory hooks.
- Enemy brain & targeting: personality-based scoring + leash behavior, NavMesh movement adapter, LOS/hearing perception, threat table with group aggro share.
- Ranged combat: simple projectile system (`Combat.Projectile` + `ProjectileShooter`).
- Projectile pooling + telegraph: pooled projectiles and a short `Telegraph.FlashLine` before firing.
- Combat feedback: floating damage numbers (`UI.DamageNumbers` via `DamageNumberSource`).
- Loot & progression: `LootDropper` spawns gold + XP pickups; `Items.Inventory` tracks gold; `Player.PlayerProgress` tracks XP/levels.
- Drops: `Items.DropTable` + `Items.DropsRegistry` drive item drops per family/tier; `LootDropper` supports table overrides.
- HUD link: `UI.PlayerHUDLink` feeds SoulsHUD HP/Stamina and gold.
- Inventory UI: `UI.InventoryUI` lists gold and items; `UI.PickupToastUI` shows pickup toasts.
- Death/respawn: `Player.PlayerDeathRespawn` fades, revives, and teleports to a spawn point (settable by shrines later).
- Local quests: `Quests.Local` (QuestDefinition, LocalQuestList, LocalQuestTracker) + `UI.LocalQuestUI` for a simple Kill/Collect chain.
- Quests menu: `UI.QuestsMenuController` toggles the quests panel (default key J) and `UI.QuestsButtonLink` lets any HUD button toggle it.
 - HUD Quests button: `UI.SoulsHUD` auto-adds a bottom-right "Quests" button at runtime if one isn’t present (disable via `autoAddQuestsButton`).
 - Action timing: windup/active/recovery phases with optional root windows per action.
- Hit-stop: brief global time slowdown on hits for better impact.
- Enemy melee: strike/lunge enable a timed hitbox with windup telegraph lines.
- Stat scaling: HP/DMG/SPD scaled by tier/stage/size with role modifiers and hard caps.
- Tunable scaling: create `Resources/EnemyStatScaling.asset` to customize tier steps, size/stage/role multipliers, and caps.
- Debug panel: `UI.EnemyStatsDebugUI` shows live scaled stats for the nearest enemy (toggle with F3).
- Editor helper: `MMORPG/Enemy Tools` creates/selects the scaling asset for quick edits.
 - Gizmos: `Enemies.EnemyMover` draws home (green) and leash (cyan) in Scene view.
 - Custom inspector: `Enemies.EnemyMover` inspector exposes home/leash fields with a "Set Home = Current" button.
 - AI debug: `AI.Debug.UtilityAIDebugLabel` shows current action/phase and cooldowns (toggle with F2), attach to enemies.

Next Suggested Additions
- NavMesh-based movement adapter for actions (pathfinding, obstacle avoidance).
- Projectile pooling + impact VFX/SFX; telegraphs for heavy attacks.
- Basic Inventory UI + hotbar; item rarities/affixes and vendors.
- Threat/aggro table with group sharing; perception (LOS/hearing) and return-to-home movement.
- Boss phases/mechanics using scripted state machines and arena telegraphs.

NavMesh Notes
- The `Enemies.EnemyMover` requires a baked NavMesh in the scene (or a runtime NavMeshSurface) for pathfinding. If no NavMesh is present, actions gracefully fall back to direct transform movement.
- Add `World.NavMeshBootstrap` to a scene to attempt runtime building via `NavMeshSurface` if the AI Navigation package is installed. Otherwise, bake offline or import the package.

Projectile Pooling & Telegraph
- Add a singleton `Combat.ProjectilePool` anywhere (optional). The shooter will auto-create one if missing.
- Ranged enemies use `Telegraph.FlashLine` a short moment before each shot; tweak `telegraphLead` in `EnemyActionRangedShot` for feel.

Damage Numbers & Impacts
- `UI.DamageNumbers` now pools TextMesh instances automatically. Add one to your bootstrap scene to configure prewarm/font, or let it auto-create.
- Projectiles spawn a simple billboard flash (`VFX.ImpactVFX`) and call `Audio.SfxPlayer.PlayImpact`. Assign a default impact clip on `Audio.SfxPlayer` in a bootstrap scene.
- Impact VFX are pooled via `VFX.VFXPool` to reduce allocations.
- Ranged enemies now show a short `VFX.MuzzleFlash` at the shooter origin.
Live Roadmap & TODOs
- For the current prioritized checklist and acceptance criteria, see: ./LIVE_README.md

Sample Content
- Use Editor menu:
  - `MMORPG/Generate Sample DropTables` to create `Resources/DropTables` for Rift Stag/Harmonic Jelly.
  - `MMORPG/Generate Sample Local Quests` to create `Resources/Quests/LocalQuests.asset` and example quests.

First-Person Mode
- Add to player root:
  - `Player.FirstPersonController` for movement/attacks
  - A child object for head/camera with `Player.FPSCamera` (it will auto-create a Camera if missing)
- Disable or remove the old `Player.PlayerController` to avoid double input. You can preserve the same weapon hitbox setup.
- Set `FPSCamera.lockCursor=true` for mouse-locked aiming.

Desktop Optimization (Windows/macOS/Linux)
- Add `Boot.PlatformOptimizer` to a bootstrap GameObject in your first scene.
  - Sets `targetFrameRate`, disables VSync (optional), enables anisotropic filtering, configures MSAA and shadow quality.
  - Applies small platform-specific tweaks (`UNITY_STANDALONE_WIN/OSX/LINUX`).

Options Panel
- Add an in-game options panel to your Canvas:
  - Add `UI.QualityDropdown` to a `Dropdown` for quality presets.
  - Add `UI.DisplayOptions` to a parent and assign:
    - `resolutionDropdown` (list of available resolutions)
    - `fullscreenToggle` (toggles exclusive fullscreen/windowed)
    - `vsyncToggle` (switch VSync on/off)
  - Optionally add `Boot.PlatformOptimizer` in the scene to set sensible defaults at startup; users can override via the options panel.

Auto FPS Bootstrap (optional)
- Add `Boot.AutoFPSSetup` to a bootstrap GameObject:
  - Assign `playerRoot` or let it find the Player by tag or existing `PlayerController`.
  - It disables the third-person `PlayerController`, adds `CharacterController` + `FirstPersonController`, and spawns a `Head` with `FPSCamera` if missing.

Shrines
- `World.SpawnShrine`: add to a shrine prefab with a trigger collider to set the player's respawn point on contact. Plays a small VFX/SFX on attune.

Quests Panel Setup
- Create a Canvas panel for quests with a `CanvasGroup` and child scroll `content` and hidden `rowTemplate` `Text`.
- Add `UI.LocalQuestUI` to that panel and assign `content` + `rowTemplate`.
- Add `UI.QuestsMenuController` to the panel; set `startHidden` and the `toggleKey` if desired.
- To add a HUD button, place a `Button` in your HUD Canvas, add `UI.QuestsButtonLink`, and wire the Button OnClick to `QuestsButtonLink.CallToggle()`.
