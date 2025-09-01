#!/usr/bin/env python3
"""
Generate biome activity packs: POIs and dynamic events to keep regions dense and varied.
Output: data/activities.json
"""
import json, os

BIOMES = [
    "Neon Metropolis","Industrial Wastes","Void Marsh","Plasma Lagoons","Nanite Forest",
    "Orbital Graveyard","Cryo Tundra Colony","Sunken Arcology","Reactor Badlands","Hologram Dunes"
]

def poi(biome, name, density, tags):
    return {"biome": biome, "name": name, "density_per_hectare": density, "tags": tags}

def event(biome, name, weight, cooldown_chunks):
    return {"biome": biome, "name": name, "weight": weight, "cooldown_chunks": cooldown_chunks}

def main():
    out_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(out_dir, exist_ok=True)
    pois = []
    events = []
    # Simple handcrafted lists per biome
    pois += [
        poi("Neon Metropolis", "HoloMarket", 0.5, ["vendors","quest"]),
        poi("Neon Metropolis", "Monorail Station", 0.3, ["travel","encounter"]),
        poi("Industrial Wastes", "Scrap Bazaar", 0.6, ["craft","quest"]),
        poi("Industrial Wastes", "Cooling Tower Base", 0.4, ["encounter"]),
        poi("Void Marsh", "Rift Shrine", 0.4, ["ritual","event"]),
        poi("Plasma Lagoons", "Bridge Span", 0.5, ["travel","encounter"]),
        poi("Nanite Forest", "Hive Node", 0.6, ["craft","event"]),
        poi("Orbital Graveyard", "Derelict Dock", 0.4, ["quest","encounter"]),
        poi("Cryo Tundra Colony", "Hab Dome Ruin", 0.5, ["quest","encounter"]),
        poi("Sunken Arcology", "Drowned Atrium", 0.5, ["explore","encounter"]),
        poi("Reactor Badlands", "Heat Sink Ridge", 0.4, ["encounter"]),
        poi("Sentient Sands Expanse", "Signal Obelisk", 0.6, ["event","encounter"]),
    ]
    events += [
        event("Neon Metropolis", "Gang Broadcast Jack", 4, 6),
        event("Neon Metropolis", "Billboard Oracle", 3, 4),
        event("Industrial Wastes", "Toxic Leak Containment", 5, 6),
        event("Void Marsh", "Rift Stabilization", 5, 8),
        event("Plasma Lagoons", "Arc Surge", 4, 5),
        event("Nanite Forest", "Swarm Bloom", 5, 7),
        event("Orbital Graveyard", "Black Box Hunt", 4, 6),
        event("Cryo Tundra Colony", "Heater Re-ignite", 4, 6),
        event("Sunken Arcology", "Pressure Gate Unlock", 4, 6),
        event("Reactor Badlands", "Rod Retrieval", 4, 6),
        event("Sentient Sands Expanse", "Mirage Decode", 5, 6),
    ]
    with open(os.path.join(out_dir, 'activities.json'), 'w') as f:
        json.dump({"pois": pois, "events": events}, f, indent=2)
    print("Wrote activities.json")

if __name__ == '__main__':
    main()
