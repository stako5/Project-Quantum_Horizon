Character Customization + Face Scan

Features
- Realistic constraints: height (145–210 cm), weight (35–180 kg), BMI guard, body ratios in safe human ranges.
- Avatar parameters: torso scale, limb ratios, colors, hair style, face blendshapes (0..100).
- Application: scales rig bones and blendshapes; optional ragdoll mass distribution.
- Face scan: webcam capture + placeholder reconstruction hook for AR/ML.
- Upload: serializes to JSON; call `set_avatar` reducer (server validates and stores).

Scripts
- AvatarDefinition.cs: data model for body/face params.
- AvatarApplier.cs: applies parameters to rig and SkinnedMeshRenderer.
- CustomizationUI.cs: sliders wired to AvatarDefinition and applier.
- FaceScanController.cs: webcam preview + capture, stub for generating blendshapes.
- AvatarUploader.cs: builds JSON and (placeholder) network upload.

Notes
- Integrate ARFoundation (ARKit/ARCore) or a face reconstruction SDK to fill blendshapes from the captured image; ensure user consent and secure storage for biometric data.
- Materials/colors: adapt AvatarApplier to set skin/hair/eye materials as needed.

ARFoundation Integration (optional)
- Packages: add `AR Foundation`, `ARKit Face Tracking` (for iOS), or an ARCore equivalent.
- Scripting Define Symbols: add `ARFOUNDATION_PRESENT` to enable ARFaceBlendMapper code paths.
- Scene: place `ARSession`, `ARSessionOrigin` with `ARFaceManager`, add `ARFaceBlendMapper` and reference `AvatarApplier`.
- iOS: `ARKitFaceSubsystem` provides blendshape coefficients that are mapped to AvatarDefinition.face and applied to your mesh blendshapes.

SpacetimeDB SDK Wiring
- Install the SpacetimeDB C# SDK and run `spacetime generate` to create typed table/reducer bindings.
- Add define symbol `SPACETIMEDB_SDK` to enable SDK-backed data sources and network calls.
- Configure `SpacetimeDbClient` (host/module) and call `ConnectAsync()` before uploading avatar (`AvatarUploader`) or reading Codex data.
