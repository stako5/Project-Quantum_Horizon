Codex UI (Classes/Abilities/Perks)

Components
- CodexUI.cs: attach to a GameObject in a scene with:
  - `Dropdown` reference (for classes)
  - Two `RectTransform` containers (abilities/perks list parents)
  - A disabled `Text` component as item template (will be cloned per line)
- Data source: by default `useLocalJson` reads generated JSON from the repo (Editor/dev). Switch to `SpacetimeCodexDataSource` once the SpacetimeDB C# SDK is integrated.

Runtime with Local JSON
- Generate content: see `design/classes/USAGE.md` (writes JSON to `SpacetimeDB/design/classes/data/<Class>/full`)
- In Unity, add CodexUI and enable `useLocalJson`. Press Play to browse.

Integrating SpacetimeDB SDK
- Subscribe to `class_catalog` (public) for classes list.
- Subscribe to `ability` filtered by `class_name = <selected>`.
- Subscribe to `perk` filtered by `class_name = <selected>`.
- Replace `SpacetimeCodexDataSource` methods to return the latest rows to the UI.

Active Events Panel
- `ActiveEventsUI.cs`: attach to a GameObject with:
  - `RectTransform` list container and disabled `Text` item template
  - Optional `refreshInterval` (seconds)
- Data: uses `BindingsBridge.GetActiveBiomeEvents()` (requires `SPACETIMEDB_SDK` and generated bindings). Each row renders `Name — time-remaining`.
