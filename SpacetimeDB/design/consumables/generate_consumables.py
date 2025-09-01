#!/usr/bin/env python3
import json, random, os

random.seed(42)

TIERS = [1,2,3,4,5]
CATEGORIES = [
    ("potion", (120, 240), "RefreshDuration"),
    ("tonic", (300, 600), "Unique"),
    ("elixir", (90, 180), "RefreshDuration"),
    ("meal", (600, 1200), "Unique"),
    ("brew", (240, 420), "StackDiminishing"),
    ("tincture", (240, 480), "RefreshDuration"),
    ("scroll", (30, 90), "Unique"),
]

ADJ = ["Amber", "Azure", "Crimson", "Umbral", "Iridescent", "Gilded", "Verdant", "Cobalt", "Opaline", "Abyssal", "Solar", "Lunar", "Quicksilver", "Vermilion", "Obsidian"]
NOUN = ["Vigor", "Aegis", "Alacrity", "Fury", "Fortune", "Insight", "Focus", "Resolve", "Grace", "Storm", "Embers", "Frost", "Surge", "Shadows", "Echoes"]
OF = ["of Sparks", "of the Dunes", "of the Deep", "of Parallax", "of the Archipelago", "of Glass", "of Brass", "of Null", "of the Rift", "of the Moors"]

EFFECTS = [
    ("DamageMult", 0.03, 0.35),
    ("DefenseMult", 0.03, 0.35),
    ("MoveSpeedMult", 0.03, 0.20),
    ("CritChanceAdd", 0.02, 0.15),
    ("CritDamageMult", 0.05, 0.30),
    ("CooldownReductionMult", 0.04, 0.25),
    ("StaminaRegenMult", 0.05, 0.35),
    ("HealthRegenFlat", 0.5, 4.5),
    ("LifeStealPct", 0.02, 0.08),
    ("ResistFire", 0.05, 0.25),
    ("ResistIce", 0.05, 0.25),
    ("ResistShock", 0.05, 0.25),
    ("ResistVoid", 0.05, 0.25),
    ("DropRateMult", 0.05, 0.35),
    ("XPGainMult", 0.05, 0.35),
    ("GoldFindMult", 0.05, 0.35),
    ("ShieldFlat", 15.0, 160.0),
]

def tier_scale(tier, base, peak):
    # Map tier 1..5 into 0..1 curve with light acceleration
    t = (tier-1) / 4.0
    s = 0.5 * t + 0.5 * (t*t)  # gentle acceleration
    return base + (peak-base) * s

def pick_effects(category, tier):
    # Category palettes
    palette = {
        "potion": ["DamageMult","DefenseMult","MoveSpeedMult","StaminaRegenMult","ResistFire","ResistIce","ResistShock","ResistVoid","CritChanceAdd"],
        "tonic": ["DamageMult","DefenseMult","CooldownReductionMult","StaminaRegenMult","XPGainMult","GoldFindMult"],
        "elixir": ["DamageMult","CritChanceAdd","CritDamageMult","LifeStealPct","ShieldFlat"],
        "meal": ["HealthRegenFlat","ResistFire","ResistIce","ResistShock","ResistVoid","StaminaRegenMult"],
        "brew": ["DamageMult","DefenseMult","DropRateMult","XPGainMult","GoldFindMult"],
        "tincture": ["MoveSpeedMult","CooldownReductionMult","CritChanceAdd","CritDamageMult"],
        "scroll": ["DamageMult","CritDamageMult","ShieldFlat"],
    }[category]
    k = random.randint(1, 3)
    kinds = random.sample(palette, k)
    eff = []
    for kind in kinds:
        base, peak = next((b,p) for (k2,b,p) in EFFECTS if k2==kind)
        amt = round(random.uniform(0.9, 1.1) * tier_scale(tier, base, peak), 3)
        eff.append({"kind": kind, "amount": amt})
    return eff

def main():
    total = 500
    out = []
    counter = 0
    while len(out) < total:
        cat, (dmin,dmax), stack = random.choice(CATEGORIES)
        tier = random.choice(TIERS)
        duration = random.randint(dmin, dmax)
        cooldown = random.randint(30, 90)
        name = f"T{tier} {random.choice(ADJ)} {random.choice(NOUN)} {random.choice(OF)}"
        cid = f"{cat}/{name.lower().replace(' ','_').replace('/','_')}"
        effects = pick_effects(cat, tier)
        for e in effects:
            e["duration_s"] = duration
        out.append({
            "id": cid,
            "name": name,
            "tier": tier,
            "category": cat,
            "cooldown_s": cooldown,
            "stack": stack,
            "effects": effects,
        })
        counter += 1

    env = {"consumables": out}
    tgt_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(tgt_dir, exist_ok=True)
    tgt = os.path.join(tgt_dir, 'consumables.json')
    with open(tgt, 'w') as f:
        json.dump(env, f)
    print(f"Wrote {len(out)} consumables to {tgt}")

if __name__ == '__main__':
    main()

