#!/usr/bin/env python3
import json, random, os

random.seed(77)

FACTIONS = ["Parallax Concord","Free Archipelagos","Brass Arc Cartel","Shard Tribunal"]
ROLES = ["Archivist","Mechanic","Navigator","Scout","Smuggler","Cartographer","Judge","Mediator","Ritualist","Warden","Courier","Drifter","Prospector","Singer","Alchemist"]
PERSONA = ["Stoic","Wry","Melancholic","Optimistic","Pragmatic","Reckless","Methodical","Gentle","Abrasive","Curious","Secretive","Devout","Playful","Stern"]
ADJ = ["Neon","Iridescent","Holographic","Cinder","Null","Abyssal","Crystal","Brass","Shard","Ultraviolet","Resonant","Harmonic","Chisel","Fog","Gossamer","Static","Parallax"]
NOUN = ["Runner","Archivist","Smith","Navigator","Scribe","Broker","Skipper","Whisper","Pilot","Seeker","Warden","Glass","Dune","Delta","Arc","Rift"]

BIO_TEMPLATES = [
    "Raised among the {fac} adjuncts, {name} memorized dispute ledgers and now recites them like prayer.",
    "A {role} who charted the {place} by scent alone, {name} bargains in favors instead of credits.",
    "Once left for dead in the {place}, {name} survived on salvaged coolant and myth.",
    "{name} believes the world is a machine with missing screws—and carries a pocket full of replacements.",
    "When the {fac} seized a relay, {name} smuggled families through its signal shadow.",
    "{name} sings to quartz antennas; the echoes answer with coordinates.",
    "The Tribunal tested {name} three times. The fourth time, {name} tested them back.",
]

PLACES = ["Hologram Dunes","Neon Metropolis","Glass Moors","Archipelago Tiltways","Null Estuary","Basilisk Warrens"]

def rand_name():
    return f"{random.choice(ADJ)} {random.choice(NOUN)}"

def mk_bio(name, role, fac):
    base = random.choice(BIO_TEMPLATES).format(name=name, role=role, fac=fac, place=random.choice(PLACES))
    quirk = random.choice([
        "hums numbers to stay calm",
        "refuses to step on blue tiles",
        "trades only in favors on odd days",
        "believes storms carry messages",
        "keeps maps folded to avoid jinxing routes",
        "collects broken instruments for comfort",
    ])
    return f"{base} They {quirk}."

def schedule_for(home, role, traits):
    # Traits: dict with openness, conscientiousness, extraversion
    o = traits['o']; c = traits['c']; e = traits['e']
    # Work start varies by role
    work_start = random.randint(7, 11)
    if role in ("Warden","Courier"): work_start = random.choice([6,7,8])
    if role in ("Ritualist","Navigator"): work_start = random.choice([9,10,11])
    # Sleep start influenced by conscientiousness (higher c -> earlier sleep)
    sleep_start = random.choice([21,22,23,0,1])
    if c > 0.7: sleep_start = random.choice([21,22])
    if e > 0.7: sleep_start = random.choice([23,0,1])
    # Social window influenced by extraversion
    social_hour = random.choice([17,18,19,20,21,22]) if e >= 0.5 else random.choice([17,18,19])
    # Midday break
    rest_hour = random.choice([11,12,13,14])
    # Commute / explore hour influenced by openness
    commute_hour = max(work_start+7, random.choice([15,16,17]))
    hobby_hour = random.choice([6,7,16]) if o >= 0.6 else random.choice([6,16])
    # Some activities occur in neighboring regions to feel alive
    def near_region():
        d = random.choice([-1,0,1])
        rr = (home + d) % 10
        return rr
    return [
        {"hour": work_start, "activity": "Work", "region_id": home, "x": random.uniform(5,25), "y": 0.0, "z": random.uniform(5,25)},
        {"hour": rest_hour, "activity": "Rest", "region_id": home, "x": random.uniform(10,30), "y": 0.0, "z": random.uniform(10,30)},
        {"hour": commute_hour, "activity": "Commute", "region_id": near_region(), "x": random.uniform(0,10), "y": 0.0, "z": random.uniform(0,10)},
        {"hour": social_hour, "activity": "Social", "region_id": near_region(), "x": random.uniform(15,35), "y": 0.0, "z": random.uniform(15,35)},
        {"hour": hobby_hour, "activity": "Hobby", "region_id": home, "x": random.uniform(-8,12), "y": 0.0, "z": random.uniform(-8,12)},
        {"hour": sleep_start, "activity": "Sleep", "region_id": home, "x": random.uniform(-5,5), "y": 0.0, "z": random.uniform(-5,5)}
    ]

def main():
    total = 500
    out = []
    for i in range(total):
        fac = random.choice(FACTIONS)
        role = random.choice(ROLES)
        per = random.choice(PERSONA)
        home = random.randint(0, 9)
        name = rand_name()
        traits = { 'o': round(random.uniform(0.2, 0.95), 2), 'c': round(random.uniform(0.2, 0.95), 2), 'e': round(random.uniform(0.2, 0.95), 2) }
        personality = f"{per}; O:{traits['o']:.2f} C:{traits['c']:.2f} E:{traits['e']:.2f}"
        out.append({
            "name": name,
            "faction": fac,
            "role": role,
            "personality": personality,
            "home_region": home,
            "bio": mk_bio(name, role, fac),
            "schedule": schedule_for(home, role, traits)
        })
    env = {"npcs": out}
    tgt_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(tgt_dir, exist_ok=True)
    tgt = os.path.join(tgt_dir, 'npcs.json')
    with open(tgt, 'w') as f:
        json.dump(env, f)
    print(f"Wrote {total} NPCs to {tgt}")

if __name__ == '__main__':
    main()
