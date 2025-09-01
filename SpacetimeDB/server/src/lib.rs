use spacetimedb::{table, reducer, Table, ReducerContext, Identity, Timestamp};
use spacetimedb::rand::{rngs::StdRng, Rng, SeedableRng};
use serde::{Deserialize, Serialize};
use serde_json::json;

// Players table: one row per unique Identity
#[table(name = player, public)]
pub struct Player {
    #[primary_key]
    pub identity: Identity,
    pub name: Option<String>,
    pub created_at: Timestamp,
}

// World state: keep a single row (id = 1) for global state
#[table(name = world, public)]
pub struct WorldState {
    #[primary_key]
    pub id: u32,
    pub tick: u64,
}

// World configuration: single row with id = 1
#[table(name = world_config, public)]
pub struct WorldConfig {
    #[primary_key]
    pub id: u32,
    pub seed: u64,
    pub size_meters: u32,
    pub chunk_size_m: u32,
    pub created_at: Timestamp,
}

// Realtime player state (lightweight positional sync)
#[table(name = player_state, public)]
pub struct PlayerState {
    #[primary_key]
    pub identity: Identity,
    #[index(btree)]
    pub region_id: u8,
    pub x: f32,
    pub y: f32,
    pub z: f32,
    pub yaw_deg: f32,
    pub updated_at: Timestamp,
}

// Content: Abilities (imported from JSON)
#[table(name = ability, public)]
pub struct Ability {
    #[primary_key]
    pub id: String,
    #[index(btree)]
    pub class_name: String,
    pub name: String,
    pub branch: String,
    pub tier: u8,
    pub kind: String,
    pub resource_type: Option<String>,
    pub resource_cost: Option<f32>,
    pub cooldown_s: Option<f32>,
    pub description: String,
}

// Content: Perks (imported from JSON)
#[table(name = perk, public)]
pub struct Perk {
    #[primary_key]
    pub id: String,
    #[index(btree)]
    pub class_name: String,
    pub name: String,
    pub tier: String,
    pub tags_csv: String,
    pub effect_desc: String,
    pub roll_stat: Option<String>,
    pub roll_min: Option<f32>,
    pub roll_max: Option<f32>,
    pub description: String,
}

// Perk catalog for fast roll selection
#[table(name = perk_catalog, public)]
pub struct PerkCatalog {
    #[primary_key]
    pub class_name: String,
    pub ids_csv: String,
    pub updated_at: Timestamp,
}

#[table(name = class_catalog, public)]
pub struct ClassCatalog {
    #[primary_key]
    pub class_name: String,
    pub updated_at: Timestamp,
}

// Generated loot
#[table(name = weapon_instance, public)]
pub struct WeaponInstance {
    #[primary_key]
    pub id: u64,
    pub owner: Identity,
    pub class_name: String,
    pub weapon_type: String,
    pub rarity: String,
    pub power: u32,
    pub level: u32,
    pub created_at: Timestamp,
}

#[table(name = weapon_perk, public)]
pub struct WeaponPerk {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub weapon_id: u64,
    pub perk_id: String,
    pub perk_name: String,
    pub rolled_stat: Option<String>,
    pub rolled_value: Option<f32>,
}

#[table(name = rarity_config, public)]
pub struct RarityConfig {
    #[primary_key]
    pub name: String,
    pub weight: u32,
    pub order: u8,
}

// World regions (overworld entries for 10x 5km biomes)
#[table(name = world_region, public)]
pub struct WorldRegion {
    #[primary_key]
    pub id: u8,
    pub name: String,
    pub seed: u64,
    pub size_meters: u32,
    pub chunk_size_m: u32,
    pub biome_json: String,
    pub created_at: Timestamp,
    pub updated_at: Timestamp,
}

// Boss catalog and armor sets
#[table(name = boss_type, public)]
pub struct BossType {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub biome: String,
    pub tier: u8,
    pub mechanics_csv: String,
    pub puzzle_modules_csv: String,
    pub phases_csv: String,
    pub armor_set_id: String,
    pub description: String,
}

#[table(name = armor_set, public)]
pub struct ArmorSet {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub set_bonuses_csv: String,
}

#[table(name = armor_piece, public)]
pub struct ArmorPiece {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub set_id: String,
    pub slot: String,
    pub name: String,
    pub bonus: String,
}

// World Bosses
#[table(name = world_boss_type, public)]
pub struct WorldBossType {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub biome: String,
    pub tier: u8,
    pub env_json: String,
    pub min_ng_plus: u8,
    pub description: String,
}

#[table(name = world_boss_spawn, public)]
pub struct WorldBossSpawn {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub boss_id: String,
    pub region_id: u8,
    pub x: f32,
    pub z: f32,
    pub alive: bool,
    pub spawned_at: Timestamp,
}

#[derive(Deserialize)]
struct WorldBossIn {
    id: String,
    name: String,
    biome: String,
    tier: u8,
    env_effect: serde_json::Value,
    min_ng_plus: u8,
    description: String,
}
#[derive(Deserialize)]
struct WorldBossEnvelope { world_bosses: Vec<WorldBossIn> }

#[derive(Deserialize)]
struct BiomeIn {
    id: u8,
    name: String,
    seed: u64,
    size_meters: u32,
    chunk_size_m: u32,
    palette: serde_json::Value,
    lighting: Option<serde_json::Value>,
    vfx: Option<serde_json::Value>,
    props: Option<serde_json::Value>,
    spawn_weights: serde_json::Value,
    description: String,
}

#[derive(Deserialize)]
struct BiomesEnvelope { biomes: Vec<BiomeIn> }

#[derive(Deserialize)]
struct BossArmorPiece { slot: String, name: String, bonus: String }
#[derive(Deserialize)]
struct BossArmorSet { id: String, name: String, pieces: Vec<BossArmorPiece>, set_bonuses: Vec<String> }
#[derive(Deserialize)]
struct BossIn {
    id: String,
    name: String,
    biome: String,
    tier: u8,
    mechanics: Vec<String>,
    puzzle_modules: Vec<String>,
    phases: Vec<String>,
    armor_set: BossArmorSet,
    description: String,
}
#[derive(Deserialize)]
struct BossesEnvelope { bosses: Vec<BossIn> }

// Enemy catalog
#[table(name = enemy_type, public)]
pub struct EnemyType {
    #[primary_key]
    pub id: String,
    pub name: String,
    #[index(btree)]
    pub family: String,
    pub tier: u8,
    pub role: String,
    pub element: String,
    pub size: String,
    pub movement: String,
    pub armor: String,
    pub hp: u32,
    pub damage: u32,
    pub speed: f32,
    pub abilities_csv: String,
    pub tags_csv: String,
    pub description: String,
}

#[derive(Deserialize)]
struct EnemyIn {
    id: String,
    name: String,
    family: String,
    tier: u8,
    role: String,
    element: String,
    size: String,
    movement: String,
    armor: String,
    hp: u32,
    damage: u32,
    speed: f32,
    abilities: Vec<String>,
    tags: Vec<String>,
    description: String,
}

// Player avatar customization (public for client reads; consider RLS later)
#[table(name = avatar, public)]
pub struct Avatar {
    #[primary_key]
    pub identity: Identity,
    pub height_cm: u16,
    pub weight_kg: u16,
    pub body_json: String,
    pub face_json: String,
    pub created_at: Timestamp,
    pub updated_at: Timestamp,
}

#[derive(Deserialize)]
struct AvatarIn {
    height_cm: u16,
    weight_kg: u16,
    #[serde(default)] body: serde_json::Value,
    #[serde(default)] face: serde_json::Value,
}

fn validate_avatar(a: &AvatarIn) -> Result<(), String> {
    // Basic anthropometric constraints (adult human ranges)
    if !(145..=210).contains(&a.height_cm) { return Err("height_cm must be 145..210".into()); }
    if !(35..=180).contains(&a.weight_kg) { return Err("weight_kg must be 35..180".into()); }
    let h_m = a.height_cm as f32 / 100.0;
    let bmi = a.weight_kg as f32 / (h_m * h_m);
    if !(15.0..=45.0).contains(&bmi) { return Err("BMI out of allowed range (15..45)".into()); }
    // Optional: clamp a few known body ratios if present
    if let Some(obj) = a.body.as_object() {
        if let Some(v) = obj.get("arm_length_ratio").and_then(|x| x.as_f64()) {
            if v < 0.85 || v > 1.15 { return Err("arm_length_ratio must be 0.85..1.15".into()); }
        }
        if let Some(v) = obj.get("leg_length_ratio").and_then(|x| x.as_f64()) {
            if v < 0.85 || v > 1.15 { return Err("leg_length_ratio must be 0.85..1.15".into()); }
        }
        if let Some(v) = obj.get("torso_scale").and_then(|x| x.as_f64()) {
            if v < 0.9 || v > 1.1 { return Err("torso_scale must be 0.9..1.1".into()); }
        }
    }
    // Face blendshape weights if provided: 0..100
    if let Some(obj) = a.face.as_object() {
        for (_k, val) in obj.iter() {
            if let Some(w) = val.as_f64() {
                if w < 0.0 || w > 100.0 { return Err("face blendshape weights must be 0..100".into()); }
            }
        }
    }
    Ok(())
}

// =====================
// NLP: Natural replies
// =====================
#[derive(Deserialize, Serialize)]
struct NlpInput {
    speaker_name: Option<String>,
    player_name: Option<String>,
    topic: Option<String>,
    style: Option<String>, // e.g., quest_intro, quest_progress, quest_complete, generic
    tone: Option<String>,  // e.g., friendly, stoic, gruff, scholarly
    o: Option<f32>,        // openness 0..1
    c: Option<f32>,        // conscientiousness 0..1
    e: Option<f32>,        // extraversion 0..1
    last_player_line: Option<String>,
    locale: Option<String>,
    webhook_url: Option<String>,
}

fn clamp01(x: f32) -> f32 { if x < 0.0 { 0.0 } else if x > 1.0 { 1.0 } else { x } }

