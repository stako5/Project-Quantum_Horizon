#!/usr/bin/env python3
"""
Generate 20 unique dungeon bosses with puzzle modules and mythic armor sets.
Output: data/bosses.json
"""
import json, os, random, hashlib

BIOMES = [
    "Neon Metropolis","Industrial Wastes","Void Marsh","Plasma Lagoons","Nanite Forest",
    "Orbital Graveyard","Cryo Tundra Colony","Sunken Arcology","Reactor Badlands","Hologram Dunes"
]
MECHS = ["phase-shift","gravity-well","adds-wave","split-form","reflect-shield","arena-rotate","hazard-flood","energy-tethers","stack-spread","memory-runes"]
PUZZLES = ["glyph-keys","pressure-plates","laser-mirror","battery-routing","pattern-match","rune-dials","weight-scales","pipe-valves","holo-bridge","code-switches"]
PHASES = ["burn","adds","burst","hazard","soft-enrage","interlude"]
SLOTS = ["helm","chest","gloves","legs","boots"]

BIOME_LORE = {
    "Neon Metropolis": [
        "Neon rain hisses on overheated steel as monorails carve light through the skyline.",
        "Corporate ghosts bargain in the glass, and every reflection lies.",
        "Billboards stutter prophecies between frames you weren't meant to see."
    ],
    "Industrial Wastes": [
        "Toxic winds comb through skeletonized refineries, singing in rusted teeth.",
        "Scavenger choirs drum on barrels that still remember the last meltdown.",
        "Pressure valves exhale prayers in scintillating steam."],
    "Void Marsh": [
        "Low fog clots around reality's bruises where the world sinks into Null.",
        "Each reed is a metronome for the far tide beyond causality.",
        "Rifts blink like cold stars beneath your feet."],
    "Plasma Lagoons": [
        "Azure arcs stitch the water; every ripple carries the taste of thunder.",
        "Cooling spires hum a lullaby they learned from storms.",
        "Bridges sway over irradiant ponds that dream in current."],
    "Nanite Forest": [
        "Leaves are algorithms; bark is compiled; roots gossip in swarms.",
        "Every breeze rearranges decisions made a heartbeat ago.",
        "Light diffracts through clouds of helpful teeth."],
    "Orbital Graveyard": [
        "Dead constellations of hulls drift in slow elegies.",
        "Sails rattle with the last sunlight they ever caught.",
        "Air tastes like old vacuum and powdered orbit."],
    "Cryo Tundra Colony": [
        "Frost gardens bloom on abandoned domes, each petal a memory of warmth.",
        "Wind writes thin scriptures across ice corridors.",
        "Heaters groan awake, unwilling saints of survival."],
    "Sunken Arcology": [
        "Drowned boulevards sleep beneath algae-lit vaults.",
        "Elevators ferry bubbles past statues of drowned ambition.",
        "Glass halls echo with whalesong from broken speakers."],
    "Reactor Badlands": [
        "Cracked vents cough daylight; the earth sweats mercury.",
        "Heat mirages deal in counterfeit horizons.",
        "Geiger winds rattle the bones of outlived machines."],
    "Hologram Dunes": [
        "Mirages argue with each other until one becomes real.",
        "Signal obelisks chant coordinates to nowhere.",
        "Sand remembers the footprints of people who never existed."]
}

def seed(name):
    return int(hashlib.sha256(name.encode()).hexdigest()[:16],16)

def mythic_piece(boss, slot):
    return {
        "slot": slot,
        "name": f"{boss} {slot.title()}",
        "bonus": f"{random.choice(['+Haste','+Power','+FP','+DR','+Crit'])} vs bosses"
    }

ADJ = ["Neon", "Chromatic", "Abyssal", "Obsidian", "Holographic", "Iridescent", "Static", "Null", "Phosphor", "Cataclysmic", "Ultraviolet", "Gravitic"]
NOUN = ["Archon", "Seraph", "Colossus", "Chimera", "Oracle", "Harbinger", "Paragon", "Specter", "Leviathan", "Catalyst", "Monolith", "Sovereign"]
OF = ["of the Void", "of Nullspace", "of Neon Night", "of Broken Axiom", "of the Reactor", "of Lost Signals", "of Last Light", "of Eventide", "of the Pale Sun", "of Parallax"]
EPITHET = ["Prime", "Eternal", "Eclipse", "Omniscient", "Omega", "Singularity", "Transcendent", "Anomalous", "Divine"]

def creative_name(rng):
    style = rng.randint(0,2)
    if style == 0:
        return f"The {rng.choice(ADJ)} {rng.choice(NOUN)} {rng.choice(OF)}"
    elif style == 1:
        return f"{rng.choice(NOUN)} {rng.choice(EPITHET)}"
    else:
        return f"{rng.choice(ADJ)} {rng.choice(NOUN)}"

def make_boss(i):
    rng = random.Random(seed(f"boss-{i}"))
    name = creative_name(rng)
    biome = rng.choice(BIOMES)
    tier = rng.randint(3,5)
    mechanics = rng.sample(MECHS, k=3)
    puzzle_modules = rng.sample(PUZZLES, k=3)
    phases = rng.sample(PHASES, k=3)
    set_id = f"set_{i:02}"
    pieces = [ mythic_piece(name, s) for s in SLOTS ]
    set_bonuses = ["(2) +5% damage to elites","(4) +10% ability haste","(5) Unique: unleash a mythic burst on dodge"]
    armor = {"id": set_id, "name": f"Mythic {name}", "pieces": pieces, "set_bonuses": set_bonuses}
    # Loreful description
    lore_lines = BIOME_LORE.get(biome, [])
    line1 = lore_lines[0] if lore_lines else f"The air in {biome} hums with consequence."
    line2 = lore_lines[1] if len(lore_lines) > 1 else "Somewhere nearby, probability wavers."
    line3 = f"{name} stirs — a conductor of {', '.join(mechanics[:2])} and ritual." 
    line4 = f"Their lair reconfigures itself with {', '.join(puzzle_modules)} until your choices fit the pattern."
    line5 = f"Survive the {phases[0]} and {phases[1]} to face the {phases[2]} that writes your name into the coolant."
    desc = " ".join([line1, line2, line3, line4, line5])
    return {
        "id": f"boss_{i:02}",
        "name": name,
        "biome": biome,
        "tier": tier,
        "mechanics": mechanics,
        "puzzle_modules": puzzle_modules,
        "phases": phases,
        "armor_set": armor,
        "description": desc
    }

def main():
    out_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(out_dir, exist_ok=True)
    bosses = [ make_boss(i+1) for i in range(20) ]
    with open(os.path.join(out_dir, 'bosses.json'), 'w') as f:
        json.dump({"count": len(bosses), "bosses": bosses}, f, indent=2)
    print("Wrote", len(bosses), "bosses")

if __name__ == '__main__':
    main()
