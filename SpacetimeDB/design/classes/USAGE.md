Class Content Workflow

Generate
- Python 3.8+ required.
- Example (Time Mage):
- `python SpacetimeDB/design/classes/generate_content.py --class "Time Mage" --branches Acceleration Reversion Fracture --resource "Chrono Charge" --out SpacetimeDB/design/classes/data/TimeMage/full`

Import to Server (via SpacetimeDB CLI)
- Start local DB: `spacetime start`
- Build module: `cd SpacetimeDB/server && spacetime build`
- Clear existing: `spacetime reducer call clear_class_data -- "Time Mage"`
- Import abilities (pass file contents as string):
  - `spacetime reducer call import_abilities -- "Time Mage" "$(cat ../design/classes/data/TimeMage/full/abilities.json)"`
- Import perks:
  - `spacetime reducer call import_perks -- "Time Mage" "$(cat ../design/classes/data/TimeMage/full/perks.json)"`

Roll Loot
- `spacetime reducer call roll_weapon -- "Time Mage" "Chronometer Staff" 50 4`

Notes
- Repeat for each class directory under `design/classes/data/*/full`.
- JSON schema reference: `design/classes/schema.json`.