fn pick<'a>(rng: &mut StdRng, list: &'a [&'a str]) -> &'a str {
    if list.is_empty() { return ""; }
    let i = (rng.gen::<usize>()) % list.len(); list[i]
}

fn build_reply(mut rng: StdRng, req: &NlpInput) -> String {
    let speaker = req.speaker_name.as_deref().unwrap_or("I");
    let player = req.player_name.as_deref().unwrap_or("you");
    let topic = req.topic.as_deref().unwrap_or("");
    let style = req.style.as_deref().unwrap_or("generic");
    let tone = req.tone.as_deref().unwrap_or("neutral");
    let o = clamp01(req.o.unwrap_or(0.5));
    let c = clamp01(req.c.unwrap_or(0.5));
    let e = clamp01(req.e.unwrap_or(0.5));
    let last = req.last_player_line.as_deref().unwrap_or("");

    // Tone modifiers
    let (adverb, honorific) = match tone {
        "friendly" => (pick(&mut rng, &["kindly", "gladly", "happily"]).to_string(), pick(&mut rng, &["friend", "traveler", "ally"]).to_string()),
        "gruff" => (pick(&mut rng, &["plainly", "frankly", "briefly"]).to_string(), pick(&mut rng, &["stranger", "hunter", "scout"]).to_string()),
        "scholarly" => (pick(&mut rng, &["notably", "curiously", "precisely"]).to_string(), pick(&mut rng, &["adept", "apprentice", "seeker"]).to_string()),
        _ => (pick(&mut rng, &["", "", ""]).to_string(), pick(&mut rng, &["traveler", "friend", ""]).to_string()),
    };

    let greet = if e > 0.65 {
        pick(&mut rng, &["Hey", "Greetings", "Well met", "Hello"]).to_string()
    } else if e < 0.35 { pick(&mut rng, &["Hm", "Ah", "...", "Yes?"]).to_string() } else { pick(&mut rng, &["Hello", "Greetings"]).to_string() };

    let ask = if o > 0.65 { pick(&mut rng, &["care to explore", "shall we consider", "want to try"]) } else { pick(&mut rng, &["can you", "would you", "could you"]) };
    let thanks = if c > 0.6 { pick(&mut rng, &["Much appreciated", "You have my thanks", "Duly noted"]) } else { pick(&mut rng, &["Thanks", "Cheers", "Good work"]) };

    let mut line = String::new();
    match style {
        "quest_intro" => {
            let need = if topic.is_empty() { "a task" } else { topic };
            line = format!("{greet}, {honorific}. {adverb} speaking, {speaker} has {ask} something: {need}.");
        }
        "quest_progress" => {
            let probe = if last.is_empty() { pick(&mut rng, &["How goes it?", "Any progress?", "Status?"]) } else { pick(&mut rng, &["I hear you say: ", "Noted: ", "Understood: "]) };
            if last.is_empty() { line = format!("{probe}"); } else { line = format!("{probe}{last}"); }
        }
        "quest_complete" => {
            let result = if topic.is_empty() { "the job" } else { topic };
            line = format!("{thanks}. That's {result} done. {adverb} put, {speaker} can rest easier now.");
        }
        _ => {
            // generic smalltalk
            let filler = pick(&mut rng, &["The wind shifts.", "These lands change by the hour.", "Eyes open; paths are fickle.", "Keep your wits about you."]);
            if !topic.is_empty() { line = format!("About {topic}, {filler}"); }
            else { line = filler.to_string(); }
        }
    }
    // Small personalization
    if !player.is_empty() && !line.contains(&player) && rng.gen::<f32>() > 0.5 { line.push_str(&format!(" {player}.")); }
    line
}

#[reducer]
pub fn nlp_generate_reply(ctx: &ReducerContext, input_json: String) -> Result<String, String> {
    // Deterministic RNG from timestamp + sender to keep replies stable across retries
    let seed = ctx.timestamp.to_micros_since_unix_epoch() as u64 ^ (ctx.sender.as_u64());
    let rng = StdRng::seed_from_u64(seed);
    let req: NlpInput = serde_json::from_str(&input_json).map_err(|e| e.to_string())?;
    // Try webhook if enabled, else fallback to local template
    if let Some(t) = try_webhook(&req) {
        return Ok(json!({"text": t}).to_string());
    }
    let text = build_reply(rng, &req);
    // Return as a JSON envelope for future extensibility
    Ok(json!({"text": text}).to_string())
}

// ======================================
// Dynamic Choices: unique per quest/player
// ======================================
#[derive(Deserialize)]
struct ChoiceInput {
    quest_id: Option<String>,
    topic: Option<String>,
    style: Option<String>,   // quest_intro | quest_progress | quest_complete | generic
    tone: Option<String>,
    o: Option<f32>, c: Option<f32>, e: Option<f32>,
    count: Option<u8>,       // number of choices to return (default 4)
}

#[derive(serde::Serialize)]
struct ChoiceOut<'a> { id: &'a str, text: String, tag: &'a str }

trait Shuffle<T> { fn shuffle(&mut self, rng: &mut StdRng); }
impl<T> Shuffle<T> for Vec<T> {
    fn shuffle(&mut self, rng: &mut StdRng) {
        let mut i = self.len();
        while i > 1 {
            let j = (rng.gen::<usize>()) % i;
            i -= 1;
            self.swap(i, j);
        }
    }
}

fn build_choices(mut rng: StdRng, inp: &ChoiceInput) -> Vec<ChoiceOut<'static>> {
    let topic = inp.topic.as_deref().unwrap_or("");
    let style = inp.style.as_deref().unwrap_or("quest_intro");
    let tone = inp.tone.as_deref().unwrap_or("neutral");
    let o = clamp01(inp.o.unwrap_or(0.5));
    let c = clamp01(inp.c.unwrap_or(0.5));
    let e = clamp01(inp.e.unwrap_or(0.5));

    // Base choice pools
    let mut choices: Vec<ChoiceOut> = Vec::new();

    match style {
        "quest_intro" => {
            choices.push(ChoiceOut { id: "accept", tag: "accept", text: format!("I'll take it: {topic}.") });
            choices.push(ChoiceOut { id: "decline", tag: "decline", text: "Not today.".into() });
            choices.push(ChoiceOut { id: "ask_details", tag: "ask", text: "Tell me more first.".into() });
            // Personality-driven
            if o > 0.6 { choices.push(ChoiceOut { id: "alt_approach", tag: "creative", text: "There's another way we could try.".into() }); }
            if e > 0.6 { choices.push(ChoiceOut { id: "bold", tag: "bold", text: "Leave it to me. I'll make it quick.".into() }); }
            if c > 0.6 { choices.push(ChoiceOut { id: "plan", tag: "plan", text: "Give me the plan and I'll execute.".into() }); }
        }
        "quest_progress" => {
            choices.push(ChoiceOut { id: "status", tag: "status", text: "I'm still working on it.".into() });
            choices.push(ChoiceOut { id: "blocker", tag: "blocker", text: "There's a complication.".into() });
            choices.push(ChoiceOut { id: "hint", tag: "hint", text: "Any advice to speed this up?".into() });
            if o > 0.6 { choices.push(ChoiceOut { id: "detour", tag: "creative", text: "I found a detour that might be safer.".into() }); }
            if e > 0.6 { choices.push(ChoiceOut { id: "charge", tag: "bold", text: "I'll push harder and finish it.".into() }); }
            if c > 0.6 { choices.push(ChoiceOut { id: "report", tag: "report", text: "Progress is steady; expect completion soon.".into() }); }
        }
        "quest_complete" => {
            choices.push(ChoiceOut { id: "turn_in", tag: "complete", text: "It's done. Here's proof.".into() });
            choices.push(ChoiceOut { id: "negotiate", tag: "bargain", text: "That was risky. Can we discuss the reward?".into() });
            choices.push(ChoiceOut { id: "followup", tag: "followup", text: "Got any follow-up work?".into() });
            if o > 0.6 { choices.push(ChoiceOut { id: "insight", tag: "insight", text: "I learned something out there.".into() }); }
            if e > 0.6 { choices.push(ChoiceOut { id: "brag", tag: "bold", text: "Clean and swift. What's next?".into() }); }
            if c > 0.6 { choices.push(ChoiceOut { id: "summary", tag: "report", text: "Task complete. Summary ready if needed.".into() }); }
        }
        _ => {
            choices.push(ChoiceOut { id: "ok", tag: "ok", text: "Understood.".into() });
            choices.push(ChoiceOut { id: "question", tag: "ask", text: "I have a question.".into() });
            choices.push(ChoiceOut { id: "goodbye", tag: "bye", text: "I'll be on my way.".into() });
        }
    }
    // Tone variants: lightly swap phrasing
    if tone == "friendly" { for ch in &mut choices { if ch.text.ends_with('.') { ch.text.push_str(" Thanks."); } } }
    if tone == "gruff" { for ch in &mut choices { ch.text = ch.text.replace("Please ", ""); } }

    // Shuffle and trim to requested count
    choices.shuffle(&mut rng);
    let want = inp.count.unwrap_or(4) as usize;
    if choices.len() > want { choices.truncate(want); }
    choices
}

#[reducer]
pub fn nlp_generate_choices(ctx: &ReducerContext, input_json: String) -> Result<String, String> {
    let inp: ChoiceInput = serde_json::from_str(&input_json).map_err(|e| e.to_string())?;
    // Seed by player + quest to keep choices stable across a session/day
    let mut mix: u64 = ctx.sender.as_u64() ^ (ctx.timestamp.to_micros_since_unix_epoch() as u64 / 86_400_000_000); // per day
    if let Some(qid) = &inp.quest_id { for b in qid.as_bytes() { mix = mix.wrapping_mul(131).wrapping_add(*b as u64); } }
    let rng = StdRng::seed_from_u64(mix);
    let list = build_choices(rng, &inp);
    let json_out = json!({ "choices": list });
    Ok(json_out.to_string())
}

// Persist player choice (optional; clients can call this after selecting)
#[table(name = player_quest_choice, public)]
pub struct PlayerQuestChoice {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub identity: Identity,
    pub quest_id: String,
    pub choice_id: String,
    pub chosen_at: Timestamp,
}

#[reducer]
pub fn record_quest_choice(ctx: &ReducerContext, quest_id: String, choice_id: String) -> Result<(), String> {
    ctx.db.player_quest_choice().insert(PlayerQuestChoice {
        id: 0,
        identity: ctx.sender,
        quest_id,
        choice_id,
        chosen_at: ctx.timestamp,
    });
    Ok(())
}
#[cfg(feature = "nlp_webhook")]
fn try_webhook(req: &NlpInput) -> Option<String> {
    let url = if let Some(u) = &req.webhook_url { if u.is_empty() { None } else { Some(u.clone()) } } else { std::env::var("NLP_WEBHOOK_URL").ok() }?;
    let body = serde_json::to_string(req).ok()?;
    let agent = ureq::AgentBuilder::new().timeout(std::time::Duration::from_millis(1500)).build();
    let resp = agent.post(&url).set("content-type", "application/json").send_string(&body).ok()?;
    let text = resp.into_string().ok()?;
    // Try parse JSON with {"text": ...}
    if let Ok(v) = serde_json::from_str::<serde_json::Value>(&text) {
        if let Some(t) = v.get("text").and_then(|x| x.as_str()) { return Some(t.to_string()); }
    }
    // Otherwise return raw body (truncated)
    Some(text.chars().take(600).collect())
}

#[cfg(not(feature = "nlp_webhook"))]
fn try_webhook(_req: &NlpInput) -> Option<String> { None }

#[reducer]
pub fn set_avatar(ctx: &ReducerContext, avatar_json: String) -> Result<(), String> {
    let input: AvatarIn = serde_json::from_str(&avatar_json).map_err(|e| e.to_string())?;
    validate_avatar(&input)?;
    let body_json = serde_json::to_string(&input.body).unwrap_or_else(|_| "{}".to_string());
    let face_json = serde_json::to_string(&input.face).unwrap_or_else(|_| "{}".to_string());
    if let Some(mut existing) = ctx.db.avatar().identity().find(ctx.sender) {
        existing.height_cm = input.height_cm;
        existing.weight_kg = input.weight_kg;
        existing.body_json = body_json;
        existing.face_json = face_json;
        existing.updated_at = ctx.timestamp;
        ctx.db.avatar().identity().update(existing);
    } else {
        ctx.db.avatar().insert(Avatar {
            identity: ctx.sender,
            height_cm: input.height_cm,
            weight_kg: input.weight_kg,
            body_json,
            face_json,
            created_at: ctx.timestamp,
            updated_at: ctx.timestamp,
        });
    }
    Ok(())
}
// JSON import shapes (subset)
#[derive(Deserialize)]
struct AbilityIn {
    id: String,
    name: String,
    branch: String,
    tier: u8,
    #[serde(rename = "type")] kind: String,
    #[serde(default)] description: String,
    #[serde(default)] cooldown_s: Option<f32>,
    #[serde(default)] resource: Option<ResourceIn>,
}

