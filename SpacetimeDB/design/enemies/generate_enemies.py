#!/usr/bin/env python3
import json, random, os

random.seed(4242)

FAMILIES = [
  "Glass Wraith","Brass Gnarl","Null Serpent","Dune Strider","Moors Lurker","Arc Runner",
  "Rift Stag","Cinder Drake","Opaline Beetle","Shard Hound","Static Moth","Harmonic Jelly",
]
ELEMENTS = ["fire","ice","shock","void","earth","water"]
ROLES = ["bruiser","assassin","caster","tank","skirmisher"]
MOVES = ["bite","slash","tail_whip","pounce","dash","roar","acid_spit","quill_shot","charge","phase_blink","sand_burst","spark_chain","ice_shard","void_gaze"]
MOVEMENT = ["walker","runner","slider","flyer"]
ARMOR = ["light","medium","heavy"]
SIZES = ["Small","Large","Big","Colossal"]

ADJ = ["Neon","Abyssal","Iridescent","Obsidian","Gossamer","Static","Resonant","Null","Cinder","Crystal","Brass","Parallax"]
NOUN = ["Stalker","Howler","Crab","Scuttler","Worm","Golem","Rogue","Marauder","Whelp","Spawn","Warden","Sentry","Spinner","Wisp"]

def unique_name():
    return f"{random.choice(ADJ)} {random.choice(NOUN)}"

def pick_abilities():
    # Unpredictable feel: 3-6 random moves from pool, order matters
    k = random.randint(3,6)
    return random.sample(MOVES, k)

def traits():
    return round(random.uniform(0.2,0.95),2), round(random.uniform(0.2,0.95),2), round(random.uniform(0.2,0.95),2)

def make_enemy(base_id):
    fam = random.choice(FAMILIES)
    role = random.choice(ROLES)
    elem = random.choice(ELEMENTS)
    move = random.choice(MOVEMENT)
    arm = random.choice(ARMOR)
    # Base stats scale by stage and tier; include three stages per base
    out = []
    stages = [("Infant","Small", 1, 0.6), ("Adolescent","Large", 2, 0.85), ("Adult", random.choice(["Big","Colossal"]), 3, 1.1)]
    for idx,(stage,size,tier,scale) in enumerate(stages):
        name = f"{stage} {fam} {unique_name()}"
        hp = int(80*scale + random.randint(0,40))
        dmg = int(12*scale + random.randint(0,12))
        spd = round(1.5*scale + random.uniform(-0.2,0.6), 2)
        abs = pick_abilities()
        o,c,e = traits()
        tags = [f"stage:{stage}", f"O:{o}", f"C:{c}", f"E:{e}", "unpredictable"]
        desc = f"A {stage.lower()} of the {fam} family, attuned to {elem}. Personality O:{o} C:{c} E:{e}."
        out.append({
            "id": f"{fam.lower().replace(' ','_')}/{base_id}_{idx}",
            "name": name,
            "family": fam,
            "tier": tier,
            "role": role,
            "element": elem,
            "size": size,
            "movement": move,
            "armor": arm,
            "hp": hp,
            "damage": dmg,
            "speed": spd,
            "abilities": abs,
            "tags": tags,
            "description": desc,
        })
    return out

def main():
    # Generate ~1000 enemies total including stages → ~334 bases * 3 stages
    bases = 334
    enemies = []
    for i in range(bases):
        enemies.extend(make_enemy(i))
    env = {"count": len(enemies), "enemies": enemies}
    tgt_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(tgt_dir, exist_ok=True)
    tgt = os.path.join(tgt_dir, 'enemies.json')
    with open(tgt, 'w') as f:
        json.dump(env, f)
    print(f"Wrote {env['count']} enemies to {tgt}")

if __name__ == '__main__':
    main()
