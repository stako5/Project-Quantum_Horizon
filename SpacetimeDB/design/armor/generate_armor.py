#!/usr/bin/env python3
"""
Generate armor sets and perks:
 - 100 world sets (5-piece)
 - 10 faction sets per faction (4 factions * 10 = 40 sets)
 - One unique set per enemy family (uses design/enemies/data/enemies.json families)
 - 500 unique perks
Outputs under design/armor/data/ : world_sets.json, faction_sets.json, enemy_sets.json, perks.json
"""
import json, os, random, hashlib

FACTIONS = ["Parallax Concord","Free Archipelagos","Brass Arc Cartel","Shard Tribunal"]
SLOTS = ["helm","chest","gloves","legs","boots"]
RARITIES = ["Common","Uncommon","Rare","Epic","Mythic"]

def seed(*parts):
    return int(hashlib.sha256("::".join(map(str,parts)).encode()).hexdigest()[:16],16)

def make_world_sets(n=100):
    rng = random.Random(seed("world-sets"))
    sets = []
    for i in range(1, n+1):
        sid = f"world_set_{i:03}"
        name = f"Frontier Weave {i:03}"
        pieces = [{"slot": s, "name": f"{name} {s.title()}", "rarity": rng.choice(RARITIES)} for s in SLOTS]
        sets.append({"id": sid, "name": name, "pieces": pieces})
    return sets

def make_faction_sets():
    out = []
    for f in FACTIONS:
        rng = random.Random(seed("faction", f))
        for i in range(1, 11):
            sid = f"faction_{f.replace(' ','_').lower()}_{i:02}"
            name = f"{f} Regalia {i:02}"
            pieces = [{"slot": s, "name": f"{name} {s.title()}", "rarity": rng.choice(RARITIES)} for s in SLOTS]
            out.append({"id": sid, "name": name, "faction": f, "pieces": pieces, "rep_required": i*100})
    return out

def slug(s):
    return ''.join(ch.lower() if ch.isalnum() else '_' for ch in s).strip('_')

def make_enemy_sets(enemies_path):
    with open(enemies_path, 'r') as f:
        data = json.load(f)
    types = data.get('enemies', [])
    name_suffixes = ["Harness","Aegis","Panoply","Carapace","Raiment","Bulwark","Mail","Ward","Guard","Array"]
    out = []
    used_names = set()
    for e in types:
        etype = e.get('name') or e.get('id')
        fam = e.get('family','Unknown')
        rng = random.Random(seed("enemy-type", etype))
        base = f"{etype} {rng.choice(name_suffixes)}"
        name = base
        k = 2
        while name in used_names:
            name = f"{base} Mk-{k}"; k += 1
        used_names.add(name)
        sid = f"enemy_type_{slug(etype)}"
        pieces = [{"slot": s, "name": f"{name} {s.title()}", "rarity": rng.choice(RARITIES)} for s in SLOTS]
        out.append({"id": sid, "name": name, "enemy_family": fam, "pieces": pieces})
    return out

def make_perks(n=500):
    rng = random.Random(seed("perks"))
    stats = ["power","haste","crit","cdr","hp","dr","fp","dot","move","tenacity"]
    perks = []
    used_names = set()
    for i in range(1, n+1):
        stat = rng.choice(stats)
        mn = round(rng.uniform(1.0, 3.0),1)
        mx = round(rng.uniform(3.0, 10.0),1)
        base = f"{stat.upper()} Surge {i:03}"
        name = base
        k = 2
        while name in used_names:
            name = f"{base}-{k}"; k += 1
        used_names.add(name)
        rarity = rng.choices(RARITIES, weights=[50,30,12,6,2])[0]
        perks.append({"id": f"perk_{i:03}", "name": name, "stat": stat, "min": mn, "max": mx, "rarity": rarity})
    return perks

def main():
    root = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(root, exist_ok=True)
    world_sets = make_world_sets(100)
    faction_sets = make_faction_sets()
    enemies_path = os.path.join(os.path.dirname(__file__), '..', 'enemies', 'data', 'enemies.json')
    enemy_sets = make_enemy_sets(os.path.abspath(enemies_path))
    perks = make_perks(500)
    with open(os.path.join(root, 'world_sets.json'), 'w') as f: json.dump({"sets": world_sets}, f, indent=2)
    with open(os.path.join(root, 'faction_sets.json'), 'w') as f: json.dump({"sets": faction_sets}, f, indent=2)
    with open(os.path.join(root, 'enemy_sets.json'), 'w') as f: json.dump({"sets": enemy_sets}, f, indent=2)
    with open(os.path.join(root, 'perks.json'), 'w') as f: json.dump({"perks": perks}, f, indent=2)
    print("Wrote armor sets and perks")

if __name__ == '__main__':
    main()
