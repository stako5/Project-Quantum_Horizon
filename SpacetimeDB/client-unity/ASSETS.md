Space‑Punk Assets (CC0/permissive only)

Licensing Policy
- Use CC0/public‑domain or clearly permissive licenses. Keep copies of licenses with the project.
- Avoid branded IP and trademarked looks; do not use Minecraft or other proprietary assets.

Recommended CC0/Permissive Sources
- Kenney.nl: Sci‑Fi kits, VFX, UI icons (CC0).
- Quaternius.dev: Sci‑Fi characters, weapons, ships (very permissive; credit optional).
- Poly Haven: CC0 PBR textures, HDRIs (great for metal panels, concrete, cables).
- ambientCG: CC0 PBR surfaces/decals (paint, dirt, grime, stickers).
- OpenGameArt.org: filter by license = CC0.
- Sketchfab: filter by license = CC0 (double‑check per model).

Space‑Punk Art Direction
- Palette: charcoal #0e0e11, deep violet #1b0033, cyan #00eaff, magenta #ff2bdc, neon green #39ff14, accent orange #ff9d00.
- Materials: high metalness (0.8–1.0), mid roughness (0.3–0.6), heavy edge wear; bright emissive trims and signage.
- Shapes: chunky industrial panels, exposed cables, kit‑bashed greebles; holographic displays and neon decals.
- Lighting: URP Bloom on, subtle Chromatic Aberration, Vignette; bluish ambient with magenta rim lights.

Unity Import + Setup (URP, Mobile‑friendly)
- Textures: set Platform Overrides (ASTC/ETC2), 1k–2k; generate normal maps where available.
- Materials: URP/Lit with Emission (intensity 2–8). Use Shader Graph for pulsing emission, Fresnel edge glow, and hologram scanlines.
- HDRIs: use Poly Haven studio/city night HDRIs for reflections; clamp exposure in volumes.
- Performance: enable GPU Instancing, merge static meshes, target <150 draw calls on mid devices; bake lightmaps for static props.

Fast Asset Set To Start
- Environment: Kenney sci‑fi modular kit pieces + ambientCG metal plates/paint chips; Poly Haven cables/hoses.
- Props: Quaternius sci‑fi crates, terminals, weapons; add neon emissive stripes/decals.
- Characters: Quaternius sci‑fi/space crews; recolor to palette and add emissive trims.
- VFX/UI: Kenney VFX and UI packs; recolor to cyan/magenta/green.

Workflow To “Space‑Punkify” Any Generic Asset
- Retexture with CC0 metals/concrete; paint emission masks on edges, logos, and panels.
- Add decals (warnings, barcodes, neon glyphs) via projector/mesh decals.
- Apply pulsing emission via Shader Graph; optional scanline/dither for holograms.
- Color‑grade with Post‑Processing (LUT) to push cyan/magenta hues.

Legal Notes
- Store licenses in `Assets/_Licenses/` and keep source links in a README.
- If an asset isn’t clearly CC0/permissive, don’t include it.
