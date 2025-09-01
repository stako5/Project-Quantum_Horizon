#!/usr/bin/env python3
"""
Generate 10 world bosses (one per biome) with random names and environment effects.
Output: data/world_bosses.json
"""
import json, os, random, hashlib

BIOMES = [
    "Neon Metropolis","Industrial Wastes","Void Marsh","Plasma Lagoons","Nanite Forest",
    "Orbital Graveyard","Cryo Tundra Colony","Sunken Arcology","Reactor Badlands","Sentient Sands Expanse"
]

# Canonical, biome-themed, one-of-a-kind names (exactly one boss per biome)
CANONICAL_NAME = {
    "Neon Metropolis": "Skyline Sovereign of Neon Metropolis",
    "Industrial Wastes": "Rust-Crown Warden of the Wastes",
    "Void Marsh": "Null-Tide Leviathan of the Marsh",
    "Plasma Lagoons": "Azure Reactor Colossus",
    "Nanite Forest": "Swarm-Heart Paragon of the Nanite Wilds",
    "Orbital Graveyard": "Helios-Cemetery Archon",
    "Cryo Tundra Colony": "Glacier Seraph of the Colony",
    "Sunken Arcology": "Drowned Oracle of the Arcology",
    "Reactor Badlands": "Radstorm Behemoth of the Badlands",
    "Sentient Sands Expanse": "Sentient Dune Sovereign",
}

def seed(*parts):
    s = "::".join(map(str,parts))
    return int(hashlib.sha256(s.encode()).hexdigest()[:16],16)

def env_for(biome, rng):
    # Simple env effect config: fog, exposure, hue shift, ambient boost
    palettes = {
        "Neon Metropolis": {"fog_color":"#0b0016","fog_density":0.03,"hue":200,"exposure":1.2},
        "Industrial Wastes": {"fog_color":"#202010","fog_density":0.06,"hue":90,"exposure":0.9},
        "Void Marsh": {"fog_color":"#0a0f18","fog_density":0.08,"hue":260,"exposure":0.8},
        "Plasma Lagoons": {"fog_color":"#00121e","fog_density":0.04,"hue":190,"exposure":1.1},
        "Nanite Forest": {"fog_color":"#06140a","fog_density":0.05,"hue":130,"exposure":1.0},
        "Orbital Graveyard": {"fog_color":"#050d16","fog_density":0.05,"hue":220,"exposure":0.95},
        "Cryo Tundra Colony": {"fog_color":"#102030","fog_density":0.03,"hue":200,"exposure":1.15},
        "Sunken Arcology": {"fog_color":"#001018","fog_density":0.07,"hue":185,"exposure":0.9},
        "Reactor Badlands": {"fog_color":"#1a0e00","fog_density":0.05,"hue":35,"exposure":1.05},
        "Hologram Dunes": {"fog_color":"#120a18","fog_density":0.06,"hue":300,"exposure":1.0},
    }
    base = palettes.get(biome, {"fog_color":"#000000","fog_density":0.04,"hue":180,"exposure":1.0})
    base["ambient_boost"] = round(0.1 + rng.random()*0.3, 2)
    base["threat_radius_m"] = 200 + rng.randint(0,200)
    return base

def main():
    out_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(out_dir, exist_ok=True)
    bosses = []
    for i, biome in enumerate(BIOMES, start=1):
        rng = random.Random(seed("world-boss", i, biome))
        # One-of-a-kind, biome-themed name
        name = CANONICAL_NAME.get(biome, f"World Boss of {biome}")
        env = env_for(biome, rng)
        # Stable, biome-based id for uniqueness
        slug = biome.lower().replace(' ', '_')
        bosses.append({
            "id": f"world_boss_{slug}",
            "name": name,
            "biome": biome,
            "tier": 5,
            "env_effect": env,
            "min_ng_plus": 3,
            "description": f"An endgame horror that bends {biome} to its will. Its presence warps fog, light, and color within {env['threat_radius_m']}m."
        })
    with open(os.path.join(out_dir, 'world_bosses.json'), 'w') as f:
        json.dump({"count": len(bosses), "world_bosses": bosses}, f, indent=2)
    print("Wrote", len(bosses), "world bosses")

if __name__ == '__main__':
    main()
