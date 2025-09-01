#!/usr/bin/env python3
import json, random, os

random.seed(1337)

CATS = ["Hunt","Delivery","Explore","Puzzle","Social","WorldEvent","Boss","Crafting","Rescue","Investigate"]
ENEMY_FAMILIES = ["Glass Wraith","Brass Gnarl","Null Serpent","Dune Strider","Moors Lurker","Arc Runner"]
BIOMES = ["Hologram Dunes","Neon Metropolis","Glass Moors","Archipelago Tiltways","Null Estuary","Basilisk Warrens"]
NPC_ROLES = ["Archivist","Mechanic","Navigator","Ritualist","Warden","Courier","Mediator","Prospector"]

REWARD_ITEMS = ["Repair Kit","Arc Shard","Stimulant","Encrypted Map","Signal Beacon","Lumens"]

def maybe_reward():
    if random.random() < 0.35:
        return random.choice(REWARD_ITEMS), random.randint(1,3)
    return None, None

def hunt(i):
    family = random.choice(ENEMY_FAMILIES)
    count = random.randint(3, 12)
    name = f"Cull the {family}s ({count})"
    desc = f"Thin the numbers of {family}s threatening caravans."
    item, qty = maybe_reward()
    return dict(id=f"hunt/{i}", name=name, description=desc, category="Hunt", event_type="kill_enemy", event_param=family, required_count=count, time_limit_s=random.choice([None, 900, 1800]), chain_next_id=None, reward_credits=random.randint(60,180), reward_item=item, reward_item_qty=qty)

def delivery(i):
    role = random.choice(NPC_ROLES)
    name = f"Deliver Components to the {role}"
    desc = f"Carry a sealed bundle to a {role} without opening it."
    item, qty = maybe_reward()
    return dict(id=f"delivery/{i}", name=name, description=desc, category="Delivery", event_type="deliver_item", event_param=role, required_count=1, time_limit_s=random.choice([600,1200]), chain_next_id=None, reward_credits=random.randint(50,140), reward_item=item, reward_item_qty=qty)

def explore(i):
    biome = random.choice(BIOMES)
    name = f"Chart Unknown in {biome}"
    desc = f"Reach survey beacons and record resonances in {biome}."
    item, qty = maybe_reward()
    return dict(id=f"explore/{i}", name=name, description=desc, category="Explore", event_type="enter_biome", event_param=biome, required_count=random.randint(1,3), time_limit_s=None, chain_next_id=None, reward_credits=random.randint(70,160), reward_item=item, reward_item_qty=qty)

def wevent(i):
    biome = random.choice(BIOMES)
    name = f"Stabilize a World Event in {biome}"
    desc = f"Participate in resolving an emergent phenomenon in {biome}."
    item, qty = maybe_reward()
    return dict(id=f"wevent/{i}", name=name, description=desc, category="WorldEvent", event_type="active_event", event_param=biome, required_count=1, time_limit_s=1800, chain_next_id=None, reward_credits=random.randint(90,220), reward_item=item, reward_item_qty=qty)

def boss(i):
    name = f"Challenge a World Boss"
    desc = "Engage and defeat a roaming world boss with allies."
    item, qty = maybe_reward()
    return dict(id=f"boss/{i}", name=name, description=desc, category="Boss", event_type="kill_boss", event_param="", required_count=1, time_limit_s=3600, chain_next_id=None, reward_credits=random.randint(150,300), reward_item=item, reward_item_qty=qty)

def social(i):
    role = random.choice(NPC_ROLES)
    name = f"Broker a Truce with a {role}"
    desc = f"Persuade a {role} to stand down through dialogue choices."
    item, qty = maybe_reward()
    return dict(id=f"social/{i}", name=name, description=desc, category="Social", event_type="dialogue_node", event_param=role, required_count=1, time_limit_s=None, chain_next_id=None, reward_credits=random.randint(80,170), reward_item=item, reward_item_qty=qty)

def investigate(i):
    biome = random.choice(BIOMES)
    what = random.choice(["anomaly", "distortion", "lost beacon", "ghost signal", "geomantic knot"])
    name = f"Investigate {what} in {biome}"
    desc = f"Scan and triangulate a {what} reported in {biome}."
    item, qty = maybe_reward()
    return dict(id=f"investigate/{i}", name=name, description=desc, category="Investigate", event_type="scan_signal", event_param=biome, required_count=random.randint(1,2), time_limit_s=random.choice([600,900,None]), chain_next_id=None, reward_credits=random.randint(70,150), reward_item=item, reward_item_qty=qty)

def rescue(i):
    role = random.choice(NPC_ROLES)
    name = f"Rescue a {role}"
    desc = f"Escort a stranded {role} back to safety."
    item, qty = maybe_reward()
    return dict(id=f"rescue/{i}", name=name, description=desc, category="Rescue", event_type="rescue_npc", event_param=role, required_count=1, time_limit_s=random.choice([900, 1200, None]), chain_next_id=None, reward_credits=random.randint(100,220), reward_item=item, reward_item_qty=qty)

def crafting(i):
    item_needed = random.choice(["Signal Beacon","Repair Kit","Encrypted Map"])
    name = f"Craft {item_needed}"
    desc = f"Collect parts and assemble a {item_needed}."
    item, qty = maybe_reward()
    return dict(id=f"craft/{i}", name=name, description=desc, category="Crafting", event_type="craft_item", event_param=item_needed, required_count=1, time_limit_s=None, chain_next_id=None, reward_credits=random.randint(90,180), reward_item=item, reward_item_qty=qty)

def puzzle(i):
    name = f"Solve the Relay Puzzle"
    desc = "Align rune-dials and pipe-valves to re-route power."
    item, qty = maybe_reward()
    return dict(id=f"puzzle/{i}", name=name, description=desc, category="Puzzle", event_type="solve_puzzle", event_param="relay", required_count=1, time_limit_s=900, chain_next_id=None, reward_credits=random.randint(80,160), reward_item=item, reward_item_qty=qty)

MAKERS = [hunt, delivery, explore, wevent, boss, social, investigate, rescue, crafting, puzzle]

def main():
    total = 1000
    out = []
    for i in range(total):
        m = random.choice(MAKERS)
        q = m(i)
        out.append(q)
    # Add structured chains (Investigate -> Hunt -> Deliver) and (Rescue -> Social -> Deliver)
    for c in range(20):
        a = investigate(10000+c)
        b = hunt(11000+c)
        d = delivery(12000+c)
        a["chain_next_id"] = b["id"]; b["chain_next_id"] = d["id"]
        out.extend([a,b,d])
    for c in range(20):
        a = rescue(13000+c)
        b = social(14000+c)
        d = delivery(15000+c)
        a["chain_next_id"] = b["id"]; b["chain_next_id"] = d["id"]
        out.extend([a,b,d])
    env = {"quests": out}
    tgt_dir = os.path.join(os.path.dirname(__file__), 'data')
    os.makedirs(tgt_dir, exist_ok=True)
    tgt = os.path.join(tgt_dir, 'quests.json')
    with open(tgt, 'w') as f:
        json.dump(env, f)
    print(f"Wrote {len(out)} quests to {tgt}")

if __name__ == '__main__':
    main()
