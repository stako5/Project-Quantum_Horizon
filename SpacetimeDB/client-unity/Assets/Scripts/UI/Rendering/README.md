Ray Tracing (HDRP/DXR)

Summary
- Unity ray tracing requires HDRP, a Render Pipeline Asset with DXR enabled, and a GPU/OS that supports DXR.
- This repo includes a lightweight `RayTracingConfigurator` script. It logs guidance and is guarded by a `HDRP_PRESENT` define.

Steps
1) Switch project to HDRP (Project Settings → Graphics → Scriptable Render Pipeline Settings → HDRP Asset).
2) In HDRP Global Settings and your HDRP Asset:
   - Enable "Support DXR" and platform settings (Windows DX12).
   - Enable Ray Tracing features (Reflections, AO, Shadows, etc.).
3) Add post-processing/Volume overrides for Ray Traced Reflections/AO/Shadows as desired.
4) Add `RayTracingConfigurator` to a bootstrap GameObject and set `enableRayTracing = true`.
5) Add Scripting Define Symbol `HDRP_PRESENT` to silence warnings and indicate HDRP is present.

Notes
- For performance on mobile/low-end, keep ray tracing off and use SSR/SSAO.
- DXR targets Windows 10+, DX12, and RTX-class GPUs (or equivalent).
