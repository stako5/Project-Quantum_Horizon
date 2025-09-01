MMORPG Classes, Trees, Weapons, Abilities, Perks (Design)

Goals
- Endgame balance parity across classes; differentiation via skill expression, counters, and team roles.
- Clear class fantasies with three-branch class trees (Offense/Control/Support variants per class theme).
- Weapons with random, class-tailored perks; each class supports 100 distinct perks; 50 abilities per class.

Classes (11)
1) Time Mage
- Story: Chrono‑engineers who thread timelines to accelerate allies, rewind mistakes, and fracture reality. Exiled after a failed temporal containment, they now police paradox.
- Role: Control/Support caster with tempo manipulation and limited time rollback.
- Resource: Chrono Charge (build/spend via time effects).
- Trees: Acceleration (haste, cds), Reversion (rewind, undo), Fracture (time-rifts, burst).
- Weapons: Chronometer Staffs, Temporal Orbs, Time Blades (short sabers).

2) Warrior
- Story: Ex‑legion shocktroopers hardened by industrial wars, carrying scars and stubborn resolve.
- Role: Bruiser/tank with guard, taunt, execution windows.
- Resource: Rage (gains when hit/attacking).
- Trees: Vanguard (tanking), Berserk (sustained DPS), Tactician (counters, stance swaps).
- Weapons: Greatblades, Warhammers, Tower Shields + Shortsword.

3) Blade Master
- Story: Ascetics of steel who write poetry with edges; perfection through motion.
- Role: Precision melee with stances, parries, and weak‑point strikes.
- Resource: Focus (earned via perfect timings).
- Trees: Iaido (openers, dash), Riposte (parry/counter), Tempest (multi‑hit flurries).
- Weapons: Katanas, Dual Sabers, Poleblades.

4) Void Mage
- Story: Scholars who listen to the void’s hum, bending entropy and gravity.
- Role: Control/Burst caster: singularities, entropy burns, silences.
- Resource: Entropy (build with drains, spend on singularities).
- Trees: Singularity (pull, crush), Entropic (decay/DoT), Silence (anti‑caster tools).
- Weapons: Graviton Staves, Void Foci, Black Daggers.

5) Life Mage
- Story: Bioluminescent healers who weave growth, thorns, and renewal.
- Role: Healer/support with reactive shields and delayed blooms.
- Resource: Vitality (passive regen, spend for surges).
- Trees: Bloom (burst heals), Thorns (reflect, guard), Renewal (HoTs, revives).
- Weapons: Living Staves, Seed Scepters.

6) Nanobots Mage
- Story: Post‑human engineers commanding clouds of programmable matter.
- Role: Pet‑swarm control/support, reconfigure on demand.
- Resource: Nanite Swarm (pool shared by constructs).
- Trees: Swarm (drones, locus), Forge (constructs, turrets), Assimilate (steal/replicate buffs).
- Weapons: Nanite Gauntlets, Fabricators.

7) Dimensional Walker
- Story: Drifters stepping between branes, trading position for probability.
- Role: Skirmisher with teleports, phase shields, and position‑based crits.
- Resource: Flux (build via movement/phase, spend on blinks).
- Trees: Phase (invuln windows), Rift (tele‑traps), Momentum (chain mobility + crits).
- Weapons: Phase Lances, Blink Knives.

8) Entity Summoner
- Story: Pact‑makers who bind entities (spirits, constructs) to their will.
- Role: Pet commander; hybrid support/DPS via commands and formations.
- Resource: Command (cap per entity; upkeep).
- Trees: Legion (multiple minor summons), Avatar (one major), Conclave (synergy auras).
- Weapons: Sigil Tomes, Summoner Rings.

9) Death Mage
- Story: Keepers of the last door who harvest soul‑ash to bargain with endings.
- Role: DoT/execute caster with soul economy and curses.
- Resource: Soul Ash (earned on damage/kill).
- Trees: Woe (curses), Harvest (executes), Necrosis (rots/DoTs).
- Weapons: Reaper Scythes, Bone Wands.

10) Monk
- Story: Cyber‑monastics who discipline body and circuit, channeling chi through implants.
- Role: Sustained melee with counters, inner shields, team auras.
- Resource: Chi (flows; stance‑dependent regen).
- Trees: Iron Body (tanks), Flow (counterplay), Harmony (auras/support).
- Weapons: Fist Blades, Quarterstaves, Tonfa.

11) Jack of All Trades
- Story: Improvisers who stitch skills from many paths; masters of adaptability.
- Role: Versatile hybrid; can fill gaps but excels with planning and combos.
- Resource: Versatility (adapts based on last 3 actions).
- Trees: Toolkit (utility), Improvisation (combo chains), Specialist (pick sub‑class packages).
- Weapons: Modular Tools, Multi‑blades.

Class Trees (Structure)
- 3 branches per class; 5 tiers each; 3 nodes per tier; 1 capstone per branch.
- Nodes unlock by tier; capstones require 5 nodes in branch. Cross‑branch synergies via tags.

Weapons & Random Perks
- Each class weapon family has a pool of 100 class‑specific perks. Perks roll with tiers (Common/Rare/Epic/Legendary) and numeric ranges balanced by slot budget.
- Perk categories: Offense, Defense, Utility, Resource, Mobility, Control.

Ability & Perk Counts
- 50 abilities per class: 30 actives (10 per branch), 12 passives (4 per branch), 6 exotics (cross‑branch), 2 ultimates, 2 class mechanics.
- 100 perks per class: 70 generics (class‑flavored), 30 signature (branch/capstone synergies).

Balance Framework
- Shared budgets: every build spends the same total on damage, mitigation, control, mobility, and sustain; different classes arrange them differently.
- Counterplay triangles: burst vs sustain vs control; mobility vs zone; pets vs cleave.
- PvE/PvP scalars: individual ability PvP modifiers; normalize endgame DPS/TTK windows.
- Skill expression: tighter timings (parry, invuln frames), resource foresight (bank/spend), and positioning give advantages without breaking budgets.

Data Schema & Generation
- See `schema.json` for Ability/Perk JSON.
- Use `generate_content.py` to produce 50 abilities + 100 perks per class using deterministic seeds. Samples included for Time Mage and Warrior.

