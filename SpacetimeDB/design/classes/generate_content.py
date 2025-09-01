#!/usr/bin/env python3
"""
Generate class abilities (50) and perks (100) from deterministic templates.

Usage:
  python generate_content.py --class "Time Mage" --out data/TimeMage

This script emits two files in the output dir:
  abilities.json, perks.json

Note: This is a content scaffold; tune numbers by playtests.
"""
import argparse, json, os, random, hashlib

TIER_NAMES = [1,2,3,4,5,6]
PERK_TIERS = ["Common","Rare","Epic","Legendary"]

def seeded_rng(*parts):
    h = hashlib.sha256("::".join(map(str,parts)).encode()).hexdigest()
    seed = int(h[:16], 16)
    rng = random.Random(seed)
    return rng

def gen_abilities(cls, branches, resource_type, seed):
    rng = seeded_rng(cls, seed, "abilities")
    abilities = []
    # 30 actives (10 / branch), 12 passives (4 / branch), 6 exotics (2 / branch), 2 ultimates
    for b in branches:
        for i in range(10):
            tier = 1 + i // 2
            name = f"{b} Act {i+1}"
            abilities.append({
                "id": f"{cls.lower().replace(' ','_')}/{b.lower()}_act_{i+1}",
                "name": name,
                "branch": b,
                "tier": min(tier, 5),
                "type": "active",
                "resource": {"type": resource_type, "cost": round(rng.uniform(10,35),1)},
                "cooldown_s": round(rng.uniform(6,20),1),
                "tags": ["damage" if i%3==0 else "utility"],
                "scaling": {"power": round(rng.uniform(0.8,1.3),2)},
                "effects": [{"kind":"damage","mult": round(rng.uniform(1.0,2.2),2)}],
                "description": f"Active ability in {b} branch."
            })
        for i in range(4):
            tier = 1 + i // 2
            abilities.append({
                "id": f"{cls.lower().replace(' ','_')}/{b.lower()}_pas_{i+1}",
                "name": f"{b} Passive {i+1}",
                "branch": b,
                "tier": min(tier,5),
                "type": "passive",
                "tags": ["passive"],
                "effects": [{"kind":"stat","stat":"haste","pct": round(rng.uniform(1,4),1)}],
                "description": f"Passive bonus themed to {b}."
            })
        for i in range(2):
            abilities.append({
                "id": f"{cls.lower().replace(' ','_')}/{b.lower()}_exo_{i+1}",
                "name": f"{b} Exotic {i+1}",
                "branch": b,
                "tier": 6,
                "type": "exotic",
                "tags": ["exotic"],
                "effects": [{"kind":"special","note":"cross-branch synergy"}],
                "description": f"Exotic effect connecting {b} to other branches."
            })
    for i in range(2):
        abilities.append({
            "id": f"{cls.lower().replace(' ','_')}/ultimate_{i+1}",
            "name": f"{cls} Ultimate {i+1}",
            "branch": "All",
            "tier": 6,
            "type": "ultimate",
            "cooldown_s": 90 + i*30,
            "resource": {"type": resource_type, "cost": 0},
            "effects": [{"kind":"ultimate","note":"signature"}],
            "description": f"Thematic ultimate for {cls}."
        })
    return abilities

def gen_perks(cls, seed):
    rng = seeded_rng(cls, seed, "perks")
    tags_pool = ["damage","defense","utility","resource","mobility","control"]
    perks = []
    for i in range(100):
        tier = rng.choices(PERK_TIERS, weights=[50,30,15,5])[0]
        tags = rng.sample(tags_pool, k=rng.randint(1,2))
        roll = {"stat": rng.choice(["power","haste","crit","cdr","hp","dr"]),
                "min": round(rng.uniform(1,3),1), "max": round(rng.uniform(3,8),1)}
        perks.append({
            "id": f"{cls.lower().replace(' ','_')}/perk_{i+1}",
            "name": f"{cls} Perk {i+1}",
            "tier": tier,
            "tags": tags,
            "effect": {"note":"class-tailored bonus"},
            "rolls": [roll],
            "description": f"Random perk enhancing {', '.join(tags)} for {cls}."
        })
    return perks

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--class', dest='cls', required=True)
    ap.add_argument('--out', required=True)
    ap.add_argument('--seed', type=int, default=1337)
    ap.add_argument('--branches', nargs=3, required=True)
    ap.add_argument('--resource', default='Mana')
    args = ap.parse_args()

    os.makedirs(args.out, exist_ok=True)
    abilities = gen_abilities(args.cls, args.branches, args.resource, args.seed)
    perks = gen_perks(args.cls, args.seed)
    with open(os.path.join(args.out, 'abilities.json'), 'w') as f:
        json.dump({"class": args.cls, "abilities": abilities, "perks": []}, f, indent=2)
    with open(os.path.join(args.out, 'perks.json'), 'w') as f:
        json.dump({"class": args.cls, "abilities": [], "perks": perks}, f, indent=2)
    print(f"Wrote {len(abilities)} abilities and {len(perks)} perks to {args.out}")

if __name__ == '__main__':
    main()