#[derive(Deserialize)]
struct ResourceIn { r#type: Option<String>, cost: Option<f32> }

#[derive(Deserialize)]
struct PerkIn {
    id: String,
    name: String,
    tier: String,
    #[serde(default)] tags: Vec<String>,
    #[serde(default)] description: String,
    #[serde(default)] rolls: Vec<RollIn>,
}

#[derive(Deserialize)]
struct RollIn { stat: Option<String>, min: Option<f32>, max: Option<f32> }

// Items: auto-incrementing primary key; indexed by owner for fast lookup
#[table(name = item, public)]
pub struct Item {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub owner: Identity,
    pub name: String,
    pub qty: u32,
}

// Consumables catalog and effects
#[table(name = consumable_catalog, public)]
pub struct ConsumableCatalog {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub tier: u8,
    pub category: String,
    pub cooldown_s: u32,
    pub stack: String,
}

#[table(name = consumable_effect, public)]
pub struct ConsumableEffect {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub consumable_id: String,
    pub kind: String,
    pub amount: f32,
    pub duration_s: u32,
}

// Player-side active buffs + per-consumable cooldowns
#[table(name = player_active_buff, public)]
pub struct PlayerActiveBuff {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub identity: Identity,
    pub buff_id: String,
    pub tier: u8,
    pub stacks: u8,
    pub stack_mode: String,
    pub started_at: Timestamp,
    pub expires_at: Timestamp,
}

#[table(name = player_consumable_cooldown, public)]
pub struct PlayerConsumableCooldown {
    #[primary_key]
    pub identity_consumable: String, // identity:consumable_id
    #[index(btree)]
    pub identity: Identity,
    pub consumable_id: String,
    pub available_at: Timestamp,
}

fn validate_name(name: String) -> Result<String, String> {
    let trimmed = name.trim();
    if trimmed.is_empty() {
        return Err("Name must not be empty".to_string());
    }
    if trimmed.len() > 32 {
        return Err("Name too long (max 32 chars)".to_string());
    }
    Ok(trimmed.to_string())
}

#[reducer]
pub fn register_player(ctx: &ReducerContext) -> Result<(), String> {
    // If a Player row for this identity exists, mark success; otherwise insert one.
    if let Some(_existing) = ctx.db.player().identity().find(ctx.sender) {
        return Ok(());
    }

    ctx.db.player().insert(Player {
        identity: ctx.sender,
        name: None,
        created_at: ctx.timestamp,
    });
    Ok(())
}

#[reducer]
pub fn set_player_name(ctx: &ReducerContext, name: String) -> Result<(), String> {
    let name = validate_name(name)?;
    if let Some(player) = ctx.db.player().identity().find(ctx.sender) {
        ctx.db.player().identity().update(Player { name: Some(name), ..player });
        Ok(())
    } else {
        Err("Player not registered".to_string())
    }
}

#[reducer]
pub fn increment_world_tick(ctx: &ReducerContext) -> Result<(), String> {
    const WORLD_ROW_ID: u32 = 1;
    let next_tick = if let Some(ws) = ctx.db.world().id().find(WORLD_ROW_ID) {
        let t = ws.tick.saturating_add(1);
        ctx.db.world().id().update(WorldState { tick: t, ..ws });
        t
    } else {
        ctx.db.world().insert(WorldState { id: WORLD_ROW_ID, tick: 1 });
        1
    };
    let _ = next_tick; // explicit no-return per reducer contract
    Ok(())
}

#[reducer]
pub fn grant_item(
    ctx: &ReducerContext,
    owner: Identity,
    name: String,
    qty: u32,
) -> Result<(), String> {
    if qty == 0 { return Err("qty must be > 0".into()); }
    let final_name = name.trim();
    if final_name.is_empty() { return Err("item name must not be empty".into()); }

    // Ensure the owner has a player row; if not, create a placeholder
    if ctx.db.player().identity().find(owner).is_none() {
        ctx.db.player().insert(Player { identity: owner, name: None, created_at: ctx.timestamp });
    }

    // Insert item; #[auto_inc] generates the id server-side
    ctx.db.item().insert(Item {
        id: 0, // ignored because of #[auto_inc]
        owner,
        name: final_name.to_string(),
        qty,
    });
    Ok(())
}

// Networking: update player state at a limited cadence
#[reducer]
pub fn update_player_state(ctx: &ReducerContext, x: f32, y: f32, z: f32, yaw_deg: f32) -> Result<(), String> {
    // Clamp to reasonable world bounds to prevent abuse
    let x = x.clamp(-10_000.0, 10_000.0);
    let y = y.clamp(-1000.0, 1000.0);
    let z = z.clamp(-10_000.0, 10_000.0);
    let yaw = if yaw_deg.is_finite() { yaw_deg } else { 0.0 };
    // Rate limit: ignore updates if last was < 50ms ago
    if let Some(row) = ctx.db.player_state().identity().find(ctx.sender) {
        let last = row.updated_at.to_micros_since_unix_epoch();
        if ctx.timestamp.to_micros_since_unix_epoch().saturating_sub(last) < 50_000 { return Ok(()); }
        ctx.db.player_state().identity().update(PlayerState { x, y, z, yaw_deg: yaw, updated_at: ctx.timestamp, ..row });
    } else {
        ctx.db.player_state().insert(PlayerState { identity: ctx.sender, region_id: 0, x, y, z, yaw_deg: yaw, updated_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn set_player_region(ctx: &ReducerContext, region_id: u8) -> Result<(), String> {
    let r = region_id; // validate range if needed
    if let Some(row) = ctx.db.player_state().identity().find(ctx.sender) {
        ctx.db.player_state().identity().update(PlayerState { region_id: r, ..row });
    } else {
        ctx.db.player_state().insert(PlayerState { identity: ctx.sender, region_id: r, x: 0.0, y: 0.0, z: 0.0, yaw_deg: 0.0, updated_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn clear_stale_player_states(ctx: &ReducerContext, max_age_seconds: u32) -> Result<(), String> {
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    let max_age_us = (max_age_seconds as i64) * 1_000_000;
    for row in ctx.db.player_state().iter() {
        if now.saturating_sub(row.updated_at.to_micros_since_unix_epoch()) > max_age_us {
            ctx.db.player_state().identity().delete(&row.identity);
        }
    }
    Ok(())
}

// -----------------
// Consumables & Buffs
// -----------------

#[derive(Deserialize)]
struct ConsumableEffectIn { kind: String, amount: f32, duration_s: u32 }
#[derive(Deserialize)]
struct ConsumableIn { id: String, name: String, tier: u8, category: String, cooldown_s: u32, stack: String, effects: Vec<ConsumableEffectIn> }
#[derive(Deserialize)]
struct ConsumablesEnvelope { consumables: Vec<ConsumableIn> }

#[reducer]
pub fn import_consumables(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: ConsumablesEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    // Clear catalogs
    for row in ctx.db.consumable_catalog().iter() { ctx.db.consumable_catalog().id().delete(&row.id); }
    for row in ctx.db.consumable_effect().iter() { ctx.db.consumable_effect().id().delete(&row.id); }
    // Insert
    for c in env.consumables.into_iter() {
        ctx.db.consumable_catalog().insert(ConsumableCatalog { id: c.id.clone(), name: c.name, tier: c.tier, category: c.category, cooldown_s: c.cooldown_s, stack: c.stack });
        for e in c.effects.into_iter() {
            ctx.db.consumable_effect().insert(ConsumableEffect { id: 0, consumable_id: c.id.clone(), kind: e.kind, amount: e.amount, duration_s: e.duration_s });
        }
    }
    Ok(())
}

fn max_effect_duration_for(ctx: &ReducerContext, consumable_id: &str) -> u32 {
    let mut maxd: u32 = 0;
    for e in ctx.db.consumable_effect().consumable_id().filter(&consumable_id.to_string()) {
        if e.duration_s > maxd { maxd = e.duration_s; }
    }
    maxd
}

fn take_one_item(ctx: &ReducerContext, owner: Identity, name: &str) -> Result<(), String> {
    // Find any item row with given name and reduce qty by 1
    for it in ctx.db.item().owner().filter(&owner) {
        if it.name == name {
            if it.qty == 0 { continue; }
            if it.qty == 1 { ctx.db.item().id().delete(&it.id); }
            else { ctx.db.item().id().update(Item { qty: it.qty - 1, ..it }); }
            return Ok(());
        }
    }
    Err("Not enough consumables".into())
}

#[reducer]
pub fn grant_consumable_to_self(ctx: &ReducerContext, consumable_id: String, qty: u32) -> Result<(), String> {
    if qty == 0 { return Err("qty must be > 0".into()); }
    if ctx.db.consumable_catalog().id().find(consumable_id.clone()).is_none() { return Err("Unknown consumable".into()); }
    ctx.db.item().insert(Item { id: 0, owner: ctx.sender, name: consumable_id, qty });
    Ok(())
}

#[reducer]
pub fn consume_consumable(ctx: &ReducerContext, consumable_id: String) -> Result<(), String> {
    // Validate
    let cat = ctx.db.consumable_catalog().id().find(consumable_id.clone()).ok_or_else(|| "Unknown consumable".to_string())?;
    // Cooldown check
    let key = format!("{}:{}", ctx.sender, consumable_id);
    if let Some(cd) = ctx.db.player_consumable_cooldown().identity_consumable().find(key.clone()) {
        if cd.available_at.to_micros_since_unix_epoch() > ctx.timestamp.to_micros_since_unix_epoch() {
            return Err("Consumable on cooldown".into());
        }
    }
    // Inventory check/deduct
    take_one_item(ctx, ctx.sender, &consumable_id)?;
    // Apply buff stacking rules
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    let dur_s = max_effect_duration_for(ctx, &consumable_id);
    let expires = spacetimedb::Timestamp::from_micros_since_unix_epoch(now + (dur_s as i64) * 1_000_000);
    // Find existing buff row
    let mut existing: Option<PlayerActiveBuff> = None;
    for b in ctx.db.player_active_buff().identity().filter(&ctx.sender) {
        if b.buff_id == consumable_id { existing = Some(b); break; }
    }
    match cat.stack.as_str() {
        "Unique" => {
            if let Some(mut row) = existing { row.started_at = ctx.timestamp; row.expires_at = expires; row.stacks = 1; row.stack_mode = cat.stack.clone(); ctx.db.player_active_buff().id().update(row); }
            else { ctx.db.player_active_buff().insert(PlayerActiveBuff { id: 0, identity: ctx.sender, buff_id: consumable_id.clone(), tier: cat.tier, stacks: 1, stack_mode: cat.stack.clone(), started_at: ctx.timestamp, expires_at: expires }); }
        }
        "StackDiminishing" => {
            if let Some(mut row) = existing { row.stacks = row.stacks.saturating_add(1).min(5); if row.expires_at.to_micros_since_unix_epoch() < expires.to_micros_since_unix_epoch() { row.expires_at = expires; } row.stack_mode = cat.stack.clone(); ctx.db.player_active_buff().id().update(row); }
            else { ctx.db.player_active_buff().insert(PlayerActiveBuff { id: 0, identity: ctx.sender, buff_id: consumable_id.clone(), tier: cat.tier, stacks: 1, stack_mode: cat.stack.clone(), started_at: ctx.timestamp, expires_at: expires }); }
        }
        _ /* RefreshDuration or default */ => {
            if let Some(mut row) = existing { row.expires_at = expires; row.started_at = ctx.timestamp; row.stack_mode = cat.stack.clone(); ctx.db.player_active_buff().id().update(row); }
            else { ctx.db.player_active_buff().insert(PlayerActiveBuff { id: 0, identity: ctx.sender, buff_id: consumable_id.clone(), tier: cat.tier, stacks: 1, stack_mode: cat.stack.clone(), started_at: ctx.timestamp, expires_at: expires }); }
        }
    }
    // Set cooldown
    let avail = spacetimedb::Timestamp::from_micros_since_unix_epoch(now + (cat.cooldown_s as i64) * 1_000_000);
    if let Some(mut cd) = ctx.db.player_consumable_cooldown().identity_consumable().find(key.clone()) {
        cd.available_at = avail; ctx.db.player_consumable_cooldown().identity_consumable().update(cd);
    } else {
        ctx.db.player_consumable_cooldown().insert(PlayerConsumableCooldown { identity_consumable: key, identity: ctx.sender, consumable_id: consumable_id.clone(), available_at: avail });
    }
    Ok(())
}

#[reducer]
pub fn clear_expired_buffs(ctx: &ReducerContext) -> Result<(), String> {
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    for b in ctx.db.player_active_buff().iter() {
        if b.expires_at.to_micros_since_unix_epoch() <= now {
            ctx.db.player_active_buff().id().delete(&b.id);
        }
    }
    Ok(())
}

#[reducer]
pub fn init_world(ctx: &ReducerContext, seed: u64, size_meters: u32, chunk_size_m: u32) -> Result<(), String> {
    const CFG_ID: u32 = 1;
    if let Some(cfg) = ctx.db.world_config().id().find(CFG_ID) {
        // Update existing configuration if values changed
        if cfg.seed != seed || cfg.size_meters != size_meters || cfg.chunk_size_m != chunk_size_m {
            ctx.db.world_config().id().update(WorldConfig { seed, size_meters, chunk_size_m, ..cfg });
        }
    } else {
        ctx.db.world_config().insert(WorldConfig { id: CFG_ID, seed, size_meters, chunk_size_m, created_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn clear_class_data(ctx: &ReducerContext, class_name: String) -> Result<(), String> {
    // Remove abilities
    let key = class_name.clone();
    for a in ctx.db.ability().class_name().filter(&key) {
        ctx.db.ability().id().delete(&a.id);
    }
    // Remove perks and catalog
    for p in ctx.db.perk().class_name().filter(&key) {
        ctx.db.perk().id().delete(&p.id);
    }
    if let Some(cat) = ctx.db.perk_catalog().class_name().find(class_name.clone()) {
        ctx.db.perk_catalog().class_name().delete(&cat.class_name);
    }
    if let Some(row) = ctx.db.class_catalog().class_name().find(class_name.clone()) {
        ctx.db.class_catalog().class_name().delete(&row.class_name);
    }
    Ok(())
}

#[reducer]
pub fn import_abilities(ctx: &ReducerContext, class_name: String, abilities_json: String) -> Result<(), String> {
    let parsed: serde_json::Value = serde_json::from_str(&abilities_json).map_err(|e| e.to_string())?;
    let arr = match parsed.get("abilities") { Some(x) => x, None => &parsed };
    let items: Vec<AbilityIn> = serde_json::from_value(arr.clone()).map_err(|e| e.to_string())?;
    for it in items.into_iter() {
        let (rt, rc) = it.resource.map(|r| (r.r#type, r.cost)).unwrap_or((None, None));
        let ability = Ability {
            id: it.id,
            class_name: class_name.clone(),
            name: it.name,
            branch: it.branch,
            tier: it.tier,
            kind: it.kind,
            resource_type: rt,
            resource_cost: rc,
            cooldown_s: it.cooldown_s,
            description: it.description,
        };
        // Upsert simplistic: try delete then insert to avoid unique conflicts
        if ctx.db.ability().id().find(&ability.id).is_some() {
            ctx.db.ability().id().delete(&ability.id);
        }
        ctx.db.ability().insert(ability);
    }
    // Ensure class exists in catalog
    if let Some(row) = ctx.db.class_catalog().class_name().find(class_name.clone()) {
        ctx.db.class_catalog().class_name().update(ClassCatalog { updated_at: ctx.timestamp, ..row });
    } else {
        ctx.db.class_catalog().insert(ClassCatalog { class_name: class_name.clone(), updated_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn import_perks(ctx: &ReducerContext, class_name: String, perks_json: String) -> Result<(), String> {
    let parsed: serde_json::Value = serde_json::from_str(&perks_json).map_err(|e| e.to_string())?;
    let arr = match parsed.get("perks") { Some(x) => x, None => &parsed };
    let items: Vec<PerkIn> = serde_json::from_value(arr.clone()).map_err(|e| e.to_string())?;
    let mut ids: Vec<String> = Vec::new();
    for it in items.into_iter() {
        let (mut stat, mut rmin, mut rmax) = (None, None, None);
        if let Some(roll) = it.rolls.get(0) {
            stat = roll.stat.clone(); rmin = roll.min; rmax = roll.max;
        }
        let tags_csv = it.tags.join(",");
        let perk = Perk {
            id: it.id.clone(),
            class_name: class_name.clone(),
            name: it.name,
            tier: it.tier,
            tags_csv,
            effect_desc: String::new(),
            roll_stat: stat,
            roll_min: rmin,
            roll_max: rmax,
            description: it.description,
        };
        if ctx.db.perk().id().find(&perk.id).is_some() {
            ctx.db.perk().id().delete(&perk.id);
        }
        ctx.db.perk().insert(perk);
        ids.push(it.id);
    }
    let ids_csv = ids.join(",");
    if let Some(cat) = ctx.db.perk_catalog().class_name().find(class_name.clone()) {
        ctx.db.perk_catalog().class_name().update(PerkCatalog { ids_csv: ids_csv.clone(), updated_at: ctx.timestamp, ..cat });
    } else {
        ctx.db.perk_catalog().insert(PerkCatalog { class_name: class_name.clone(), ids_csv, updated_at: ctx.timestamp });
    }
    // Ensure class exists in catalog
    if let Some(row) = ctx.db.class_catalog().class_name().find(class_name.clone()) {
        ctx.db.class_catalog().class_name().update(ClassCatalog { updated_at: ctx.timestamp, ..row });
    } else {
        ctx.db.class_catalog().insert(ClassCatalog { class_name: class_name.clone(), updated_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn refresh_class_catalog(ctx: &ReducerContext) -> Result<(), String> {
    use std::collections::BTreeSet;
    let mut set: BTreeSet<String> = BTreeSet::new();
    for a in ctx.db.ability().iter() { set.insert(a.class_name); }
    for p in ctx.db.perk().iter() { set.insert(p.class_name); }
    // Clear all rows
    for row in ctx.db.class_catalog().iter() {
        ctx.db.class_catalog().class_name().delete(&row.class_name);
    }
    // Insert new
    for name in set.into_iter() {
        ctx.db.class_catalog().insert(ClassCatalog { class_name: name, updated_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn roll_weapon(
    ctx: &ReducerContext,
    class_name: String,
    weapon_type: String,
    level: u32,
    num_perks: u8,
) -> Result<(), String> {
    // Create weapon instance with random unique id
    let mut rng = StdRng::seed_from_u64((ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64) ^ (level as u64));
    let base_power = 10 + level as u32 * 2;
    let weapon_id = loop {
        let candidate: u64 = rng.gen();
        if candidate == 0 { continue; }
        if ctx.db.weapon_instance().id().find(candidate).is_none() { break candidate; }
    };
    // Choose rarity based on weights
    let mut rarities: Vec<RarityConfig> = ctx.db.rarity_config().iter().collect();
    if rarities.is_empty() {
        // default weights
        for (name, weight, order) in [("Common",60,1),("Uncommon",25,2),("Rare",10,3),("Epic",4,4),("Mythic",1,5)] {
            ctx.db.rarity_config().insert(RarityConfig { name: name.to_string(), weight, order });
        }
        rarities = ctx.db.rarity_config().iter().collect();
    }
    let total: u32 = rarities.iter().map(|r| r.weight).sum();
    let pick = rng.gen_range(0..total.max(1));
    let mut acc = 0u32; let mut rarity = "Common".to_string();
    for r in rarities.iter() { acc += r.weight; if pick < acc { rarity = r.name.clone(); break; } }

    ctx.db.weapon_instance().insert(WeaponInstance {
        id: weapon_id,
        owner: ctx.sender,
        class_name: class_name.clone(),
        weapon_type: weapon_type.clone(),
        rarity,
        power: base_power,
        level,
        created_at: ctx.timestamp,
    });

    // Fetch catalog
    let cat = ctx.db.perk_catalog().class_name().find(class_name.clone()).ok_or_else(|| "No perk catalog for class".to_string())?;
    let ids: Vec<&str> = cat.ids_csv.split(',').filter(|s| !s.is_empty()).collect();
    if ids.is_empty() { return Err("Empty perk catalog".into()); }

    // Sample unique perks
    let mut rng = StdRng::seed_from_u64((ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64) ^ (weapon_id as u64));
    use std::collections::HashSet;
    let mut chosen: HashSet<usize> = HashSet::new();
    let slots = num_perks.max(1).min(6) as usize;
    let n = ids.len();
    for _ in 0..slots {
        let mut idx = rng.gen_range(0..n);
        let mut guard = 0;
        while chosen.contains(&idx) && guard < 10 {
            idx = rng.gen_range(0..n); guard += 1;
        }
        chosen.insert(idx);
        let perk_id = ids[idx].to_string();
        // Lookup roll range
        let p = ctx.db.perk().id().find(perk_id.clone()).ok_or_else(|| format!("Perk not found: {}", perk_id))?;
        let (stat, val) = match (p.roll_stat.clone(), p.roll_min, p.roll_max) {
            (Some(s), Some(a), Some(b)) => {
                let v = rng.gen_range(a..=b);
                (Some(s), Some(v))
            }
            _ => (None, None)
        };
        ctx.db.weapon_perk().insert(WeaponPerk {
            id: 0,
            weapon_id: weapon_id,
            perk_id: perk_id.clone(),
            perk_name: p.name,
            rolled_stat: stat,
            rolled_value: val,
        });
    }
    Ok(())
}

#[reducer]
pub fn clear_enemy_types(ctx: &ReducerContext) -> Result<(), String> {
    for e in ctx.db.enemy_type().iter() {
        ctx.db.enemy_type().id().delete(&e.id);
    }
    Ok(())
}

#[reducer]
pub fn import_enemy_types(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let parsed: serde_json::Value = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    let arr = parsed.get("enemies").cloned().unwrap_or(parsed);
    let items: Vec<EnemyIn> = serde_json::from_value(arr).map_err(|e| e.to_string())?;
    for it in items.into_iter() {
        let abilities_csv = it.abilities.join(",");
        let tags_csv = it.tags.join(",");
        let row = EnemyType {
            id: it.id.clone(),
            name: it.name,
            family: it.family,
            tier: it.tier,
            role: it.role,
            element: it.element,
            size: it.size,
            movement: it.movement,
            armor: it.armor,
            hp: it.hp,
            damage: it.damage,
            speed: it.speed,
            abilities_csv,
            tags_csv,
            description: it.description,
        };
        if ctx.db.enemy_type().id().find(&row.id).is_some() {
            ctx.db.enemy_type().id().delete(&row.id);
        }
        ctx.db.enemy_type().insert(row);
    }
    Ok(())
}

#[reducer]
pub fn clear_world_regions(ctx: &ReducerContext) -> Result<(), String> {
    for r in ctx.db.world_region().iter() {
        ctx.db.world_region().id().delete(&r.id);
    }
    Ok(())
}

#[reducer]
pub fn import_world_regions(ctx: &ReducerContext, biomes_json: String) -> Result<(), String> {
    let env: BiomesEnvelope = serde_json::from_str(&biomes_json).map_err(|e| e.to_string())?;
    for b in env.biomes.into_iter() {
        let json = serde_json::json!({
            "palette": b.palette,
            "lighting": b.lighting,
            "vfx": b.vfx,
            "props": b.props,
            "spawn_weights": b.spawn_weights,
            "description": b.description
        }).to_string();
        let row = WorldRegion {
            id: b.id,
            name: b.name,
            seed: b.seed,
            size_meters: b.size_meters,
            chunk_size_m: b.chunk_size_m,
            biome_json: json,
            created_at: ctx.timestamp,
            updated_at: ctx.timestamp,
        };
        if ctx.db.world_region().id().find(b.id).is_some() {
            ctx.db.world_region().id().delete(&b.id);
        }
        ctx.db.world_region().insert(row);
    }
    Ok(())
}

#[reducer]
pub fn clear_bosses(ctx: &ReducerContext) -> Result<(), String> {
    for p in ctx.db.armor_piece().iter() { ctx.db.armor_piece().id().delete(&p.id); }
    for s in ctx.db.armor_set().iter() { ctx.db.armor_set().id().delete(&s.id); }
    for b in ctx.db.boss_type().iter() { ctx.db.boss_type().id().delete(&b.id); }
    Ok(())
}

#[reducer]
pub fn import_bosses(ctx: &ReducerContext, bosses_json: String) -> Result<(), String> {
    let env: BossesEnvelope = serde_json::from_str(&bosses_json).map_err(|e| e.to_string())?;
    for b in env.bosses.into_iter() {
        // armor set
        let bonuses_csv = b.armor_set.set_bonuses.join(",");
        let set_row = ArmorSet { id: b.armor_set.id.clone(), name: b.armor_set.name.clone(), set_bonuses_csv: bonuses_csv };
        if ctx.db.armor_set().id().find(set_row.id.clone()).is_some() { ctx.db.armor_set().id().delete(&set_row.id); }
        ctx.db.armor_set().insert(set_row);
        for pc in b.armor_set.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: b.armor_set.id.clone(), slot: pc.slot, name: pc.name, bonus: pc.bonus });
        }
        let boss_row = BossType {
            id: b.id,
            name: b.name,
            biome: b.biome,
            tier: b.tier,
            mechanics_csv: b.mechanics.join(","),
            puzzle_modules_csv: b.puzzle_modules.join(","),
            phases_csv: b.phases.join(","),
            armor_set_id: b.armor_set.id,
            description: b.description,
        };
        if ctx.db.boss_type().id().find(boss_row.id.clone()).is_some() { ctx.db.boss_type().id().delete(&boss_row.id); }
        ctx.db.boss_type().insert(boss_row);
    }
    Ok(())
}

#[reducer]
pub fn clear_world_bosses(ctx: &ReducerContext) -> Result<(), String> {
    for s in ctx.db.world_boss_spawn().iter() { ctx.db.world_boss_spawn().id().delete(&s.id); }
    for t in ctx.db.world_boss_type().iter() { ctx.db.world_boss_type().id().delete(&t.id); }
    Ok(())
}

#[reducer]
pub fn import_world_bosses(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: WorldBossEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    for b in env.world_bosses.into_iter() {
        let env_json = serde_json::to_string(&b.env_effect).unwrap_or_else(|_| "{}".to_string());
        let row = WorldBossType { id: b.id.clone(), name: b.name, biome: b.biome, tier: b.tier, env_json, min_ng_plus: b.min_ng_plus, description: b.description };
        if ctx.db.world_boss_type().id().find(&row.id).is_some() { ctx.db.world_boss_type().id().delete(&row.id); }
        ctx.db.world_boss_type().insert(row);
    }
    Ok(())
}

#[reducer]
pub fn spawn_world_boss_random(ctx: &ReducerContext, boss_id: String, region_id: u8) -> Result<(), String> {
    let _boss = ctx.db.world_boss_type().id().find(boss_id.clone()).ok_or_else(|| "Unknown world boss".to_string())?;
    let region = ctx.db.world_region().id().find(region_id).ok_or_else(|| "Unknown region".to_string())?;
    // Pick a random (x,z) inside region bounds [0, size)
    let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
    let x = rng.gen_range(0.0..region.size_meters as f32);
    let z = rng.gen_range(0.0..region.size_meters as f32);
    ctx.db.world_boss_spawn().insert(WorldBossSpawn { id: 0, boss_id, region_id, x, z, alive: true, spawned_at: ctx.timestamp });
    Ok(())
}

#[reducer]
pub fn defeat_world_boss(ctx: &ReducerContext, spawn_id: u64) -> Result<(), String> {
    if let Some(mut s) = ctx.db.world_boss_spawn().id().find(spawn_id) {
        if !s.alive { return Err("Already defeated".into()); }
        s.alive = false; ctx.db.world_boss_spawn().id().update(s);
        // Shards + chance to drop artifact
        if let Some(mut cur) = ctx.db.player_currency().identity().find(ctx.sender) {
            cur.mythic_shards = cur.mythic_shards.saturating_add(25);
            cur.updated_at = ctx.timestamp; ctx.db.player_currency().identity().update(cur);
        } else {
            ctx.db.player_currency().insert(PlayerCurrency { identity: ctx.sender, credits: 0, mythic_shards: 25, updated_at: ctx.timestamp });
        }
        // 20% chance to award a Mythic/Epic artifact if available
        let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
        let roll: u32 = rng.gen_range(0..100);
        if roll < 20 {
            // prefer Mythic else Epic
            let mythics: Vec<ArtifactType> = ctx.db.artifact_type().iter().filter(|a| a.rarity == "Mythic").collect();
            let epics: Vec<ArtifactType> = ctx.db.artifact_type().iter().filter(|a| a.rarity == "Epic").collect();
            let art_id = if !mythics.is_empty() {
                mythics[rng.gen_range(0..mythics.len())].id.clone()
            } else if !epics.is_empty() {
                epics[rng.gen_range(0..epics.len())].id.clone()
            } else { String::new() };
            if !art_id.is_empty() {
                ctx.db.owned_artifact().insert(OwnedArtifact { id: 0, identity: ctx.sender, artifact_id: art_id, source: "world_boss".into(), acquired_at: ctx.timestamp });
            }
        }
        Ok(())
    } else {
        Err("Spawn not found".into())
    }
}

#[reducer]
pub fn spawn_world_boss_if_allowed(ctx: &ReducerContext, region_id: u8) -> Result<(), String> {
    // Determine region biome
    let region = ctx.db.world_region().id().find(region_id).ok_or_else(|| "Unknown region".to_string())?;
    let biome_name = region.name.clone();
    // Player NG+
    let ng = ctx.db.player_ng_plus().identity().find(ctx.sender).map(|p| p.level).unwrap_or(0);
    // Find world boss for biome
    let mut boss_for_biome: Option<WorldBossType> = None;
    for t in ctx.db.world_boss_type().iter() {
        if t.biome == biome_name { boss_for_biome = Some(t); break; }
    }
    let Some(boss) = boss_for_biome else { return Err("No world boss defined for biome".into()) };
    if ng < boss.min_ng_plus { return Err("NG+ level too low for world boss".into()); }
    // If alive spawn already in this region, do nothing
    for s in ctx.db.world_boss_spawn().boss_id().filter(&boss.id) {
        if s.region_id == region_id && s.alive { return Ok(()); }
    }
    // Spawn at random location
    let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
    let x = rng.gen_range(0.0..region.size_meters as f32);
    let z = rng.gen_range(0.0..region.size_meters as f32);
    ctx.db.world_boss_spawn().insert(WorldBossSpawn { id: 0, boss_id: boss.id, region_id, x, z, alive: true, spawned_at: ctx.timestamp });
    Ok(())
}

#[table(name = owned_armor_piece, public)]
pub struct OwnedArmorPiece {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub owner: Identity,
    pub set_id: String,
    pub slot: String,
    pub piece_name: String,
    pub bonus: String,
    pub acquired_at: Timestamp,
}

// Extended Armor Catalog (world/faction/enemy sets)
#[table(name = armor_set_info, public)]
pub struct ArmorSetInfo {
    #[primary_key]
    pub set_id: String,
    pub category: String, // World | Faction | Enemy | Boss
    pub faction_name: Option<String>,
    pub enemy_family: Option<String>,
    pub rep_required: Option<u32>,
}

#[table(name = armor_perk_type, public)]
pub struct ArmorPerkType {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub stat: String,
    pub min: f32,
    pub max: f32,
    pub rarity: String,
}

#[table(name = owned_armor_piece_perk, public)]
pub struct OwnedArmorPiecePerk {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub owned_piece_id: u64,
    pub perk_id: String,
    pub rolled_value: f32,
}

// Import JSON shapes for armor
#[derive(Deserialize)]
struct ArmorSetIn { id: String, name: String, pieces: Vec<ArmorPieceIn> }
#[derive(Deserialize)]
struct ArmorPieceIn { slot: String, name: String, rarity: String }
#[derive(Deserialize)]
struct ArmorSetsEnvelope { sets: Vec<ArmorSetIn> }
#[derive(Deserialize)]
struct FactionSetIn { id: String, name: String, faction: String, pieces: Vec<ArmorPieceIn>, rep_required: u32 }
#[derive(Deserialize)]
struct FactionSetsEnvelope { sets: Vec<FactionSetIn> }
#[derive(Deserialize)]
struct EnemySetIn { id: String, name: String, enemy_family: String, pieces: Vec<ArmorPieceIn> }
#[derive(Deserialize)]
struct EnemySetsEnvelope { sets: Vec<EnemySetIn> }
#[derive(Deserialize)]
struct PerksEnvelope { perks: Vec<PerkIn2> }
#[derive(Deserialize)]
struct PerkIn2 { id: String, name: String, stat: String, min: f32, max: f32, rarity: String }

#[reducer]
pub fn import_world_armor(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: ArmorSetsEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    for s in env.sets.into_iter() {
        if ctx.db.armor_set().id().find(s.id.clone()).is_none() {
            ctx.db.armor_set().insert(ArmorSet { id: s.id.clone(), name: s.name.clone(), set_bonuses_csv: String::new() });
        }
        if ctx.db.armor_set_info().set_id().find(s.id.clone()).is_none() {
            ctx.db.armor_set_info().insert(ArmorSetInfo { set_id: s.id.clone(), category: "World".into(), faction_name: None, enemy_family: None, rep_required: None });
        }
        for p in s.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: s.id.clone(), slot: p.slot, name: p.name, bonus: String::new() });
        }
    }
    Ok(())
}

#[reducer]
pub fn import_faction_armor(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: FactionSetsEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    for s in env.sets.into_iter() {
        if ctx.db.armor_set().id().find(s.id.clone()).is_none() {
            ctx.db.armor_set().insert(ArmorSet { id: s.id.clone(), name: s.name.clone(), set_bonuses_csv: String::new() });
        }
        if ctx.db.armor_set_info().set_id().find(s.id.clone()).is_none() {
            ctx.db.armor_set_info().insert(ArmorSetInfo { set_id: s.id.clone(), category: "Faction".into(), faction_name: Some(s.faction.clone()), enemy_family: None, rep_required: Some(s.rep_required) });
        }
        for p in s.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: s.id.clone(), slot: p.slot, name: p.name, bonus: String::new() });
        }
    }
    Ok(())
}

#[reducer]
pub fn import_enemy_armor(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: EnemySetsEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    for s in env.sets.into_iter() {
        if ctx.db.armor_set().id().find(s.id.clone()).is_none() {
            ctx.db.armor_set().insert(ArmorSet { id: s.id.clone(), name: s.name.clone(), set_bonuses_csv: String::new() });
        }
        if ctx.db.armor_set_info().set_id().find(s.id.clone()).is_none() {
            ctx.db.armor_set_info().insert(ArmorSetInfo { set_id: s.id.clone(), category: "Enemy".into(), faction_name: None, enemy_family: Some(s.enemy_family.clone()), rep_required: None });
        }
        for p in s.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: s.id.clone(), slot: p.slot, name: p.name, bonus: String::new() });
        }
    }
    Ok(())
}

#[reducer]
pub fn import_armor_perks(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: PerksEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    // Clear all then insert
    for row in ctx.db.armor_perk_type().iter() { ctx.db.armor_perk_type().id().delete(&row.id); }
    for p in env.perks.into_iter() {
        ctx.db.armor_perk_type().insert(ArmorPerkType { id: p.id, name: p.name, stat: p.stat, min: p.min, max: p.max, rarity: p.rarity });
    }
    Ok(())
}

#[reducer]
pub fn import_armor_all(
    ctx: &ReducerContext,
    world_json: String,
    faction_json: String,
    enemy_json: String,
    perks_json: String,
) -> Result<(), String> {
    // world sets
    let world_env: ArmorSetsEnvelope = serde_json::from_str(&world_json).map_err(|e| e.to_string())?;
    for s in world_env.sets.into_iter() {
        if ctx.db.armor_set().id().find(s.id.clone()).is_none() {
            ctx.db.armor_set().insert(ArmorSet { id: s.id.clone(), name: s.name.clone(), set_bonuses_csv: String::new() });
        }
        if ctx.db.armor_set_info().set_id().find(s.id.clone()).is_none() {
            ctx.db.armor_set_info().insert(ArmorSetInfo { set_id: s.id.clone(), category: "World".into(), faction_name: None, enemy_family: None, rep_required: None });
        }
        for p in s.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: s.id.clone(), slot: p.slot, name: p.name, bonus: String::new() });
        }
    }
    // faction sets
    let fac_env: FactionSetsEnvelope = serde_json::from_str(&faction_json).map_err(|e| e.to_string())?;
    for s in fac_env.sets.into_iter() {
        if ctx.db.armor_set().id().find(s.id.clone()).is_none() {
            ctx.db.armor_set().insert(ArmorSet { id: s.id.clone(), name: s.name.clone(), set_bonuses_csv: String::new() });
        }
        if ctx.db.armor_set_info().set_id().find(s.id.clone()).is_none() {
            ctx.db.armor_set_info().insert(ArmorSetInfo { set_id: s.id.clone(), category: "Faction".into(), faction_name: Some(s.faction.clone()), enemy_family: None, rep_required: Some(s.rep_required) });
        }
        for p in s.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: s.id.clone(), slot: p.slot, name: p.name, bonus: String::new() });
        }
    }
    // enemy sets
    let enemy_env: EnemySetsEnvelope = serde_json::from_str(&enemy_json).map_err(|e| e.to_string())?;
    for s in enemy_env.sets.into_iter() {
        if ctx.db.armor_set().id().find(s.id.clone()).is_none() {
            ctx.db.armor_set().insert(ArmorSet { id: s.id.clone(), name: s.name.clone(), set_bonuses_csv: String::new() });
        }
        if ctx.db.armor_set_info().set_id().find(s.id.clone()).is_none() {
            ctx.db.armor_set_info().insert(ArmorSetInfo { set_id: s.id.clone(), category: "Enemy".into(), faction_name: None, enemy_family: Some(s.enemy_family.clone()), rep_required: None });
        }
        for p in s.pieces.into_iter() {
            ctx.db.armor_piece().insert(ArmorPiece { id: 0, set_id: s.id.clone(), slot: p.slot, name: p.name, bonus: String::new() });
        }
    }
    // perks
    let perk_env: PerksEnvelope = serde_json::from_str(&perks_json).map_err(|e| e.to_string())?;
    for row in ctx.db.armor_perk_type().iter() { ctx.db.armor_perk_type().id().delete(&row.id); }
    for p in perk_env.perks.into_iter() {
        ctx.db.armor_perk_type().insert(ArmorPerkType { id: p.id, name: p.name, stat: p.stat, min: p.min, max: p.max, rarity: p.rarity });
    }
    Ok(())
}

#[reducer]
pub fn grant_armor_set(ctx: &ReducerContext, set_id: String) -> Result<(), String> {
    // Grants all pieces in the set to the caller (no perk rolls)
    let pieces: Vec<ArmorPiece> = ctx.db.armor_piece().iter().filter(|p| p.set_id == set_id).collect();
    if pieces.is_empty() { return Err("Set not found or empty".into()); }
    for p in pieces.into_iter() {
        ctx.db.owned_armor_piece().insert(OwnedArmorPiece { id: 0, owner: ctx.sender, set_id: set_id.clone(), slot: p.slot, piece_name: p.name, bonus: String::new(), acquired_at: ctx.timestamp });
    }
    Ok(())
}

fn pick_armor_perks<'a>(ctx: &'a ReducerContext, rng: &mut StdRng, rarity: &str, count: usize) -> Vec<ArmorPerkType> {
    let pool: Vec<ArmorPerkType> = ctx.db.armor_perk_type().iter().filter(|p| {
        match rarity {
            "Mythic" => true,
            "Epic" => p.rarity != "Mythic",
            "Rare" => p.rarity == "Rare" || p.rarity == "Uncommon" || p.rarity == "Common",
            "Uncommon" => p.rarity == "Uncommon" || p.rarity == "Common",
            _ => p.rarity == "Common",
        }
    }).collect();
    let mut out: Vec<ArmorPerkType> = Vec::new();
    if pool.is_empty() { return out; }
    for _ in 0..count {
        let p = &pool[rng.gen_range(0..pool.len())];
        out.push(ArmorPerkType { id: p.id.clone(), name: p.name.clone(), stat: p.stat.clone(), min: p.min, max: p.max, rarity: p.rarity.clone() });
    }
    out
}

fn perk_count_for_rarity(r: &str) -> usize {
    match r { "Mythic" => 4, "Epic" => 3, "Rare" => 2, _ => 1 }
}

#[reducer]
pub fn award_enemy_drop(ctx: &ReducerContext, enemy_family: String, level: u32) -> Result<(), String> {
    // Prefer enemy set if exists, else random world set
    let mut enemy_sets: Vec<ArmorSetInfo> = ctx.db.armor_set_info().iter().filter(|i| i.category == "Enemy" && i.enemy_family.as_deref() == Some(enemy_family.as_str())).collect();
    let set_id = if !enemy_sets.is_empty() { enemy_sets[0].set_id.clone() } else {
        let worlds: Vec<ArmorSetInfo> = ctx.db.armor_set_info().iter().filter(|i| i.category == "World").collect();
        if worlds.is_empty() { return Err("No armor sets available".into()); }
        let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
        worlds[rng.gen_range(0..worlds.len())].set_id.clone()
    };
    // Pick a slot
    let pieces: Vec<ArmorPiece> = ctx.db.armor_piece().iter().filter(|p| p.set_id == set_id).collect();
    if pieces.is_empty() { return Err("Set has no pieces".into()); }
    let mut rng = StdRng::seed_from_u64((ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64) ^ (level as u64));
    let p = &pieces[rng.gen_range(0..pieces.len())];
    // Rarity via weights (reuse rarity_config)
    let mut rarities: Vec<RarityConfig> = ctx.db.rarity_config().iter().collect();
    if rarities.is_empty() {
        for (name, weight, order) in [("Common",60,1),("Uncommon",25,2),("Rare",10,3),("Epic",4,4),("Mythic",1,5)] {
            ctx.db.rarity_config().insert(RarityConfig { name: name.to_string(), weight, order });
        }
        rarities = ctx.db.rarity_config().iter().collect();
    }
    let total: u32 = rarities.iter().map(|r| r.weight).sum();
    let pick = rng.gen_range(0..total.max(1));
    let mut acc = 0u32; let mut rarity = "Common".to_string();
    for r in rarities.iter() { acc += r.weight; if pick < acc { rarity = r.name.clone(); break; } }
    // Insert owned piece
    ctx.db.owned_armor_piece().insert(OwnedArmorPiece { id: 0, owner: ctx.sender, set_id: set_id.clone(), slot: p.slot.clone(), piece_name: p.name.clone(), bonus: String::new(), acquired_at: ctx.timestamp });
    // Retrieve id (scan latest for owner+set+slot)
    let mut last_id: u64 = 0;
    for ow in ctx.db.owned_armor_piece().owner().filter(&ctx.sender) { if ow.set_id == set_id && ow.slot == p.slot && ow.piece_name == p.name { if ow.id > last_id { last_id = ow.id; } } }
    if last_id > 0 {
        let count = perk_count_for_rarity(&rarity);
        let perks = pick_armor_perks(ctx, &mut rng, &rarity, count);
        for perk in perks.into_iter() {
            let rolled = rng.gen_range(perk.min..=perk.max);
            ctx.db.owned_armor_piece_perk().insert(OwnedArmorPiecePerk { id: 0, owned_piece_id: last_id, perk_id: perk.id.clone(), rolled_value: rolled });
        }
    }
    Ok(())
}

#[reducer]
pub fn purchase_faction_armor(ctx: &ReducerContext, set_id: String, slot: String, price_credits: u32) -> Result<(), String> {
    // Check set info
    let info = ctx.db.armor_set_info().set_id().find(set_id.clone()).ok_or_else(|| "Set not found".to_string())?;
    if info.category != "Faction" { return Err("Not a faction set".into()); }
    // Rep check
    let req = info.rep_required.unwrap_or(0) as i64;
    let mut ok = false;
    for rep in ctx.db.player_reputation().identity().filter(&ctx.sender) { if rep.faction_name == info.faction_name.clone().unwrap_or_default() && rep.value as i64 >= req { ok = true; break; } }
    if !ok { return Err("Insufficient reputation".into()); }
    // Deduct credits
    if let Some(mut cur) = ctx.db.player_currency().identity().find(ctx.sender) { if cur.credits < price_credits { return Err("Not enough credits".into()); } cur.credits -= price_credits; cur.updated_at = ctx.timestamp; ctx.db.player_currency().identity().update(cur); } else { return Err("No currency row".into()); }
    // Find piece
    let mut chosen: Option<ArmorPiece> = None; for ap in ctx.db.armor_piece().iter() { if ap.set_id == set_id && ap.slot == slot { chosen = Some(ap); break; } }
    let p = chosen.ok_or_else(|| "Piece not found".to_string())?;
    // Grant owned piece (rarity defaults to Rare perks)
    ctx.db.owned_armor_piece().insert(OwnedArmorPiece { id: 0, owner: ctx.sender, set_id: set_id.clone(), slot: slot.clone(), piece_name: p.name.clone(), bonus: String::new(), acquired_at: ctx.timestamp });
    Ok(())
}

// Factions & Reputation
#[table(name = faction, public)]
pub struct Faction {
    #[primary_key]
    pub name: String,
    pub description: String,
}

#[table(name = player_reputation, public)]
pub struct PlayerReputation {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub identity: Identity,
    #[index(btree)]
    pub faction_name: String,
    pub value: i32,
    pub updated_at: Timestamp,
}

#[reducer]
pub fn seed_factions(ctx: &ReducerContext) -> Result<(), String> {
    let seeds = vec![
        ("Parallax Concord", "Inter-dimensional regulatory body maintaining minimal legal consistency"),
        ("Free Archipelagos", "Decentralized economic network maximizing flexibility"),
        ("Brass Arc Cartel", "Clandestine consortium for biological/technological acquisition"),
        ("Shard Tribunal", "Quasi-judicial authority with geometry-based legal interpretations"),
    ];
    for (name, desc) in seeds {
        if ctx.db.faction().name().find(name.to_string()).is_none() {
            ctx.db.faction().insert(Faction { name: name.to_string(), description: desc.to_string() });
        }
    }
    Ok(())
}

#[reducer]
pub fn adjust_reputation(ctx: &ReducerContext, faction_name: String, delta: i32) -> Result<(), String> {
    // Clamp value between -10000 and 10000
    let mut found = false;
    for mut rep in ctx.db.player_reputation().identity().filter(&ctx.sender) {
        if rep.faction_name == faction_name {
            let nv = (rep.value as i64 + delta as i64).clamp(-10000, 10000) as i32;
            rep.value = nv; rep.updated_at = ctx.timestamp; ctx.db.player_reputation().id().update(rep); found = true; break;
        }
    }
    if !found {
        ctx.db.player_reputation().insert(PlayerReputation { id: 0, identity: ctx.sender, faction_name, value: delta.clamp(-10000, 10000), updated_at: ctx.timestamp });
    }
    Ok(())
}

// Artifacts Catalog & Ownership
#[table(name = artifact_type, public)]
pub struct ArtifactType {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub rarity: String,
    pub description: String,
}

#[table(name = owned_artifact, public)]
pub struct OwnedArtifact {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub identity: Identity,
    pub artifact_id: String,
    pub source: String,
    pub acquired_at: Timestamp,
}

#[reducer]
pub fn seed_artifacts(ctx: &ReducerContext) -> Result<(), String> {
    let items = vec![
        ("biolumen_crystal", "Biolumen Crystal", "Epic", "Living crystal that projects probable futures"),
        ("quantum_license", "Quantum License", "Rare", "Inter-reality legal documentation enabling traversal"),
        ("adaptive_module", "Adaptive Implant Module", "Uncommon", "Bio-augment that adapts to operator physiology"),
    ];
    for (id, name, rarity, desc) in items {
        if ctx.db.artifact_type().id().find(id.to_string()).is_none() {
            ctx.db.artifact_type().insert(ArtifactType { id: id.to_string(), name: name.to_string(), rarity: rarity.to_string(), description: desc.to_string() });
        }
    }
    Ok(())
}

#[reducer]
pub fn buy_artifact(ctx: &ReducerContext, artifact_id: String, price_credits: u32) -> Result<(), String> {
    let art = ctx.db.artifact_type().id().find(artifact_id.clone()).ok_or_else(|| "Unknown artifact".to_string())?;
    // Deduct credits
    if let Some(mut cur) = ctx.db.player_currency().identity().find(ctx.sender) {
        if cur.credits < price_credits { return Err("Not enough credits".into()); }
        cur.credits -= price_credits; cur.updated_at = ctx.timestamp; ctx.db.player_currency().identity().update(cur);
    } else { return Err("No currency row".into()); }
    // Grant artifact
    ctx.db.owned_artifact().insert(OwnedArtifact { id: 0, identity: ctx.sender, artifact_id, source: "faction_store".into(), acquired_at: ctx.timestamp });
    Ok(())
}

#[table(name = player_currency, public)]
pub struct PlayerCurrency {
    #[primary_key]
    pub identity: Identity,
    pub credits: u32,
    pub mythic_shards: u32,
    pub updated_at: Timestamp,
}

/// mode: "all_missing" (default) or "random". If no pieces left, awards shards.
#[reducer]
pub fn award_boss_loot(ctx: &ReducerContext, boss_id: String, mode: Option<String>) -> Result<(), String> {
    // Find boss and its armor set
    let boss = ctx.db.boss_type().id().find(boss_id.clone()).ok_or_else(|| "Unknown boss".to_string())?;
    let give_mode = mode.unwrap_or_else(|| "all_missing".to_string());
    // Collect missing pieces
    let mut missing: Vec<ArmorPiece> = Vec::new();
    'outer: for p in ctx.db.armor_piece().set_id().filter(&boss.armor_set_id) {
        for owned in ctx.db.owned_armor_piece().owner().filter(&ctx.sender) {
            if owned.set_id == boss.armor_set_id && owned.slot == p.slot { continue 'outer; }
        }
        missing.push(p);
    }
    if missing.is_empty() {
        // Award shards
        let add = 10u32;
        if let Some(mut row) = ctx.db.player_currency().identity().find(ctx.sender) {
            row.mythic_shards = row.mythic_shards.saturating_add(add);
            row.updated_at = ctx.timestamp;
            ctx.db.player_currency().identity().update(row);
        } else {
            ctx.db.player_currency().insert(PlayerCurrency { identity: ctx.sender, credits: 0, mythic_shards: add, updated_at: ctx.timestamp });
        }
        return Ok(());
    }
    if give_mode == "random" {
        let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
        let idx = (rng.gen::<usize>()) % missing.len();
        let p = missing.remove(idx);
        ctx.db.owned_armor_piece().insert(OwnedArmorPiece { id: 0, owner: ctx.sender, set_id: boss.armor_set_id.clone(), slot: p.slot, piece_name: p.name, bonus: p.bonus, acquired_at: ctx.timestamp });
    } else {
        for p in missing.into_iter() {
            ctx.db.owned_armor_piece().insert(OwnedArmorPiece { id: 0, owner: ctx.sender, set_id: boss.armor_set_id.clone(), slot: p.slot, piece_name: p.name, bonus: p.bonus, acquired_at: ctx.timestamp });
        }
    }
    Ok(())
}

// New Game Plus and Difficulty
#[table(name = player_ng_plus, public)]
pub struct PlayerNGPlus {
    #[primary_key]
    pub identity: Identity,
    pub level: u8, // 0..5
    pub updated_at: Timestamp,
}

#[table(name = difficulty_state, public)]
pub struct DifficultyState {
    #[primary_key]
    pub id: u8,
    pub level: u8, // 1..5
    pub updated_at: Timestamp,
}

// Activities (POI/Event) cooldown + progress
#[table(name = activity_cooldown, public)]
pub struct ActivityCooldown {
    #[primary_key]
    pub activity_key: String,
    pub biome: String,
    pub available_at: Timestamp,
}

#[table(name = player_activity_progress, public)]
pub struct PlayerActivityProgress {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub identity: Identity,
    #[index(btree)]
    pub activity_key: String,
    pub completed: u32,
    pub last_completed_at: Timestamp,
}

#[reducer]
pub fn complete_activity(ctx: &ReducerContext, activity_key: String, biome: String, cooldown_seconds: u32, reward_credits: u32) -> Result<(), String> {
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    // Check cooldown
    if let Some(cd) = ctx.db.activity_cooldown().activity_key().find(activity_key.clone()) {
        if cd.available_at.to_micros_since_unix_epoch() > now { return Err("Activity cooling down".into()); }
    }
    // Set new cooldown
    let new_avail = spacetimedb::Timestamp::from_micros_since_unix_epoch(now + (cooldown_seconds as i64 * 1_000_000));
    if ctx.db.activity_cooldown().activity_key().find(activity_key.clone()).is_some() {
        ctx.db.activity_cooldown().activity_key().update(ActivityCooldown { activity_key: activity_key.clone(), biome: biome.clone(), available_at: new_avail });
    } else {
        ctx.db.activity_cooldown().insert(ActivityCooldown { activity_key: activity_key.clone(), biome: biome.clone(), available_at: new_avail });
    }
    // Progress
    let mut updated = false;
    for mut row in ctx.db.player_activity_progress().identity().filter(&ctx.sender) {
        if row.activity_key == activity_key { row.completed = row.completed.saturating_add(1); row.last_completed_at = ctx.timestamp; ctx.db.player_activity_progress().id().update(row); updated = true; break; }
    }
    if !updated {
        ctx.db.player_activity_progress().insert(PlayerActivityProgress { id: 0, identity: ctx.sender, activity_key: activity_key.clone(), completed: 1, last_completed_at: ctx.timestamp });
    }
    // Reward credits
    if let Some(mut cur) = ctx.db.player_currency().identity().find(ctx.sender) {
        cur.credits = cur.credits.saturating_add(reward_credits); cur.updated_at = ctx.timestamp; ctx.db.player_currency().identity().update(cur);
    } else {
        ctx.db.player_currency().insert(PlayerCurrency { identity: ctx.sender, credits: reward_credits, mythic_shards: 0, updated_at: ctx.timestamp });
    }
    // Small chance for a Common/Uncommon artifact
    let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
    let roll: u32 = rng.gen_range(0..100);
    if roll < 10 {
        let commons: Vec<ArtifactType> = ctx.db.artifact_type().iter().filter(|a| a.rarity == "Common" || a.rarity == "Uncommon").collect();
        if !commons.is_empty() {
            let art_id = commons[rng.gen_range(0..commons.len())].id.clone();
            ctx.db.owned_artifact().insert(OwnedArtifact { id: 0, identity: ctx.sender, artifact_id: art_id, source: "activity".into(), acquired_at: ctx.timestamp });
        }
    }
    Ok(())
}

// Dynamic biome events
#[table(name = biome_event_catalog, public)]
pub struct BiomeEventCatalog {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub biome: String,
    pub name: String,
    pub weight: u32,
    pub cooldown_seconds: u32,
}

#[table(name = biome_event_cooldown, public)]
pub struct BiomeEventCooldown {
    #[primary_key]
    pub biome: String,
    pub available_at: Timestamp,
}

#[table(name = active_biome_event, public)]
pub struct ActiveBiomeEvent {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub biome: String,
    pub name: String,
    pub region_id: u8,
    pub x: f32,
    pub z: f32,
    pub started_at: Timestamp,
    pub expires_at: Timestamp,
}

// NPCs — Lore + Life Simulation
#[table(name = npc, public)]
pub struct Npc {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    pub name: String,
    pub faction: String,
    pub role: String,
    pub personality: String,
    pub home_region: u8,
    pub bio: String,
    pub created_at: Timestamp,
}

#[table(name = npc_state, public)]
pub struct NpcState {
    #[primary_key]
    pub npc_id: u64,
    #[index(btree)]
    pub region_id: u8,
    pub x: f32,
    pub y: f32,
    pub z: f32,
    pub mood: String,
    pub activity: String,
    pub updated_at: Timestamp,
}

#[table(name = npc_memory, public)]
pub struct NpcMemory {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub npc_id: u64,
    pub at: Timestamp,
    pub kind: String,
    pub description: String,
    pub importance: u8,
}

#[table(name = npc_schedule, public)]
pub struct NpcSchedule {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub npc_id: u64,
    pub hour: u8,
    pub activity: String,
    pub region_id: u8,
    pub x: f32,
    pub y: f32,
    pub z: f32,
}

#[derive(Deserialize)]
struct NpcScheduleIn { hour: u8, activity: String, region_id: u8, x: f32, y: f32, z: f32 }
#[derive(Deserialize)]
struct NpcIn { name: String, faction: String, role: String, personality: String, home_region: u8, bio: String, schedule: Vec<NpcScheduleIn> }
#[derive(Deserialize)]
struct NpcEnvelope { npcs: Vec<NpcIn> }

#[reducer]
pub fn import_npcs(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: NpcEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    // Clear existing data (idempotent import)
    for row in ctx.db.npc_memory().iter() { ctx.db.npc_memory().id().delete(&row.id); }
    for row in ctx.db.npc_schedule().iter() { ctx.db.npc_schedule().id().delete(&row.id); }
    for row in ctx.db.npc_state().iter() { ctx.db.npc_state().npc_id().delete(&row.npc_id); }
    for row in ctx.db.npc().iter() { ctx.db.npc().id().delete(&row.id); }
    for n in env.npcs.into_iter() {
        let npc_id = {
            ctx.db.npc().insert(Npc {
                id: 0,
                name: n.name.clone(), faction: n.faction.clone(), role: n.role.clone(), personality: n.personality.clone(), home_region: n.home_region, bio: n.bio.clone(), created_at: ctx.timestamp,
            });
            // fetch latest inserted (scan by name/personality; simplified)
            let mut last: u64 = 0; for row in ctx.db.npc().iter() { if row.name == n.name && row.faction == n.faction { if row.id > last { last = row.id; } } } last
        };
        // Seed initial state at home region
        ctx.db.npc_state().insert(NpcState { npc_id, region_id: n.home_region, x: 0.0, y: 0.0, z: 0.0, mood: "Idle".into(), activity: "Idle".into(), updated_at: ctx.timestamp });
        // Insert schedule
        for s in n.schedule.into_iter() {
            ctx.db.npc_schedule().insert(NpcSchedule { id: 0, npc_id, hour: s.hour, activity: s.activity, region_id: s.region_id, x: s.x, y: s.y, z: s.z });
        }
        // First memory
        ctx.db.npc_memory().insert(NpcMemory { id: 0, npc_id, at: ctx.timestamp, kind: "creation".into(), description: format!("Arrived from {} with role {}.", n.faction, n.role), importance: 5 });
    }
    Ok(())
}

#[reducer]
pub fn tick_npcs(ctx: &ReducerContext, region_id: u8, hour_24: u8) -> Result<(), String> {
    // For each NPC in region, align to schedule and record memory
    let hour = hour_24.min(23);
    let mut updated = 0u32;
    for mut st in ctx.db.npc_state().region_id().filter(&region_id) {
        let mut rng = StdRng::seed_from_u64((ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64) ^ st.npc_id);
        // Derive deviate probability from personality string traits if available: higher openness -> more deviation; higher conscientiousness -> less
        let (mut o, mut c, mut e) = (0.5f32, 0.5f32, 0.5f32);
        if let Some(npc) = ctx.db.npc().id().find(st.npc_id) {
            let s = npc.personality;
            o = parse_trait(&s, 'O').unwrap_or(0.5);
            c = parse_trait(&s, 'C').unwrap_or(0.5);
            e = parse_trait(&s, 'E').unwrap_or(0.5);
        }
        // Find schedule at this hour
        let mut target: Option<NpcSchedule> = None;
        for s in ctx.db.npc_schedule().npc_id().filter(&st.npc_id) { if s.hour == hour { target = Some(s); break; } }
        // Deviate chance: base 10% + openness*10% - conscientiousness*10%
        let mut deviate_pct: i32 = 10 + ((o * 10.0) as i32) - ((c * 10.0) as i32);
        if deviate_pct < 3 { deviate_pct = 3; }
        if deviate_pct > 30 { deviate_pct = 30; }
        let deviate = rng.gen_range(0..100) < deviate_pct as u32;
        if deviate {
            // Sometimes socialize if extraversion is high, otherwise wander
            if e > 0.6 && rng.gen_range(0..100) < 50 {
                let dx = rng.gen_range(-1.5..=1.5); let dz = rng.gen_range(-1.5..=1.5);
                st.x += dx; st.z += dz; st.activity = "Social".into(); st.updated_at = ctx.timestamp;
                ctx.db.npc_state().npc_id().update(st);
                ctx.db.npc_memory().insert(NpcMemory { id: 0, npc_id: st.npc_id, at: ctx.timestamp, kind: "social".into(), description: "Chatted with passersby.".into(), importance: 2 });
            } else {
                let dx = rng.gen_range(-2.0..=2.0); let dz = rng.gen_range(-2.0..=2.0);
                st.x += dx; st.z += dz; st.activity = "Wander".into(); st.updated_at = ctx.timestamp;
                ctx.db.npc_state().npc_id().update(st);
                ctx.db.npc_memory().insert(NpcMemory { id: 0, npc_id: st.npc_id, at: ctx.timestamp, kind: "wander".into(), description: "Took an unplanned walk.".into(), importance: 2 });
            }
        }
        else if let Some(s) = target {
            let dx = s.x - st.x; let dy = s.y - st.y; let dz = s.z - st.z;
            let dist = (dx*dx + dy*dy + dz*dz).sqrt();
            let step = 2.0; // m per tick call; simplified
            if dist > 0.1 {
                let inv = step / dist.max(step);
                st.x += dx * inv; st.y += dy * inv; st.z += dz * inv;
            }
            let prev_activity = st.activity.clone();
            st.activity = s.activity.clone(); st.region_id = s.region_id; st.updated_at = ctx.timestamp;
            ctx.db.npc_state().npc_id().update(st);
            updated += 1;
            if prev_activity != s.activity {
                ctx.db.npc_memory().insert(NpcMemory { id: 0, npc_id: s.npc_id, at: ctx.timestamp, kind: "activity".into(), description: format!("Started {} at hour {}.", s.activity, hour), importance: 3 });
            }
        } else {
            // no schedule — idle wander
            st.x += 0.5; st.updated_at = ctx.timestamp; ctx.db.npc_state().npc_id().update(st);
        }
    }
    let _ = updated; // unused var
    Ok(())
}

fn parse_trait(s: &str, key: char) -> Option<f32> {
    // Finds tokens like "O:0.74" in the string
    let up = s.to_uppercase();
    let k = format!("{}:", key.to_ascii_uppercase());
    if let Some(idx) = up.find(&k) {
        let rest = &up[idx + 2..];
        // take up to next whitespace or semicolon
        let mut end = rest.len();
        for (i, ch) in rest.char_indices() {
            if ch.is_whitespace() || ch == ';' || ch == ',' { end = i; break; }
        }
        let token = &rest[..end];
        if let Ok(v) = token.parse::<f32>() { return Some(v.clamp(0.0, 1.0)); }
    }
    None
}

#[reducer]
pub fn npc_remember(ctx: &ReducerContext, npc_id: u64, kind: String, description: String, importance: u8) -> Result<(), String> {
    // Append a memory entry for an NPC
    if ctx.db.npc().id().find(npc_id).is_none() { return Err("unknown npc".into()); }
    let imp = importance.min(10);
    ctx.db.npc_memory().insert(NpcMemory { id: 0, npc_id, at: ctx.timestamp, kind, description, importance: imp });
    Ok(())
}

// Quests — Procedural + Chainable
#[table(name = quest_archetype, public)]
pub struct QuestArchetype {
    #[primary_key]
    pub id: String,
    pub name: String,
    pub description: String,
    pub category: String,
    pub event_type: String,      // e.g., "kill_enemy", "active_event", "deliver_item", "talk_to_npc"
    pub event_param: String,     // e.g., family name, biome, npc name, item id
    pub required_count: u32,
    pub time_limit_s: Option<u32>,
    pub chain_next_id: Option<String>,
    pub reward_credits: u32,
    pub reward_item: Option<String>,
    pub reward_item_qty: Option<u32>,
}

#[table(name = quest_instance, public)]
pub struct QuestInstance {
    #[auto_inc]
    #[primary_key]
    pub id: u64,
    #[index(btree)]
    pub archetype_id: String,
    #[index(btree)]
    pub npc_id: u64,            // quest giver
    #[index(btree)]
    pub player: Option<Identity>,
    pub state: String,          // "available", "active", "completed", "failed"
    pub created_at: Timestamp,
    pub expires_at: Option<Timestamp>,
    pub current_count: u32,
}

#[derive(Deserialize)]
struct QuestIn { id: String, name: String, description: String, category: String, event_type: String, event_param: String, required_count: u32, time_limit_s: Option<u32>, chain_next_id: Option<String>, reward_credits: u32, reward_item: Option<String>, reward_item_qty: Option<u32> }
#[derive(Deserialize)]
struct QuestsEnvelope { quests: Vec<QuestIn> }

#[reducer]
pub fn import_quests(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: QuestsEnvelope = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    // Clear existing archetypes
    for row in ctx.db.quest_archetype().iter() { ctx.db.quest_archetype().id().delete(&row.id); }
    for q in env.quests.into_iter() {
        ctx.db.quest_archetype().insert(QuestArchetype {
            id: q.id, name: q.name, description: q.description, category: q.category, event_type: q.event_type,
            event_param: q.event_param, required_count: q.required_count, time_limit_s: q.time_limit_s, chain_next_id: q.chain_next_id, reward_credits: q.reward_credits,
            reward_item: q.reward_item, reward_item_qty: q.reward_item_qty,
        });
    }
    Ok(())
}

#[reducer]
pub fn seed_npc_quests(ctx: &ReducerContext, total: u32) -> Result<(), String> {
    // Clear existing quest instances
    for row in ctx.db.quest_instance().iter() { ctx.db.quest_instance().id().delete(&row.id); }
    let mut npcs: Vec<Npc> = ctx.db.npc().iter().collect();
    let arcs: Vec<QuestArchetype> = ctx.db.quest_archetype().iter().collect();
    if arcs.is_empty() || npcs.is_empty() { return Err("No quest archetypes or NPCs available".into()); }
    let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
    let mut created = 0u32;
    let mut i = 0usize;
    while created < total {
        let npc = &npcs[rng.gen_range(0..npcs.len())];
        let arc = &arcs[i % arcs.len()]; i += 1;
        let expires = arc.time_limit_s.map(|s| spacetimedb::Timestamp::from_micros_since_unix_epoch(ctx.timestamp.to_micros_since_unix_epoch() + (s as i64)*1_000_000));
        ctx.db.quest_instance().insert(QuestInstance { id: 0, archetype_id: arc.id.clone(), npc_id: npc.id, player: None, state: "available".into(), created_at: ctx.timestamp, expires_at: expires, current_count: 0 });
        created += 1;
    }
    Ok(())
}

#[reducer]
pub fn assign_quest_to_self(ctx: &ReducerContext, quest_instance_id: u64) -> Result<(), String> {
    if let Some(mut qi) = ctx.db.quest_instance().id().find(quest_instance_id) {
        if qi.state != "available" { return Err("Quest not available".into()); }
        qi.player = Some(ctx.sender); qi.state = "active".into(); qi.current_count = 0; ctx.db.quest_instance().id().update(qi); Ok(())
    } else { Err("Quest not found".into()) }
}

#[reducer]
pub fn complete_quest(ctx: &ReducerContext, quest_instance_id: u64) -> Result<(), String> {
    if let Some(mut qi) = ctx.db.quest_instance().id().find(quest_instance_id) {
        if qi.player != Some(ctx.sender) { return Err("Not your quest".into()); }
        if qi.state != "active" { return Err("Quest not active".into()); }
        // Check requirement
        let arc = ctx.db.quest_archetype().id().find(qi.archetype_id.clone()).ok_or_else(|| "Archetype missing".to_string())?;
        if qi.current_count < arc.required_count { return Err("Requirements not met".into()); }
        qi.state = "completed".into(); ctx.db.quest_instance().id().update(qi);
        // Reward credits
        if let Some(mut cur) = ctx.db.player_currency().identity().find(ctx.sender) { cur.credits = cur.credits.saturating_add(arc.reward_credits); cur.updated_at = ctx.timestamp; ctx.db.player_currency().identity().update(cur); }
        else { ctx.db.player_currency().insert(PlayerCurrency { identity: ctx.sender, credits: arc.reward_credits, mythic_shards: 0, updated_at: ctx.timestamp }); }
        // Optional item reward
        if let Some(item_name) = arc.reward_item.clone() {
            let qty = arc.reward_item_qty.unwrap_or(1).max(1);
            ctx.db.item().insert(Item { id: 0, owner: ctx.sender, name: item_name, qty });
        }
        // Spawn next in chain (available at same NPC)
        if let Some(next) = arc.chain_next_id.clone() {
            if let Some(next_arc) = ctx.db.quest_archetype().id().find(next) {
                let expires = next_arc.time_limit_s.map(|s| spacetimedb::Timestamp::from_micros_since_unix_epoch(ctx.timestamp.to_micros_since_unix_epoch() + (s as i64)*1_000_000));
                ctx.db.quest_instance().insert(QuestInstance { id: 0, archetype_id: next_arc.id.clone(), npc_id: qi.npc_id, player: None, state: "available".into(), created_at: ctx.timestamp, expires_at: expires, current_count: 0 });
            }
        }
        Ok(())
    } else { Err("Quest not found".into()) }
}

#[reducer]
pub fn update_quest_on_event(ctx: &ReducerContext, event_type: String, param: String, count: u32) -> Result<(), String> {
    // Update all active quests for this player that match
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    for mut qi in ctx.db.quest_instance().iter() {
        if qi.player != Some(ctx.sender) || qi.state != "active" { continue; }
        if let Some(exp) = qi.expires_at { if exp.to_micros_since_unix_epoch() < now { qi.state = "failed".into(); ctx.db.quest_instance().id().update(qi); continue; } }
        let arc = match ctx.db.quest_archetype().id().find(qi.archetype_id.clone()) { Some(a) => a, None => continue };
        if arc.event_type == event_type && (arc.event_param.is_empty() || arc.event_param == param) {
            let before = qi.current_count;
            qi.current_count = qi.current_count.saturating_add(count).min(arc.required_count);
            ctx.db.quest_instance().id().update(qi);
            // Auto-complete if reached
            if before < arc.required_count && qi.current_count >= arc.required_count {
                // Auto complete rewards
                let _ = complete_quest(ctx, qi.id);
            }
        }
    }
    Ok(())
}

#[reducer]
pub fn tick_quests(ctx: &ReducerContext) -> Result<(), String> {
    // Expire overdue quests
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    for mut qi in ctx.db.quest_instance().iter() {
        if qi.state == "active" || qi.state == "available" {
            if let Some(exp) = qi.expires_at { if exp.to_micros_since_unix_epoch() < now { qi.state = "failed".into(); ctx.db.quest_instance().id().update(qi); } }
        }
    }
    Ok(())
}

#[derive(Deserialize)]
struct ActivitiesEnvelopeIn { events: Vec<ActivityEventIn> }
#[derive(Deserialize)]
struct ActivityEventIn { biome: String, name: String, weight: u32, cooldown_chunks: u32 }

#[reducer]
pub fn import_biome_events(ctx: &ReducerContext, json: String) -> Result<(), String> {
    let env: ActivitiesEnvelopeIn = serde_json::from_str(&json).map_err(|e| e.to_string())?;
    // Clear existing
    for row in ctx.db.biome_event_catalog().iter() { ctx.db.biome_event_catalog().id().delete(&row.id); }
    for e in env.events.into_iter() {
        let cooldown_secs = (e.cooldown_chunks as u32) * 60; // interpret chunks as minutes by default
        ctx.db.biome_event_catalog().insert(BiomeEventCatalog { id: 0, biome: e.biome, name: e.name, weight: e.weight, cooldown_seconds: cooldown_secs });
    }
    Ok(())
}

#[reducer]
pub fn tick_biome_events(ctx: &ReducerContext, region_id: u8) -> Result<(), String> {
    // Find region and biome
    let region = ctx.db.world_region().id().find(region_id).ok_or_else(|| "Unknown region".to_string())?;
    let biome = region.name.clone();
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    // Cooldown check
    if let Some(cd) = ctx.db.biome_event_cooldown().biome().find(biome.clone()) {
        if cd.available_at.to_micros_since_unix_epoch() > now { return Ok(()); }
    }
    // Existing active event?
    for a in ctx.db.active_biome_event().biome().filter(&biome) { if a.region_id == region_id && a.expires_at.to_micros_since_unix_epoch() > now { return Ok(()); } }
    // Pick event
    let mut list: Vec<BiomeEventCatalog> = ctx.db.biome_event_catalog().biome().filter(&biome).collect();
    if list.is_empty() { return Ok(()); }
    let mut rng = StdRng::seed_from_u64(ctx.timestamp.to_micros_since_unix_epoch() as i64 as u64);
    let total: u32 = list.iter().map(|e| e.weight).sum();
    if total == 0 { return Ok(()); }
    let pick = rng.gen_range(0..total);
    let mut acc = 0u32; let mut chosen: Option<BiomeEventCatalog> = None;
    for e in list.into_iter() { acc += e.weight; if pick < acc { chosen = Some(e); break; } }
    let Some(ev) = chosen else { return Ok(()); };
    // Place near center
    let x = rng.gen_range(0.0..region.size_meters as f32);
    let z = rng.gen_range(0.0..region.size_meters as f32);
    let duration_us = (ev.cooldown_seconds as i64) * 1_000_000;
    let expires = spacetimedb::Timestamp::from_micros_since_unix_epoch(now + duration_us);
    ctx.db.active_biome_event().insert(ActiveBiomeEvent { id: 0, biome: biome.clone(), name: ev.name, region_id, x, z, started_at: ctx.timestamp, expires_at: expires });
    // Set cooldown
    let next_avail = spacetimedb::Timestamp::from_micros_since_unix_epoch(now + duration_us);
    if ctx.db.biome_event_cooldown().biome().find(biome.clone()).is_some() {
        ctx.db.biome_event_cooldown().biome().update(BiomeEventCooldown { biome: biome.clone(), available_at: next_avail });
    } else {
        ctx.db.biome_event_cooldown().insert(BiomeEventCooldown { biome: biome.clone(), available_at: next_avail });
    }
    Ok(())
}

#[reducer]
pub fn clear_expired_biome_events(ctx: &ReducerContext) -> Result<(), String> {
    let now = ctx.timestamp.to_micros_since_unix_epoch();
    for a in ctx.db.active_biome_event().iter() {
        if a.expires_at.to_micros_since_unix_epoch() <= now { ctx.db.active_biome_event().id().delete(&a.id); }
    }
    Ok(())
}
#[reducer]
pub fn set_player_ng_plus(ctx: &ReducerContext, level: u8) -> Result<(), String> {
    let clamped = level.min(5);
    if let Some(mut row) = ctx.db.player_ng_plus().identity().find(ctx.sender) {
        row.level = clamped; row.updated_at = ctx.timestamp; ctx.db.player_ng_plus().identity().update(row);
    } else {
        ctx.db.player_ng_plus().insert(PlayerNGPlus { identity: ctx.sender, level: clamped, updated_at: ctx.timestamp });
    }
    Ok(())
}

#[reducer]
pub fn set_global_difficulty(ctx: &ReducerContext, level: u8) -> Result<(), String> {
    let clamped = level.clamp(1, 5);
    if let Some(mut state) = ctx.db.difficulty_state().id().find(1) {
        state.level = clamped; state.updated_at = ctx.timestamp; ctx.db.difficulty_state().id().update(state);
    } else {
        ctx.db.difficulty_state().insert(DifficultyState { id: 1, level: clamped, updated_at: ctx.timestamp });
    }
    Ok(())
}


// Gossip propagation: NPCs in proximity share recent memories
#[reducer]
pub fn tick_npc_gossip(ctx: &ReducerContext, region_id: u8, max_spreads: u32) -> Result<(), String> {
    // Collect states in region
    let mut states: Vec<NpcState> = ctx.db.npc_state().region_id().filter(&region_id).collect();
    let mut spreads = 0u32;
    // For each pair, if close, share one recent memory
    let n = states.len();
    for i in 0..n {
        if spreads >= max_spreads { break; }
        let a = &states[i];
        // Find most recent memory of A
        let mut latest_a: Option<NpcMemory> = None;
        for m in ctx.db.npc_memory().npc_id().filter(&a.npc_id) {
            if let Some(cur) = &latest_a { if m.at.to_micros_since_unix_epoch() > cur.at.to_micros_since_unix_epoch() { latest_a = Some(m); } }
            else { latest_a = Some(m); }
        }
        if latest_a.is_none() { continue; }
        let la = latest_a.unwrap();
        for j in (i+1)..n {
            if spreads >= max_spreads { break; }
            let b = &states[j];
            // Distance check
            let dx = a.x - b.x; let dy = a.y - b.y; let dz = a.z - b.z;
            let dist2 = dx*dx + dy*dy + dz*dz;
            if dist2 > 64.0 { continue; } // >8m apart
            // Insert mirrored memory for B (kind=gossip)
            let desc = format!("Heard about {}: {}", la.kind, la.description);
            ctx.db.npc_memory().insert(NpcMemory { id: 0, npc_id: b.npc_id, at: ctx.timestamp, kind: "gossip".into(), description: desc, importance: (la.importance/2).max(1) });
            spreads += 1;
        }
    }
    Ok(())
}
