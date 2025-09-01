SpacetimeDB Bindings Bridge — Example Wiring

After running `spacetime generate` for your module, wire the generated tables to `BindingsBridge` so Codex UI, Bestiary, and World Boss systems can query live data.

Example (pseudocode; adjust to your generated API):

```csharp
using MMORPG.Client.Net;
// Call once after connecting
void WireBindings(DbConnection conn)
{
    BindingsBridge.Provide(
        classes: () => conn.Db.ClassCatalog.Iter(),
        abilities: (cls) => conn.Db.Ability.ClassName.Filter(cls),
        perks: (cls) => conn.Db.Perk.ClassName.Filter(cls),
        enemies: () => conn.Db.EnemyType.Iter(),
        wbTypes: () => conn.Db.WorldBossType.Iter(),
        wbSpawns: () => conn.Db.WorldBossSpawn.Iter()
    );
}
```

If your generator produces a static namespace (e.g., `Generated.Tables.*`), adapt accordingly:

```csharp
BindingsBridge.Provide(
  () => Generated.Tables.ClassCatalog.Iter(),
  (cls) => Generated.Tables.Ability.ClassName.Filter(cls),
  (cls) => Generated.Tables.Perk.ClassName.Filter(cls),
  () => Generated.Tables.EnemyType.Iter(),
  () => Generated.Tables.WorldBossType.Iter(),
  () => Generated.Tables.WorldBossSpawn.Iter()
);
```

Ensure `SPACETIMEDB_SDK` is defined in Scripting Define Symbols.

