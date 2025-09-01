#!/usr/bin/env python3
"""
Create 10 hand-crafted space‑punk biomes for 5km regions.
Output: data/biomes.json
"""
import json, os, random, hashlib

BIOMES = [
    ("Neon Metropolis",   {"sky":"#0b0016","glow":"#10e0ff","accent":"#ff2bdc"}, [
        ("Neon Raider",1.6),("Drone",1.4),("Corp Enforcer",1.2),("Street Samurai",1.4),("Synth Cultist",1.1)]),
    ("Industrial Wastes",  {"sky":"#101010","glow":"#39ff14","accent":"#ffa000"}, [
        ("Scrap Beast",1.6),("Junkbot",1.7),("Gutter Pirate",1.2),("Drone",1.1)]),
    ("Void Marsh",         {"sky":"#070b12","glow":"#8a2be2","accent":"#00eaff"}, [
        ("Voidspawn",1.8),("Synth Cultist",1.3),("Drone",1.0)]),
    ("Plasma Lagoons",     {"sky":"#00121e","glow":"#00bfff","accent":"#ff9d00"}, [
        ("Drone",1.2),("Voidspawn",1.2),("Bio-Construct",1.5)]),
    ("Nanite Forest",      {"sky":"#06140a","glow":"#39ff14","accent":"#00eaff"}, [
        ("Nanite Swarm",1.8),("Bio-Construct",1.4),("Junkbot",1.1)]),
    ("Orbital Graveyard",  {"sky":"#030b18","glow":"#b0c4de","accent":"#a0ffea"}, [
        ("Drone",1.8),("Scrap Beast",1.3),("Voidspawn",1.2)]),
    ("Cryo Tundra Colony", {"sky":"#0b1d33","glow":"#a0ffea","accent":"#89cff0"}, [
        ("Corp Enforcer",1.4),("Drone",1.2),("Bio-Construct",1.2)]),
    ("Sunken Arcology",    {"sky":"#000d14","glow":"#00eaff","accent":"#39ff14"}, [
        ("Bio-Construct",1.6),("Voidspawn",1.3),("Drone",1.1)]),
    ("Reactor Badlands",   {"sky":"#1a0e00","glow":"#ff9d00","accent":"#39ff14"}, [
        ("Scrap Beast",1.5),("Gutter Pirate",1.4),("Junkbot",1.3)]),
    ("Sentient Sands Expanse",     {"sky":"#120a18","glow":"#ff2bdc","accent":"#00eaff"}, [
        ("Neon Raider",1.5),("Street Samurai",1.3),("Drone",1.2)])
]

PROPS = {
    "Neon Metropolis": ["NeonSign","HoloBillboard","TrafficDrone","SkyscraperChunk","Monorail"] ,
    "Industrial Wastes": ["PipeCluster","ToxicBarrel","Crane","Smokestack","ScrapPile"],
    "Void Marsh": ["DarkReed","VoidFumarole","RiftCrystal","MistEmitter","ShallowPool"],
    "Plasma Lagoons": ["PlasmaPool","CoolingTower","ArcLight","Conduit","BridgeSpan"],
    "Nanite Forest": ["NanoTree","BloomingCircuit","HiveNode","GleamMoss","FlowingBots"],
    "Orbital Graveyard": ["DerelictHull","SatelliteHeap","SolarSail","TrussField","Capsule"],
    "Cryo Tundra Colony": ["HabDome","IceSpire","WindTurbine","SupplyCrate","ThermalVent"],
    "Sunken Arcology": ["DrownedAtrium","GlassTube","OvergrownPlaza","Pump","AlgaeStack"],
    "Reactor Badlands": ["CoolingFin","CrackVent","SpentRodCrate","GeigerArch","HeatSink"],
    "Sentient Sands Expanse": ["HoloCactus","MirageEmitter","SignalObelisk","Scanner","Shard"],
}

def seed_for(i,name):
    h = hashlib.sha256(f"{i}-{name}".encode()).hexdigest()
    return int(h[:16],16)

def main():
    out_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(out_dir, exist_ok=True)
    size = 5000
    chunk = 100
    out = []
    for i,(name,palette,weights) in enumerate(BIOMES, start=1):
        out.append({
            "id": i,
            "name": name,
            "seed": seed_for(i,name),
            "size_meters": size,
            "chunk_size_m": chunk,
            "palette": palette,
            "lighting": {"bloom": True, "exposure": 1.0 + (i%3)*0.1, "fog_color": palette.get("sky","#000000")},
            "vfx": ["sparks","steam","haze"] if i%2==0 else ["mist","drizzle","embers"],
            "props": PROPS[name],
            "spawn_weights": [{"family": fam, "multiplier": mult} for fam,mult in weights],
            "description": f"A dense {name} region, 5km across, with space‑punk aesthetic, unique props, and tuned spawns."
        })
    with open(os.path.join(out_dir,'biomes.json'),'w') as f:
        json.dump({"count":len(out),"biomes": out}, f, indent=2)
    print("Wrote", len(out), "biomes")

if __name__ == '__main__':
    main()
